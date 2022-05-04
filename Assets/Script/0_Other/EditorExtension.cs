#if UNITY_EDITOR
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TouhouMachineLearningSummary.Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using TouhouMachineLearningSummary.Extension;
using System.Net;

namespace TouhouMachineLearningSummary.Other
{
    public class EditorExtension : MonoBehaviour
    {
        [MenuItem("Tools/打开服务端", false, 1)]
        static void StartServer() => Process.Start(@"OtherSolution\Server\bin\Debug\net6.0\Server.exe");
        [MenuItem("Tools/打开游戏客户端", false, 2)]
        static void StartClient() => Process.Start(@"Pc\TouhouMachineLearning.exe");
        [MenuItem("Tools/打开数据表格", false, 51)]
        static void OpenXls() => Process.Start(@"Assets\Resources\GameData\GameData.xlsx");
        [MenuItem("Tools/打开表格数据实时同步工具", false, 52)]
        static void UpdateXls() => Process.Start(@"OtherSolution\xls检测更新\bin\Debug\net6.0\xls检测更新.exe");
        [MenuItem("Tools/发布当前卡牌版本", false, 101)]
        static void UpdateCardSpace()
        {
            var gameCardAssembly = new DirectoryInfo(@"Library\ScriptAssemblies").GetFiles("GameCard*.dll").FirstOrDefault();
            var singleCardFile = new FileInfo(@"Assets\Resources\GameData\CardData-Single.json");
            var multiCardFile = new FileInfo(@"Assets\Resources\GameData\CardData-Multi.json");
            if (gameCardAssembly != null && singleCardFile != null && multiCardFile != null)
            {
                CardConfig cardConfig = new CardConfig(DateTime.Today.ToString("yyy_MM_dd"), gameCardAssembly, singleCardFile, multiCardFile);
                _ = Command.NetCommand.UploadCardConfigsAsync(cardConfig);
            }
            else
            {
                UnityEngine.Debug.LogError("检索不到卡牌dll文件");
            }
        }
        [MenuItem("Tools/发布当前服务器版本", false, 102)]
        static async void UpdateServer()
        {
            var VersionsHub = new HubConnectionBuilder().WithUrl($"http://106.15.38.165:233/VersionsHub").Build();
            //VersionsHub = new HubConnectionBuilder().WithUrl($"http://127.0.0.1:233/VersionsHub").Build();
            await VersionsHub.StartAsync();
            var result = await VersionsHub.InvokeAsync<bool>("UpdateServer", File.ReadAllBytes(@"OtherSolution\Server\bin\Debug\net6.0\Server.dll"));
            UnityEngine.Debug.LogWarning("上传结果" + result);
            await VersionsHub.StopAsync();
        }
        [MenuItem("Tools/发布游戏热更资源文件", priority = 103)]
        static async void BuildAssetBundles()
        {
            //打标签
            new DirectoryInfo(@"Assets\Resources")
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Where(file => file.Extension != ".meta")
                .ToList()
                .ForEach(file =>
                {
                    string path = file.FullName.Replace(Directory.GetCurrentDirectory() + @"\", "");
                    UnityEngine.Debug.Log(path);
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    importer.assetBundleName = "res.gezi";
                });
            UnityEngine.Debug.LogWarning("标签修改完毕，开始打包");
            string PcOutputPath = Directory.GetCurrentDirectory() + @"\AssetBundles\PC";
            string AndroidOutputPath = Directory.GetCurrentDirectory() + @"\AssetBundles\Android";
            Directory.CreateDirectory(PcOutputPath);
            Directory.CreateDirectory(AndroidOutputPath);
            BuildPipeline.BuildAssetBundles(PcOutputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            UnityEngine.Debug.LogWarning("PC打包完毕");
            //BuildPipeline.BuildAssetBundles(AndroidOutputPath, BuildAssetBundleOptions.None, BuildTarget.Android);
            UnityEngine.Debug.LogWarning("安卓打包完毕");
            UnityEngine.Debug.LogWarning("开始生成MD5值校验文件");
            new FileInfo(Directory.GetCurrentDirectory() + @"\Library\ScriptAssemblies\TouHouMachineLearning.dll").CopyTo(PcOutputPath + "\\TouHouMachineLearning.dll", true);
            new FileInfo(Directory.GetCurrentDirectory() + @"\Library\ScriptAssemblies\TouHouMachineLearning.dll").CopyTo(AndroidOutputPath + "\\TouHouMachineLearning.dll", true);

            var localMD5Dict = CreatMD5FIle(PcOutputPath);
            CreatMD5FIle(AndroidOutputPath);
            UnityEngine.Debug.LogWarning("MD5值校验生成完毕,开始上传文件");


            //获取网络md5文件，判断需要上传的文件
            WebClient webClient = new WebClient();
            string OnlieMD5FiIeDatas = webClient.DownloadString(@"http://106.15.38.165:7777/PC/MD5.json");
            webClient.Dispose();

            var onlineMD5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();

            //上传pc端AB包
            var touhouHub = new HubConnectionBuilder().WithUrl($"http://106.15.38.165:495/TouHouHub").Build();
            touhouHub.ServerTimeout = new TimeSpan(0, 5, 0);
            await touhouHub.StartAsync();
            foreach (var item in localMD5Dict)
            {
                //如果文件不存在或者md5值不相等才上传
                if (!onlineMD5Dict.ContainsKey(item.Key) || !onlineMD5Dict[item.Key].SequenceEqual(item.Value))
                {
                    UnityEngine.Debug.LogWarning(item.Key + "开始传输");
                    var result = await touhouHub.InvokeAsync<bool>("UploadAssetBundles", @$"AssetBundles/PC/{item.Key }", File.ReadAllBytes(@$"AssetBundles/PC/{item.Key }"));
                    UnityEngine.Debug.LogWarning(item.Key + "传输" + result);
                }
                else
                {
                    UnityEngine.Debug.LogWarning(item.Key + "无更改，无需上传");
                }
            }
            await touhouHub.InvokeAsync<bool>("UploadAssetBundles", @$"AssetBundles/PC/MD5.json", File.ReadAllBytes(@$"AssetBundles/PC/MD5.json"));
            UnityEngine.Debug.LogWarning("MD5.json上传完成");
            await touhouHub.StopAsync();

            Dictionary<string, byte[]> CreatMD5FIle(string direPath)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                Dictionary<string, byte[]> MD5s = new();
                new DirectoryInfo(direPath).GetFiles("*.*").ToList().ForEach(file =>
                {
                    //只上传代码和gezi文件
                    if (file.Extension == ".gezi" || file.Extension == ".dll")
                    {
                        byte[] result = md5.ComputeHash(File.ReadAllBytes(file.FullName));
                        MD5s[file.Name] = result;
                    }
                });
                md5.Dispose();
                File.WriteAllText(direPath + @"\MD5.json", MD5s.ToJson());
                return MD5s;
            }
        }

        [MenuItem("Scene/载入热更场景", priority = 151)]
        static void LoadHotfixedScene()
        {
            Process.Start(@"Assets\Scenes\0_HotfixedScene.unity");
        }
        [MenuItem("Scene/载入登录场景", priority = 152)]
        static void LoadLoginScene()
        {
            Process.Start(@"Assets\Scenes\1_LoginScene.unity");
        }
        [MenuItem("Scene/载入对战场景", priority = 153)]
        static void LoaBattleScene()
        {
            Process.Start(@"Assets\Scenes\2_BattleScene.unity");
        }
    }
}
#endif
