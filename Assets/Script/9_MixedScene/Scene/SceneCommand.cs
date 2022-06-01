using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Manager;

namespace TouhouMachineLearningSummary.Command
{
    /// <summary>
    /// 用于不同场景初始化操作
    /// </summary>
    internal class SceneCommand
    {
        static bool IsInit { get; set; }
        public static async Task InitAsync(bool isHotFixedLoad )
        {
            //加载AB包(仅一次)
            await Command.AssetBundleCommand.Init(isHotFixedLoad);
            //初始化网络系统，用于获取指定版本卡牌信息
            await NetCommand.Init();
            //加载音效数据
            Command.SoundEffectCommand.Init();
            //加载卡牌和卡画数据
            await CardAssemblyManager.SetCurrentAssembly("");
            //加载本地卡画数据
#if UNITY_EDITOR
            InspectorCommand.Init();
#endif
            //加载状态与字段

        }
    }
}
