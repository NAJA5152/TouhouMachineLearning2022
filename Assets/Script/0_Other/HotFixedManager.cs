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

public class HotFixedManager : MonoBehaviour
{
    public TextMeshProUGUI loadText;
    public TextMeshProUGUI processText;
    public TextMeshProUGUI versiousText;
    public Slider slider;
    string DownLoadPath { get; set; } = "";
    async void Start()
    {
        versiousText.text = "v6";
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

        DownLoadPath = Application.streamingAssetsPath + "/Assetbundles/";
        Directory.CreateDirectory(DownLoadPath);
        //����MD5�ļ�
        WebClient webClient = new WebClient();
        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) =>
        {
            processText.text = $"{e.BytesReceived / 1024 / 1024}/{e.TotalBytesToReceive / 1024 / 1024} MB. {e.ProgressPercentage} %"; ;
            slider.value = e.ProgressPercentage * 1.0f / 100;
        });
        string OnlieMD5FiIeDatas = webClient.DownloadString(@"http://106.15.38.165:7777/PC/MD5.json");
        var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
        Debug.LogError("MD5�ļ��Ѽ������");

        //У�鱾���ļ�
        MD5 md5 = new MD5CryptoServiceProvider();
        foreach (var MD5FiIeData in Md5Dict)
        {
            FileInfo localFile = null;
            if (MD5FiIeData.Key.EndsWith(".dll")) //�������Ϸ����dll�ļ�������ݲ�ͬƽ̨�ԱȲ�ͬ·���µ���Ϸ����dll
            {
                Debug.LogError("��ǰ����·��Ϊ" + Directory.GetCurrentDirectory());
                string currentGamePath = "";
                if (isEditor)
                {
                    currentGamePath = @"Library\ScriptAssemblies\TouHouMachineLearning.dll";
                    Debug.LogError("��ǰΪ�༭��" + currentGamePath);
                }
                else
                {
                    if (isMobile)
                    {
                        currentGamePath = Application.persistentDataPath;
                        Debug.LogError("��ǰΪ��׿,�ű�·���ڣ�" + currentGamePath);
                    }
                    else
                    {
                        //currentGamePath = (Directory.GetCurrentDirectory().Contains("PC") ? Directory.GetCurrentDirectory() : Directory.GetCurrentDirectory() + "\\PC") + @"\TouhouMachineLearning_Data\Managed\TouHouMachineLearning.dll";
                        currentGamePath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                        Debug.LogError("��ǰΪwindows,�ű�·���ڣ�" + currentGamePath);
                    }
                }
                localFile = new FileInfo(currentGamePath);
            }
            else
            {
                localFile = new FileInfo(DownLoadPath + MD5FiIeData.Key);
            }

            if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
            {
                //Debug.LogWarning(MD5FiIeData.Key + "У��ɹ�");
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
                        Debug.LogError("���ش���MD5ֵ"+md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                        Debug.LogError("�������MD5ֵ" + MD5FiIeData.Value.ToJson());

                        await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
                        Debug.LogError("���ش���MD5ֵ" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());

                        Debug.LogError("���븲����ϣ��ȴ�����");
                    }
                }
                else
                {
                    await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
                }
                Debug.LogWarning(MD5FiIeData.Key + "�������");
                loadText.text = MD5FiIeData.Key + "�������";
            }
        }
        md5.Dispose();
        webClient.Dispose();
        //����Ķ���dll����Ҫ����
        if (isNeedRestartApplication) 
        {
            //��������ȷ�ϵû�����
            Application.Quit();
        }
        //����AB���������м��س���
        Debug.LogWarning("��ʼ��ʼ��AB��");
        loadText.text = "��ʼ������Դ��";
        await SceneCommand.InitAsync(true); 
        Debug.LogWarning("��ʼ����ϣ����س���������");
        SceneManager.LoadScene("1_LoginScene");
    }
}
