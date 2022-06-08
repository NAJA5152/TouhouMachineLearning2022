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
        loadText.text = "初始化网络";
        _ = NetCommand.Init();
        loadText.text = "校验资源包";
        await CheckAssetBundles();
    }

    private async Task CheckAssetBundles()
    {
        bool isNeedRestartApplication = false;
        bool isEditor = Application.isEditor;//是否是编辑器状态
        bool isMobile = Application.isMobilePlatform;//是否是移动平台

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
        Debug.LogError("MD5文件已加载完成");

        //校验本地文件
        MD5 md5 = new MD5CryptoServiceProvider();
        foreach (var MD5FiIeData in Md5Dict)
        {
            FileInfo localFile = null;
            if (MD5FiIeData.Key.EndsWith(".dll")) //如果是游戏程序集dll文件，则根据不同平台对比不同路径下的游戏程序集dll
            {
                Debug.LogError("当前程序集路径为" + Directory.GetCurrentDirectory());
                string currentGamePath = "";
                if (isEditor)
                {
                    currentGamePath = @"Library\ScriptAssemblies\TouHouMachineLearning.dll";
                    Debug.LogError("当前为编辑器" + currentGamePath);
                }
                else
                {
                    if (isMobile)
                    {
                        currentGamePath = Application.persistentDataPath;
                        Debug.LogError("当前为安卓,脚本路径在：" + currentGamePath);
                    }
                    else
                    {
                        //currentGamePath = (Directory.GetCurrentDirectory().Contains("PC") ? Directory.GetCurrentDirectory() : Directory.GetCurrentDirectory() + "\\PC") + @"\TouhouMachineLearning_Data\Managed\TouHouMachineLearning.dll";
                        currentGamePath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                        Debug.LogError("当前为windows,脚本路径在：" + currentGamePath);
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
                //Debug.LogWarning(MD5FiIeData.Key + "校验成功");
                loadText.text = MD5FiIeData.Key + "校验成功";
            }
            else
            {
                loadText.text = MD5FiIeData.Key + "有新版本，开始重新下载";
                Debug.LogError(MD5FiIeData.Key + "有新版本，开始重新下载");

                string savePath = localFile.FullName;

                if (MD5FiIeData.Key.EndsWith(".dll")) //如果dll，则需要重启游戏进行载入
                {
                    Debug.LogWarning("需要重启");
                    loadText.text = MD5FiIeData.Key + "更新代码资源";
                    isNeedRestartApplication = true;
                    Debug.LogError("代码覆盖路径:" + savePath);
                    if (!isEditor)
                    {
                        Debug.LogError("本地代码MD5值"+md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                        Debug.LogError("网络代码MD5值" + MD5FiIeData.Value.ToJson());

                        await webClient.DownloadFileTaskAsync(new System.Uri(@$"http://106.15.38.165:7777/PC/{MD5FiIeData.Key}"), savePath);
                        Debug.LogError("本地代码MD5值" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());

                        Debug.LogError("代码覆盖完毕，等待重启");
                    }
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
        if (isNeedRestartApplication) 
        {
            //弹个窗，确认得话重启
            Application.Quit();
        }
        //加载AB包，并从中加载场景
        Debug.LogWarning("开始初始化AB包");
        loadText.text = "开始加载资源包";
        await SceneCommand.InitAsync(true); 
        Debug.LogWarning("初始化完毕，加载场景。。。");
        SceneManager.LoadScene("1_LoginScene");
    }
}
