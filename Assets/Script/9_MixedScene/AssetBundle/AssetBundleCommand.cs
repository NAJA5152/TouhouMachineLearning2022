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
        static AssetBundle Scene1AB { get; set; }
        static AssetBundle Scene2AB { get; set; }
        List<AssetBundle> AssetBundles { get;}
        public static async void Init(Action completedAction)
        {
            if (AlreadyInit) { return; }
            AlreadyInit = true;

            Scene1AB = await LoadAssetBundle(Application.streamingAssetsPath + "/AssetBundles/scene1resource.gezi");
            Debug.LogWarning("场景1资源加载完毕");
            Debug.LogWarning("场景1加载完毕");
            foreach (var file in new DirectoryInfo(Application.streamingAssetsPath + "/AssetBundles/").GetFiles().Where(file => file.Name.Contains("scene1")))
            {
                await LoadAssetBundle(file.FullName);
                Debug.LogWarning($"{file.FullName}加载完毕");
            }

            completedAction?.Invoke();

            foreach (var file in new DirectoryInfo(Application.streamingAssetsPath + "/AssetBundles/").GetFiles().Where(file => file.Name.Contains("scene2")))
            {
                await LoadAssetBundle(file.FullName);
                Debug.LogWarning($"{file.FullName}加载完毕");
            }
            //Scene2AB = await LoadAssetBundle(Application.streamingAssetsPath + "/AssetBundles/scene2resource.gezi");
            //Debug.LogWarning("场景2资源加载完毕");
            //_ = LoadAssetBundle(Application.streamingAssetsPath + "/AssetBundles/scene2.gezi");
            //Debug.LogWarning("场景2加载完毕");

            Debug.LogWarning("资源加载完毕");

            async Task<AssetBundle> LoadAssetBundle(string path)
            {
                var ABLoadRequir = AssetBundle.LoadFromFileAsync(path);
                while (!ABLoadRequir.isDone)
                {
                    await Task.Delay(10);
                }
                return ABLoadRequir.assetBundle;
            }
        }
        /// <summary>
        /// 从指定AB包中加载素材，1是登陆场景，2是对战场景
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">加载路径</param>
        /// <param name="AssetBundlesID">1是登陆场景，2是对战场景</param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            foreach (var ab in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (ab.Contains(path))
                {
                    return ab.LoadAsset(path, typeof(T)) as T;
                }
            }
            return null;
        }
    }
}
