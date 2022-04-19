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
        static ConfigInfoModel configInfo;
       
        //1 ȫ�� 2 �ޱ߿� 3����
        public FullScreenMode ScreenMode { get; set; }
        public float Volume { get; set; }
        public bool H_Mode { get; set; }
        //
        public void InitConfig()
        {
            //�ж����ޱ��������ļ�
            //�����򴴽�Ĭ�������ļ�
            //������ؼ�ֵ���ڴ����ļ�ֵ
            if (File.Exists("GameConfig.ini"))
            {
                configInfo.Width = Screen.width;
                configInfo.Heigh = Screen.height;
                configInfo.ScreenMode = Screen.fullScreenMode;
                configInfo.UseLanguage = "Ch";
                File.WriteAllText("GameConfig.ini", configInfo.ToJson());
            }
            else
            {
                configInfo = File.ReadAllText("GameConfig.ini").ToObject<ConfigInfoModel>();
                Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.ScreenMode, 60);
            }
        }
        public TextMeshProUGUI ResolutionText;
        public TextMeshProUGUI LanguageText;
        public TextMeshProUGUI CodeText;

        //�����ļ�ֵ���ڿؼ�ֵ
        public void Apply()
        {
            configInfo.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            configInfo.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            configInfo.ScreenMode = ScreenMode;
            configInfo.Volume = Volume;
            configInfo.UseLanguage = LanguageText.text;
            configInfo.H_Mode = H_Mode;
            File.WriteAllText("GameConfig.ini", configInfo.ToJson());
            Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.ScreenMode, 60);
        }
        //�ؼ�ֵ���ڴ����ļ�ֵ
        public void Reback()
        {
            configInfo = File.ReadAllText("GameConfig.ini").ToObject<ConfigInfoModel>();
            ResolutionText.text = $"{configInfo.Width}*{configInfo.Heigh}";
            LanguageText.text = configInfo.UseLanguage;

        }
        public void SendCode()
        {

        }
    }
}