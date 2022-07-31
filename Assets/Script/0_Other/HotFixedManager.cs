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
using System.Text;
using System.Net.Sockets;
using System.Net.Http;

public class HotFixedManager : MonoBehaviour
{
    public Text loadText;
    public Text processText;
    public Text versiousText;
    public Slider slider;
    public GameObject RestartNotice;

    MD5 md5 = new MD5CryptoServiceProvider();

    static bool isEditor = Application.isEditor;//是否是编辑器状态
    static bool isMobile = Application.isMobilePlatform;//是否是移动平台
    string downLoadPath = isMobile switch
    {
        true => Application.persistentDataPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/",
        false => Application.streamingAssetsPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/"
    };
    //程序集存储路径
    string dllFIleRootPath = isMobile switch
    {
        true => new DirectoryInfo(Application.persistentDataPath).Parent.FullName,
        false => Directory.GetCurrentDirectory()
    };
    //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) =>
    //{
    //    processText.text = $"{e.BytesReceived / 1024 / 1024}/{e.TotalBytesToReceive / 1024 / 1024} MB. {e.ProgressPercentage} %"; ;
    //    slider.value = e.ProgressPercentage * 1.0f / 100;
    //});
    async void Start()
    {
        RestartNotice.transform.localScale = new Vector3(1, 0, 1);
        versiousText.text = "我要改成v2";
        ConfigManager.InitConfig();
        //loadText.text = "初始化网络";
        //_ = NetCommand.Init();
        loadText.text = "校验资源包";
        await CheckAssetBundles();
    }

    private async Task CheckAssetBundles()
    {
        bool isNeedRestartApplication = false;
        loadText.text = "检查AB包中";
        //编辑器模式下不进行下载
        if (!isEditor)
        {
            //AB包存储路径

            loadText.text = "开始下载文件";
            Debug.LogWarning("开始下载文件" + System.DateTime.Now);
            Directory.CreateDirectory(downLoadPath);

            //加载MD5文件
            var httpClient = new HttpClient();
            var responseMessage = httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/MD5.json").Result;
            if (!responseMessage.IsSuccessStatusCode)
            {
                loadText.text = "MD5文件获取出错";
                return;
            }

            var OnlieMD5FiIeDatas = responseMessage.Content.ReadAsStringAsync().Result;
            var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
            Debug.Log("MD5文件已加载完成" + OnlieMD5FiIeDatas);
            loadText.text = "MD5文件已加载完成：";

            //校验本地文件
            List<Task> downloadTaskList = new();
            foreach (var MD5FiIeData in Md5Dict)
            {
                //当前校验的本地文件
                FileInfo localFile = null;
                if (MD5FiIeData.Key.EndsWith(".dll")) //如果是游戏程序集dll文件，则根据不同平台对比不同路径下的游戏程序集dll
                {
                    Debug.LogError("当前程序集路径为" + dllFIleRootPath);
                    string currentDllPath = new DirectoryInfo(dllFIleRootPath).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                    Debug.LogError("当前为脚本路径在：" + currentDllPath);
                    localFile = new FileInfo(currentDllPath);
                    Debug.Log("本地dll文件md5值为：" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                }
                else
                {
                    localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                }

                if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                {
                    loadText.text = MD5FiIeData.Key + "校验成功";
                    Debug.LogWarning(MD5FiIeData.Key + "校验成功");
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
                        Debug.LogError("本地代码MD5值" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                        Debug.LogError("网络代码MD5值" + MD5FiIeData.Value.ToJson());
                    }
                    downloadTaskList.Add(Task.Run(async () =>
                    {
                        var fileData = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/{MD5FiIeData.Key}").Result.Content.ReadAsByteArrayAsync();
                        File.WriteAllBytes(savePath, fileData);
                        Debug.LogWarning(MD5FiIeData.Key + "下载完成");
                        loadText.text = MD5FiIeData.Key + "下载完成";
                        Debug.LogWarning("结束下载文件" + localFile.Name + " " + System.DateTime.Now);
                    }));
                }
            }
            await Task.WhenAll(downloadTaskList);
            Debug.LogWarning("全部下载完成");
            loadText.text = "全部下载完成";
            md5.Dispose();
            httpClient.Dispose();
            //如果改动了dll，需要重启
            if (isNeedRestartApplication)
            {
                Debug.Log("需要重启");
                //弹个窗，确认得话重启
                RestartNotice.GetComponent<AudioSource>().Play();
                await CustomThread.TimerAsync(0.5f, (process) =>
                {
                    RestartNotice.transform.localScale = new Vector3(1, process, 1);
                });
                //等待用户重启，不再进行加载
                return;
            }
        }

        //加载AB包，并从中加载场景
        Debug.LogWarning("开始初始化AB包");
        AssetBundle.UnloadAllAssetBundles(true);
        loadText.text = "开始加载资源包";
        await SceneCommand.InitAsync(true);
        Debug.LogWarning("初始化完毕，加载场景。。。");
        SceneManager.LoadScene("1_LoginScene");
    }
    public void RestartGame()
    {
        if (isMobile)
        {
            SceneManager.LoadScene(0);
            //Application.Quit();
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
