using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotFixedManager : MonoBehaviour
{
    public Text loadText;
    public Text processText;
    public Text versiousText;
    public Slider slider;
    public GameObject RestartNotice;

    MD5 md5 = new MD5CryptoServiceProvider();

    static bool isEditor = Application.isEditor;//�Ƿ��Ǳ༭��״̬
    static bool isMobile = Application.isMobilePlatform;//�Ƿ����ƶ�ƽ̨
    string downLoadPath = isMobile switch
    {
        true => Application.persistentDataPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/",
        false => Application.streamingAssetsPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/"
    };
    //���򼯴洢·��
    string dllFIleRootPath = isMobile switch
    {
        true => new DirectoryInfo(Application.persistentDataPath).Parent.FullName,
        false => Directory.GetCurrentDirectory()
    };
    async void Start()
    {
        RestartNotice.transform.localScale = new Vector3(1, 0, 1);
        versiousText.text = "��Ҫ�ĳ�v4";
        ConfigManager.InitConfig();
        //loadText.text = "��ʼ������";
        //_ = NetCommand.Init();
        loadText.text = "У����Դ��";
        await CheckAssetBundles();
    }
    //���º�������
    int downloadTaskCount = 0;
    //����������
    List<Task> downloadTaskList = new List<Task>();
    //��ǰ�����ļ�
    string currentDownloadFileName;

    private void Update()
    {
        loadText.text = currentDownloadFileName;
        processText.text = $"{downloadTaskCount}/{downloadTaskList.Count} %"; ;
        slider.value = downloadTaskList.Count == 0 ? 1 : downloadTaskCount * 1f / downloadTaskList.Count;
    }
    //У�鱾���ļ�
    private async Task CheckAssetBundles()
    {
        bool isNeedRestartApplication = false;
        loadText.text = "���AB����";
        //�༭��ģʽ�²���������
        if (!isEditor)
        {
            //AB���洢·��

            loadText.text = "��ʼ�����ļ�";
            Debug.LogWarning("��ʼ�����ļ�" + System.DateTime.Now);
            Directory.CreateDirectory(downLoadPath);
            var httpClient = new HttpClient();
            var responseMessage = httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/MD5.json").Result;
            if (!responseMessage.IsSuccessStatusCode)
            {
                loadText.text = "MD5�ļ���ȡ����";
                return;
            }

            var OnlieMD5FiIeDatas = responseMessage.Content.ReadAsStringAsync().Result;
            var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
            Debug.Log("MD5�ļ��Ѽ������" + OnlieMD5FiIeDatas);
            loadText.text = "MD5�ļ��Ѽ�����ɣ�";

            //У�鱾���ļ�
            downloadTaskCount = 0;
            downloadTaskList.Clear();

            foreach (var MD5FiIeData in Md5Dict)
            {
                //��ǰУ��ı����ļ�
                FileInfo localFile = null;
                if (MD5FiIeData.Key.EndsWith(".dll")) //�������Ϸ����dll�ļ�������ݲ�ͬƽ̨�ԱȲ�ͬ·���µ���Ϸ����dll
                {
                    Debug.LogError("��ǰ����·��Ϊ" + dllFIleRootPath);
                    string currentDllPath = new DirectoryInfo(dllFIleRootPath).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                    Debug.LogError("��ǰΪ�ű�·���ڣ�" + currentDllPath);
                    localFile = new FileInfo(currentDllPath);
                    Debug.Log("����dll�ļ�md5ֵΪ��" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                    Debug.Log("����dll�ļ��޸�ʱ��Ϊ��" + localFile.LastWriteTime);
                }
                else
                {
                    localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                }

                if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                {
                    loadText.text = MD5FiIeData.Key + "У��ɹ�";
                    Debug.LogWarning(MD5FiIeData.Key + "У��ɹ�");
                }
                else
                {
                    loadText.text = MD5FiIeData.Key + "���°汾����ʼ��������";
                    Debug.LogError(MD5FiIeData.Key + "���°汾����ʼ��������");

                    string savePath = localFile.FullName;

                    if (MD5FiIeData.Key.EndsWith(".dll")) //���dll������Ҫ������Ϸ��������
                    {
                        Debug.LogWarning("��Ҫ����");
                        loadText.text = MD5FiIeData.Key + "���´�����Դ";
                        isNeedRestartApplication = true;
                        Debug.LogError("���븲��·��:" + savePath);
                        Debug.LogError("���ش���MD5ֵ" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                        Debug.LogError("�������MD5ֵ" + MD5FiIeData.Value.ToJson());
                    }
                    downloadTaskList.Add(Task.Run(async () =>
                    {
                        currentDownloadFileName = "�������أ�" + MD5FiIeData.Key;
                        var fileData = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/{MD5FiIeData.Key}").Result.Content.ReadAsByteArrayAsync();
                        File.WriteAllBytes(savePath, fileData);
                        currentDownloadFileName = "������ɣ�" + MD5FiIeData.Key;
                        Debug.LogWarning(MD5FiIeData.Key + "�������");
                        Debug.LogWarning("���������ļ�" + localFile.Name + " " + System.DateTime.Now);
                        downloadTaskCount++;
                    }));
                }
            }
            await Task.WhenAll(downloadTaskList);
            Debug.LogWarning("ȫ���������");
            loadText.text = "ȫ���������";
            md5.Dispose();
            httpClient.Dispose();
            //����Ķ���dll����Ҫ����
            if (isNeedRestartApplication)
            {
                Debug.Log("��Ҫ����");
                //��������ȷ�ϵû�����
                RestartNotice.GetComponent<AudioSource>().Play();
                await CustomThread.TimerAsync(0.5f, (process) =>
                {
                    RestartNotice.transform.localScale = new Vector3(1, process, 1);
                });
                //�ȴ��û����������ٽ��м���
                return;
            }
        }

        //����AB���������м��س���
        Debug.LogWarning("��ʼ��ʼ��AB��");
        AssetBundle.UnloadAllAssetBundles(true);
        loadText.text = "��ʼ������Դ��";
        await SceneCommand.InitAsync(true);
        Debug.LogWarning("��ʼ����ϣ����س���������");
        SceneManager.LoadScene("1_LoginScene");
    }



    public void RestartGame()
    {
        if (isMobile)
        {
            //SceneManager.LoadScene(0);
            Application.Quit();
        }
        else
        {
            var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouhouMachineLearning.exe", SearchOption.AllDirectories).FirstOrDefault();
            if (game != null)
            {
                System.Diagnostics.Process.Start(game.FullName);
            }
            Application.Quit();
        }
    }
}
