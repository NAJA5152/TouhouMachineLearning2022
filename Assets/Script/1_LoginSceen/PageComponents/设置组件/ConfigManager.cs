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
        //1 全屏 2 无边框 3窗口
        public FullScreenMode ScreenMode { get; set; }
        public float Volume { get; set; }
        public bool H_Mode { get; set; }
        private static void SaveConfig() => File.WriteAllText(Application.streamingAssetsPath + "/GameConfig.ini", configInfo.ToJson());

        public static void InitConfig()
        {
            //判断有无本地配置文件
            //若无则创建默认配置文件
            //若有则控件值等于储存文件值
            if (!File.Exists(Application.streamingAssetsPath + "/GameConfig.ini"))
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
                configInfo = File.ReadAllText(Application.streamingAssetsPath + "/GameConfig.ini").ToObject<ConfigInfoModel>();
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
            configInfo.IsFullScreen = (index == 0);
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

        //储存文件值等于控件值
        public void Apply()
        {
            //configInfo.Width = int.Parse(ResolutionText.text.Split("*")[0]);
            //configInfo.Heigh = int.Parse(ResolutionText.text.Split("*")[1]);
            //configInfo.ScreenMode = ScreenMode;
            //configInfo.Volume = Volume;
            //configInfo.UseLanguage = LanguageText.text;
            //configInfo.H_Mode = H_Mode;
            //File.WriteAllText("GameConfig.ini", configInfo.ToJson());
            ////根据参数设置应用程序
            //Screen.SetResolution(configInfo.Width, configInfo.Heigh, configInfo.IsFullScreen);
            //TranslateManager.currentLanguage = configInfo.UseLanguage;
        }
        public void SendCode()
        {

        }
    }
    public class testward : MonoBehaviour
    {
        [SerializeField]
        public Dictionary<Color, int> colorMap = new Dictionary<Color, int>();
        public Texture2D texture;
        float[,] matrix;
        GameObject[,] models;
        public GameObject[] model;
        int w => texture.width;
        int h => texture.height;
        // Start is called before the first frame update
        void Start()
        {
            matrix = new float[w, h];
            models = new GameObject[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color color = texture.GetPixel(i, j);
                    var heigh = FakeHigh(color);
                    matrix[i, j] = heigh;
                    models[i, j] = Instantiate(model.OrderBy(x=>Random.Range(0,1f)).FirstOrDefault());
                    models[i, j].transform.position = new Vector3(i, j, heigh);
                    models[i, j].GetComponent<Renderer>().material.color = color;
                }
            }
        }
        public float FakeHigh(Color color)
        {
            return colorMap.Any() ? colorMap.ToList().Sum(map =>
            {
                var distance = Vector3.Distance(ToVector3(map.Key), ToVector3(color));
                float weight = (Mathf.Sqrt(3) - distance) / Mathf.Sqrt(3);
                return map.Value * weight;
            }) : 0;
        }
        public Vector3 ToVector3(Color color) => new Vector3(color.r, color.g, color.b);

    }
}