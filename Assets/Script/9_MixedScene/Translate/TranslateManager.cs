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
        static Dictionary<string, Dictionary<string, string>> translateDatas;
        /// <summary>
        /// 返回关键字的翻译或者效果说明翻译
        /// </summary>
        /// <param name="text"></param>
        /// <param name="IsGetIntroduction"></param>
        /// <returns></returns>
        public static string TranslationGameText(this string text, bool IsGetIntroduction = false)
        {
            if (translateDatas == null)
            {
                translateDatas = AssetBundleCommand.Load<TextAsset>("GameData", "Game-Text.json").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }
            if (translateDatas.ContainsKey(text))
            {
                var translationDict = translateDatas[text];
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
        public static List<(string stageName, string stageOpLeaderName, string stageOpIntroduction, string stageOpLeadId)> TranslationStageText(this string stageTag)
        {
            if (translateDatas == null)
            {
                translateDatas = AssetBundleCommand.Load<TextAsset>("GameData", "StageText.json").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }
            List<(string stageName, string stageOpLeaderName, string stageOpIntroduction, string stageOpLeadId)> currentSelectStageData = new();
            foreach (var translateData in translateDatas)
            {
                if (translateData.Key.Split('-')[0] == stageTag)
                {
                    var translationDict = translateDatas[stageTag];
                    //假如当前词语没有对应语言的翻译或者翻译为空则默认使用中文

                    string stageName = translationDict.ContainsKey($"Name-{currentLanguage}") ? translationDict[$"Name-{currentLanguage}"] : $"Name-Ch";
                    string stageOpLeaderName = translationDict.ContainsKey($"Name-{currentLanguage}") ? translationDict[$"Name-{currentLanguage}"] : $"Name-Ch";
                    string stageOpIntroduction = translationDict.ContainsKey($"Name-{currentLanguage}") ? translationDict[$"Name-{currentLanguage}"] : $"Name-Ch";
                    string stageOpLeadId = translationDict.ContainsKey($"Name-{currentLanguage}") ? translationDict[$"Name-{currentLanguage}"] : $"Name-Ch";
                    currentSelectStageData.Add((stageName, stageOpLeaderName, stageOpIntroduction, stageOpLeadId));
                }
            }

            return currentSelectStageData;
        }
        public static List<KeyWordModel> CheckKeyWord(string text)
        {
            if (translateDatas == null)
            {
                translateDatas = AssetBundleCommand.Load<TextAsset>("GameData", "Game-Text").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }

            List<KeyWordModel> keyWordInfos = new List<KeyWordModel>();
            translateDatas.Values.ForEach(pair =>
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