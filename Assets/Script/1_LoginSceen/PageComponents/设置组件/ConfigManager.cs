using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    public partial class ConfigManager : MonoBehaviour
    {
        static ConfigInfoModel configInfo = new ConfigInfoModel();
        //1 ȫ�� 2 �ޱ߿� 3����
        public FullScreenMode ScreenMode { get; set; }
        public float Volume { get; set; }
        public bool H_Mode { get; set; }
        private void SaveConfig() => File.WriteAllText("GameConfig.ini", configInfo.ToJson());

        public static void InitConfig()
        {
            //�ж����ޱ��������ļ�
            //�����򴴽�Ĭ�������ļ�
            //������ؼ�ֵ���ڴ����ļ�ֵ
            if (!File.Exists("GameConfig.ini"))
            {
                configInfo.Width = Screen.width;
                configInfo.Heigh = Screen.height;
                //configInfo.ScreenMode = Screen.fullScreenMode;
                configInfo.IsFullScreen = Screen.fullScreen;
                configInfo.UseLanguage = TranslateManager.currentLanguage;
                Debug.LogError(Directory.GetCurrentDirectory());
                SaveConfig();
            }
            else
            {
                configInfo = File.ReadAllText("GameConfig.ini").ToObject<ConfigInfoModel>();
                Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.ScreenMode, 60);
                TranslateManager.currentLanguage = configInfo.UseLanguage;
            }
        }
        public TextMeshProUGUI ResolutionText;
        public TextMeshProUGUI LanguageText;
        public TextMeshProUGUI CodeText;
        public void SetResolution(int index)
        {
            configInfo.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            configInfo.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            SaveConfig();
        }
        public void SetScreenMode(int index)
        {
            configInfo.IsFullScreen = (index == 1);
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            SaveConfig();
            //Screen.fullScreenMode = select switch
            //{
            //    1=> FullScreenMode.FullScreenWindow,
            //    2=> FullScreenMode.MaximizedWindow,
            //    3=> FullScreenMode.Windowed,
            //    4=> FullScreenMode.ExclusiveFullScreen,
            //    _ => throw new System.NotImplementedException(),
            //};
        }
        public void SetLanguage(int index)
        {
            configInfo.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = configInfo.UseLanguage;
            SaveConfig();
        }


        //�����ļ�ֵ���ڿؼ�ֵ
        public void Apply()
        {
            //configInfo.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            //configInfo.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            //configInfo.ScreenMode = ScreenMode;
            //configInfo.Volume = Volume;
            //configInfo.UseLanguage = LanguageText.text;
            //configInfo.H_Mode = H_Mode;
            //File.WriteAllText("GameConfig.ini", configInfo.ToJson());
            ////���ݲ�������Ӧ�ó���
            //Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            //TranslateManager.currentLanguage = configInfo.UseLanguage;
        }
        //�ؼ�ֵ���ڴ����ļ�ֵ
        public void Reback()
        {
            //configInfo = File.ReadAllText("GameConfig.ini").ToObject<ConfigInfoModel>();
            //ResolutionText.text = $"{configInfo.Width}*{configInfo.Heigh}";
            //LanguageText.text = configInfo.UseLanguage;
        }
        public void SendCode()
        {

        }
    }
}