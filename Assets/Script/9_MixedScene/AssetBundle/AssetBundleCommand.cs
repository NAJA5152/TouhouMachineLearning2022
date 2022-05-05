using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class AssetBundleCommand
    {
        static bool AlreadyInit { get; set; } = false;
        static AssetBundle SceneAB { get; set; }
        public static async void Init(Action completedAction)
        {
            if (AlreadyInit) { return; }
            var abr = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/gameresources.gezi");
            Debug.LogWarning("资源加载完毕");
            AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/AssetBundles/scene1.gezi").completed += x =>
             {
                 Debug.LogWarning("场景1加载完毕");
                 completedAction();
                 AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/AssetBundles/scene2.gezi").completed += x =>
                 {
                     Debug.LogWarning("场景2加载完毕");
                     //var abr = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/AssetBundles/gameresources.gezi");
                     //SceneAB = abr.assetBundle;
                     //abr.completed += x =>
                     //{
                     //    Debug.LogWarning("资源加载完毕");
                     //    Load();
                     //};
                 };
             };
            AlreadyInit = true;
        }
        public static async void Load()
        {
            var a = SceneAB.AllAssetNames();
        }
    }
}
