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

public class HotFixedManager : MonoBehaviour
{
    public TextMeshProUGUI loadText;
    public TextMeshProUGUI processText;
    public TextMeshProUGUI versiousText;
    public Slider slider;
    string DownLoadPath { get; set; } = "";
    async void Start()
    {
        versiousText.text = "v1";
        ConfigManager.InitConfig();
        loadText.text = "��ʼ������";
        _ = NetCommand.Init();
        loadText.text = "У����Դ��";
        _ = CheckAssetBundles();
    }

    private async Task CheckAssetBundles()
    {
        bool isNeedRestartApplication = false;
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
        //У�鱾���ļ�
        MD5 md5 = new MD5CryptoServiceProvider();
        foreach (var MD5FiIeData in Md5Dict)
        {
            FileInfo localFile = null;
            if (MD5FiIeData.Key.EndsWith(".dll")) //�����dll������ݲ�ͬƽ̨�Ա���Ϸ����dll
            {
#if UNITY_EDITOR
                localFile = new FileInfo(@"Library\ScriptAssemblies\TouHouMachineLearning.dll");
#elif UNITY_STANDALONE_WIN
                localFile = new FileInfo(@"TouhouMachineLearning_Data\Managed\TouHouMachineLearning.dll");
#elif UNITY_ANDROID
                localFile = new FileInfo(Application.persistentDataPath);
#endif
            }
            else
            {
                localFile = new FileInfo(DownLoadPath + MD5FiIeData.Key);
            }

            if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
            {
                Debug.LogWarning(MD5FiIeData.Key + "У��ɹ�");
                loadText.text = MD5FiIeData.Key + "У��ɹ�";
            }
            else
            {
                loadText.text = MD5FiIeData.Key + "���°汾����ʼ��������";
                string savePath = localFile.FullName;
                //byte[] saveData = webClient.DownloadData(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}");




                if (MD5FiIeData.Key.EndsWith(".dll")) //���dll������Ҫ������Ϸ��������
                {
                    Debug.LogWarning("��Ҫ����");
                    loadText.text = MD5FiIeData.Key + "���´�����Դ";
                    //isNeedRestartApplication = true;
                    //Debug.LogWarning(savePath + ":" + saveData.Length);
                    await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
                    Debug.LogWarning("���븲����ϣ��ȴ�����");
#if !UNITY_EDITOR
                    await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
#endif
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
        if (isNeedRestartApplication) { Application.Quit(); }
        //����AB���������м��س���
        Debug.LogWarning("��ʼ��ʼ��AB��");
        loadText.text = "��ʼ������Դ��";
        await AssetBundleCommand.Init();
        Debug.LogWarning("��ʼ����ϣ����س���������");
        SceneManager.LoadScene("1_LoginScene");
    }
}
