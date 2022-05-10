using System.Collections.Generic;
using TouhouMachineLearningSummary.Command;
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
        /// <summary>
        /// 返回关键字的翻译或者效果说明翻译
        /// </summary>
        /// <param name="text"></param>
        /// <param name="IsGetIntroduction"></param>
        /// <returns></returns>
        public static string Translation(this string text, bool IsGetIntroduction = false)
        {
            if (translateData == null)
            {
                translateData = AssetBundleCommand.Load<TextAsset>("GameData", "Game-Text").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }
            if (translateData.ContainsKey(text))
            {
                var translationDict = translateData[text];
                string tag = currentLanguage + (IsGetIntroduction ? "_Introduction" : "");
                //假如当前词语没有对应语言的翻译或者翻译为空则默认使用中文
                if (!translationDict.ContainsKey(tag) || (translationDict[tag] == ""))
                {
                    tag = "Ch" + (IsGetIntroduction ? "_Introduction" : "");
                }
                return translationDict[tag];
            }
            return "无法检索到Tag，请核对";
        }

        public static List<KeyWordModel> CheckKeyWord(string text)
        {
            if (translateData == null)
            {
                translateData = AssetBundleCommand.Load<TextAsset>("GameData", "Game-Text").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }

            List<KeyWordModel> keyWordInfos = new List<KeyWordModel>();
            translateData.Values.ForEach(pair =>
            {
                string keyWord = pair[currentLanguage] == "" ? pair["Ch"] : pair[currentLanguage];
                int index = 0;
                while ((index = text.IndexOf(keyWord, index)) != -1)
                {
                    //获取该关键词的说明
                    string introduction = pair[currentLanguage + "_Introduction"] == "" ? pair["Ch_Introduction"] : pair[currentLanguage + "_Introduction"];

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