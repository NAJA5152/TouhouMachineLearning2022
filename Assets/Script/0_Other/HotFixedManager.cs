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

    static bool isEditor = Application.isEditor;//是否是编辑器状态
    static bool isMobile = Application.isMobilePlatform;//是否是移动平台
    static string downLoadPath = isMobile switch
    {
        true => Application.persistentDataPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/",
        false => Application.streamingAssetsPath + $"/Assetbundles/{ConfigManager.GetServerTag()}/"
    };
    //程序集存储路径
    static string dllFIleRootPath = isMobile switch
    {
        true => new DirectoryInfo(Application.persistentDataPath).Parent.FullName,
        false => Directory.GetCurrentDirectory()
    };
    async void Start()
    {
        RestartNotice.transform.localScale = new Vector3(1, 0, 1);
        versiousText.text = "我要改成v4";
        ConfigManager.InitConfig();
        //loadText.text = "初始化网络";
        //_ = NetCommand.Init();
        loadText.text = "校验资源包";
        await CheckAssetBundles();
    }
    //已下好任务数
    static int downloadTaskCount = 0;
    //总下载任务
    static List<Task> downloadTaskList = new List<Task>();
    //当前下载文件
    static string currentDownloadFileName;

    private void Update()
    {
        loadText.text = currentDownloadFileName;
        processText.text = $"{downloadTaskCount}/{downloadTaskList.Count} %"; ;
        slider.value = downloadTaskList.Count == 0 ? 1 : downloadTaskCount * 1f / downloadTaskList.Count;
    }
    //校验本地文件
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
            var httpClient = new HttpClient();
            var responseMessage =await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/MD5.json");
            if (!responseMessage.IsSuccessStatusCode)
            {
                loadText.text = "MD5文件获取出错";
                return;
            }

            var OnlieMD5FiIeDatas =await responseMessage.Content.ReadAsStringAsync();
            var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
            Debug.Log("MD5文件已加载完成" + OnlieMD5FiIeDatas);
            loadText.text = "MD5文件已加载完成：";

            //校验本地文件
            downloadTaskCount = 0;
            downloadTaskList.Clear();

            foreach (var MD5FiIeData in Md5Dict)
            {
                //当前校验的本地文件
                FileInfo localFile = null;
                if (MD5FiIeData.Key.EndsWith(".dll")) //如果是游戏程序集dll文件，则根据不同平台对比不同路径下的游戏程序集dll
                {
                    if (isMobile)
                    {
                        continue;
                    }
                    Debug.LogError("当前程序集路径为" + dllFIleRootPath);
                    string currentDllPath = new DirectoryInfo(dllFIleRootPath).GetFiles("TouHouMachineLearning.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                    Debug.LogError("当前为脚本路径在：" + currentDllPath);
                    localFile = new FileInfo(currentDllPath);
                    Debug.Log("本地dll文件md5值为：" + md5.ComputeHash(File.ReadAllBytes(localFile.FullName)).ToJson());
                    Debug.Log("本地dll文件修改时间为：" + localFile.LastWriteTime);
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
                        currentDownloadFileName = "正在下载：" + MD5FiIeData.Key;
                        var fileData = await httpClient.GetAsync($"http://106.15.38.165:7777/AssetBundles/{ConfigManager.GetServerTag()}/{MD5FiIeData.Key}").Result.Content.ReadAsByteArrayAsync();
                        File.WriteAllBytes(savePath, fileData);
                        currentDownloadFileName = "下载完成：" + MD5FiIeData.Key;
                        Debug.LogWarning(MD5FiIeData.Key + "下载完成");
                        Debug.LogWarning("结束下载文件" + localFile.Name + " " + System.DateTime.Now);
                        downloadTaskCount++;
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
