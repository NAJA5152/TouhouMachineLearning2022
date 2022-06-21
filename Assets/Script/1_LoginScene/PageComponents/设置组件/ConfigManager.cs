using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static string ConfigFileSavePath =>
#if UNITY_ANDROID //��׿
                Application.persistentDataPath;
#else
                Directory.GetCurrentDirectory();
#endif

        private static void SaveConfig() => File.WriteAllText(ConfigFileSavePath + "/GameConfig.ini", configInfo.ToJson());


        public static void InitConfig()
        {
            //�ж����ޱ��������ļ�
            //�����򴴽�Ĭ�������ļ�
            //������ؼ�ֵ���ڴ����ļ�ֵ

            if (!File.Exists(ConfigFileSavePath + "/GameConfig.ini"))
            {
                Debug.Log("����Ĭ�������ļ�");
                configInfo.Width = Screen.currentResolution.width;
                configInfo.Heigh = Screen.currentResolution.height;
                configInfo.IsFullScreen = Screen.fullScreen;
                configInfo.UseLanguage = TranslateManager.currentLanguage;
                Directory.CreateDirectory(ConfigFileSavePath);
                SaveConfig();
            }
            else
            {
                Debug.Log("�������������ļ�");
                configInfo = File.ReadAllText(ConfigFileSavePath + "/GameConfig.ini").ToObject<ConfigInfoModel>();
                Screen.SetResolution(192*6,108*6,false);
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
            Debug.Log(configInfo.Width + "_" + configInfo.Heigh + "_" + configInfo.IsFullScreen);
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            SaveConfig();
        }
        public void SetScreenMode(int index)
        {
            configInfo.IsFullScreen = (index == 0);
            configInfo.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            configInfo.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            Debug.Log(configInfo.Width + "_" + configInfo.Heigh + "_" + configInfo.IsFullScreen);
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            SaveConfig();
        }
        public void SetLanguage(int index)
        {
            configInfo.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = configInfo.UseLanguage;
            SaveConfig();
        }
        public void SetVolume(int index)
        {
            configInfo.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = configInfo.UseLanguage;
            SaveConfig();
        }
        public void SetH_Mode(int index)
        {
            configInfo.UseLanguage = LanguageText.text;
            TranslateManager.currentLanguage = configInfo.UseLanguage;
            SaveConfig();
        }
        public void SendCode()
        {

        }
    }
}