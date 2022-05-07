using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class AssetBundleCommand
    {
        static bool AlreadyInit { get; set; } = false;
        static Dictionary<string, List<UnityEngine.Object>> assets = new();
        public static async Task Init(Action completedAction = null)
        {
            if (AlreadyInit) { return; }
            AlreadyInit = true;
            Directory.CreateDirectory(Application.streamingAssetsPath + "/AssetBundles/");
            foreach (var file in new DirectoryInfo(Application.streamingAssetsPath + "/AssetBundles/")
                .GetFiles()
                .Where(file => file.Name.Contains("scene1") && !file.Name.Contains("meta")))
            {
                await LoadAssetBundle(file.FullName);
                Debug.LogWarning($"{file.FullName}加载完毕");
            }

            
            completedAction?.Invoke();

            foreach (var file in new DirectoryInfo(Application.streamingAssetsPath + "/AssetBundles/")
                .GetFiles()
                .Where(file => file.Name.Contains("scene2") && !file.Name.Contains("meta")))
            {
                _ = LoadAssetBundle(file.FullName);
                Debug.LogWarning($"{file.FullName}加载完毕");
            }

            foreach (var ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                assets[ab.name] = ab.LoadAllAssets().ToList();
            }
            Debug.LogWarning("资源加载完毕");

            async Task<AssetBundle> LoadAssetBundle(string path)
            {
                var ABLoadRequir = AssetBundle.LoadFromFileAsync(path);
                while (!ABLoadRequir.isDone) { await Task.Delay(50); }
                return ABLoadRequir.assetBundle;
            }
        }
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            return default;
        }
        /// <summary>
        /// 从带有tag名的AB包中加载素材
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T Load<T>(string tag, string fileName) where T : UnityEngine.Object
        {
            var targetAssets = assets.FirstOrDefault(asset => asset.Key.Contains(tag.ToLower())).Value;
            if (targetAssets != null)
            {
                var targetAsset = targetAssets.FirstOrDefault(asset => asset.name == fileName);
                return targetAsset as T;
            }
            return null;
            //foreach (var ab in AssetBundle.GetAllLoadedAssetBundles())
            //{
            //    var a = ab.GetAllAssetNames();
            //    if (ab.Contains(path))
            //    {
            //        return ab.LoadAsset(path, typeof(T)) as T;
            //    }
            //}
            //return null;
        }
    }
}
