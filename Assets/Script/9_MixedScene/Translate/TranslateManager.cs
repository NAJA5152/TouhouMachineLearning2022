using System.Collections.Generic;
using System.IO;
using TouhouMachineLearningSummary.Extension;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{
    /// <summary>
    /// 翻译管理器
    /// </summary>
    static class TranslateManager
    {
        public static string currentLanguage = "Ch";
        static Dictionary<string, Dictionary<string, string>> translateData;
        public static string Translation(this string text)
        {
            if (translateData == null)
            {
                translateData = Resources.Load<TextAsset>("GameData/Game-Text").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }
            if (translateData.ContainsKey(text))
            {
                var translationDict = translateData[text];
                string language = currentLanguage;
                if (!translationDict.ContainsKey(currentLanguage))
                {
                    language = "Ch";
                }
                return translationDict[language];
            }
            return "无法检索到Tag，请核对";
        }
    }
}