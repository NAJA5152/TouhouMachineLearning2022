using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using System.Reflection;
using TouhouMachineLearningSummary.Thread;

public class HotFixedManager : MonoBehaviour
{
    public Text loadText;
    public Text processText;
    public Text versiousText;
    public Slider slider;
    public GameObject RestartNotice;
    async void Start()
    {
        RestartNotice.transform.localScale = new Vector3(1, 0, 1);
        versiousText.text = "��Ҫ�ĳ�v1";
        ConfigManager.InitConfig();
        loadText.text = "��ʼ������";
        _ = NetCommand.Init();
        loadText.text = "У����Դ��";
        await CheckAssetBundles();
    }

    private async Task CheckAssetBundles()
    {
        bool isNeedRestartApplication = false;
        bool isEditor = Application.isEditor;//�Ƿ��Ǳ༭��״̬
        bool isMobile = Application.isMobilePlatform;//�Ƿ����ƶ�ƽ̨
        loadText.text = "���AB����";

        //�༭��ģʽ�²���������
        if (!isEditor)
        {
            //AB���洢·��
            string DownLoadPath = isMobile switch
            {
                true => Application.persistentDataPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/",
                false => Application.streamingAssetsPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/"
            };
            //���򼯴洢·��
            string dllFIleRootPath = isMobile switch
            {
                true => Application.persistentDataPath,
                false => Directory.GetCurrentDirectory()
            };
            loadText.text = "��ʼ�����ļ�" ;
            Debug.LogWarning("��ʼ�����ļ�" + System.DateTime.Now);
            if (isMobile)
            {
                var direPath = new DirectoryInfo(Application.persistentDataPath);
                File.WriteAllText(Application.persistentDataPath + "/2.ini", "1");
                loadText.text = Application.persistentDataPath + "/2.ini"+File.Exists(Application.persistentDataPath + "/2.txt");
                File.WriteAllText(direPath.FullName + "/1.ini","1");
                File.WriteAllText(direPath.Parent.FullName + "/3.ini","1");
                File.WriteAllText(direPath.Parent.FullName + @"/pram-shadow-files/assets/bin/Data/4.ini","1");
                //loadText.text = "��׿�����ݴ洢·��Ϊ" + direPath.FullName;
                Debug.LogError("��׿�����ݴ洢·��Ϊ" + direPath.FullName);
                direPath.GetFiles("*.*").ForEach(file =>
                {
                    Debug.LogError("��׿�����ļ�Ϊ" + file.FullName);
                    loadText.text += "��׿�����ļ�Ϊ" + file.FullName;
                });
            }
            Directory.CreateDirectory(DownLoadPath);

            //����MD5�ļ�
            WebClient webClient = new WebClient();
            //���������ٶ�
            webClient.Proxy = null;
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) =>
            {
                processText.text = $"{e.BytesReceived / 1024 / 1024}/{e.TotalBytesToReceive / 1024 / 1024} MB. {e.ProgressPercentage} %"; ;
                slider.value = e.ProgressPercentage * 1.0f / 100;
            });
            string OnlieMD5FiIeDatas = webClient.DownloadString(@$"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/MD5.json");
            var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
            Debug.LogError("MD5�ļ��Ѽ������");

            loadText.text = "MD5�ļ��Ѽ�����ɣ�";
            //Directory.CreateDirectory(DownLoadPath);

            //У�鱾���ļ�
            MD5 md5 = new MD5CryptoServiceProvider();
            foreach (var MD5FiIeData in Md5Dict)
            {
                //��ǰУ��ı����ļ�
                FileInfo localFile = null;
                if (MD5FiIeData.Key.EndsWith(".dll")) //�������Ϸ����dll�ļ�������ݲ�ͬƽ̨�ԱȲ�ͬ·���µ���Ϸ����dll
                {
                    Debug.LogError("��ǰ����·��Ϊ" + Directory.GetCurrentDirectory());
                    string currentDllPath = new DirectoryInfo(dllFIleRootPath).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                    Debug.LogError("��ǰΪ�ű�·���ڣ�" + currentDllPath);
                    localFile = new FileInfo(currentDllPath);
                }
                else
                {
                    localFile = new FileInfo(DownLoadPath + MD5FiIeData.Key);
                }

                if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                {
                    loadText.text = MD5FiIeData.Key + "У��ɹ�";
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
                        if (!isEditor)
                        {
                            Debug.LogError("���ش���MD5ֵ" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                            Debug.LogError("�������MD5ֵ" + MD5FiIeData.Value.ToJson());

                            await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/{MD5FiIeData.Key}"), savePath);
                            Debug.LogError("���ش���MD5ֵ" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());

                            Debug.LogError("���븲����ϣ��ȴ�����");
                        }
                    }
                    else
                    {
                        await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/{MD5FiIeData.Key}"), savePath);
                    }
                    Debug.LogWarning(MD5FiIeData.Key + "�������");
                    loadText.text = MD5FiIeData.Key + "�������";
                }
                Debug.LogWarning("���������ļ�" + System.DateTime.Now);
            }
            md5.Dispose();
            webClient.Dispose();
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
        var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouhouMachineLearning.exe", SearchOption.AllDirectories).FirstOrDefault();
        if (game != null)
        {
            System.Diagnostics.Process.Start(game.FullName);
        }
        Application.Quit();
    }
}
