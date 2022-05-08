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
        loadText.text = "初始化网络";
        _ = NetCommand.Init();
        loadText.text = "校验资源包";
        _ = CheckAssetBundles();
    }

    private async Task CheckAssetBundles()
    {
        bool isNeedRestartApplication = false;
        DownLoadPath = Application.streamingAssetsPath + "/Assetbundles/";
        Directory.CreateDirectory(DownLoadPath);
        //加载MD5文件
        WebClient webClient = new WebClient();
        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) =>
        {
            processText.text = $"{e.BytesReceived / 1024 / 1024}/{e.TotalBytesToReceive / 1024 / 1024} MB. {e.ProgressPercentage} %"; ;
            slider.value = e.ProgressPercentage * 1.0f / 100;
        });
        string OnlieMD5FiIeDatas = webClient.DownloadString(@"http://106.15.38.165:7777/PC/MD5.json");
        var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
        //校验本地文件
        MD5 md5 = new MD5CryptoServiceProvider();
        foreach (var MD5FiIeData in Md5Dict)
        {
            FileInfo localFile = null;
            if (MD5FiIeData.Key.EndsWith(".dll")) //如果是dll，则根据不同平台对比游戏自身dll
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
                Debug.LogWarning(MD5FiIeData.Key + "校验成功");
                loadText.text = MD5FiIeData.Key + "校验成功";
            }
            else
            {
                loadText.text = MD5FiIeData.Key + "有新版本，开始重新下载";
                string savePath = localFile.FullName;
                //byte[] saveData = webClient.DownloadData(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}");




                if (MD5FiIeData.Key.EndsWith(".dll")) //如果dll，则需要重启游戏进行载入
                {
                    Debug.LogWarning("需要重启");
                    loadText.text = MD5FiIeData.Key + "更新代码资源";
                    //isNeedRestartApplication = true;
                    //Debug.LogWarning(savePath + ":" + saveData.Length);
                    await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
                    Debug.LogWarning("代码覆盖完毕，等待重启");
#if !UNITY_EDITOR
                    await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
#endif
                }
                else
                {
                    await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
                }
                Debug.LogWarning(MD5FiIeData.Key + "下载完成");
                loadText.text = MD5FiIeData.Key + "下载完成";
            }
        }
        md5.Dispose();
        webClient.Dispose();
        //如果改动了dll，需要重启
        if (isNeedRestartApplication) { Application.Quit(); }
        //加载AB包，并从中加载场景
        Debug.LogWarning("开始初始化AB包");
        loadText.text = "开始加载资源包";
        await AssetBundleCommand.Init();
        Debug.LogWarning("初始化完毕，加载场景。。。");
        SceneManager.LoadScene("1_LoginScene");
    }
}
