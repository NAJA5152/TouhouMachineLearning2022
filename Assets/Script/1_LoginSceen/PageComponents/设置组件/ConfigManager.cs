using System.Collections;
using System.Collections.Generic;
using System.IO;
using TouhouMachineLearningSummary.Extension;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public class ConfigManager : MonoBehaviour
    {
        static ConfigInfoModel configInfo;
        class ConfigInfoModel
        {
            public int Width { get; set; }
            public int Heigh { get; set; }
            //1 全屏 2 无边框 3窗口
            public FullScreenMode ScreenMode { get; set; }
            public string UseLanguage { get; set; }
            public float Volume { get; set; }
            public bool H_Mode { get; set; }
        }
        public void InitConfig()
        {
            //判断有无本地配置文件
            //若无则创建默认配置文件
            if (File.Exists("GameConfig.ini"))
            {
                configInfo.Width = Screen.width;
                configInfo.Heigh = Screen.height;
                configInfo.ScreenMode = Screen.fullScreenMode;
                File.WriteAllText("GameConfig.ini", configInfo.ToJson());
            }
            else
            {
                configInfo = File.ReadAllText("GameConfig.ini").ToObject<ConfigInfoModel>();
                Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.ScreenMode,60);
            }
        }
        public void Apply()
        {
            File.WriteAllText("GameConfig.ini", configInfo.ToJson());
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.ScreenMode, 60);
        }
    }
}