using System.Collections.Generic;
using System.IO;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Model;
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
                //假如当前词语没有对应翻译则默认使用中文
                if (!translationDict.ContainsKey(currentLanguage))
                {
                    language = "Ch";
                }
                return translationDict[language];
            }
            return "无法检索到Tag，请核对";
        }

        public static List<KeyWordModel> CheckKeyWord(string text)
        {
            if (translateData == null)
            {
                translateData = Resources.Load<TextAsset>("GameData/Game-Text").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }

            List<KeyWordModel> keyWordInfos = new List<KeyWordModel>();
            translateData.Values.ForEach(pair =>
            {
                string keyWord = pair[currentLanguage];
                int index = 0;
                while ((index = text.IndexOf(keyWord, index)) != -1)
                {
                    //获取该关键词的说明
                    string introduction = "";
                    if (translateData.ContainsKey(pair["Tag"] + "_Introduction"))
                    {
                        introduction = translateData[pair["Tag"] + "_Introduction"][currentLanguage];
                    }
                    //加入到关键词列表
                    keyWordInfos.Add(new KeyWordModel()
                    {
                        tag = pair["Tag"],
                        keyWord = keyWord,
                        startIndex = index,
                        endIndex = index + keyWord.Length,
                        introduction = introduction
                    }); 
                    index = index + keyWord.Length;
                }
            });
            return keyWordInfos;
        }

    }
}