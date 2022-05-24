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
        static Dictionary<string, Dictionary<string, string>> gameTextTranslateDatas;
        static Dictionary<string, Dictionary<string, string>> stageTranslateDatas;
        /// <summary>
        /// 返回关键字的翻译或者效果说明翻译
        /// </summary>
        /// <param name="text"></param>
        /// <param name="IsGetIntroduction"></param>
        /// <returns></returns>
        public static string TranslationGameText(this string text, bool IsGetIntroduction = false)
        {
            if (gameTextTranslateDatas == null)
            {
                gameTextTranslateDatas = AssetBundleCommand.Load<TextAsset>("GameData", "Game-Text").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }
            if (gameTextTranslateDatas.ContainsKey(text))
            {
                var translationDict = gameTextTranslateDatas[text];
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
        //获取目标关卡的所有文字信息
        public static List<(string stageProcess, string stageName, string stageOpLeaderName, string stageOpIntroduction, string stageOpLeadId, string leaderNick)> TranslationStageText(this string stageTag)
        {
            if (stageTranslateDatas == null)
            {
                stageTranslateDatas = AssetBundleCommand.Load<TextAsset>("GameData", "Stage").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }
            List<(string stageProcess, string stageName, string stageOpLeaderName, string stageOpIntroduction, string stageOpLeadId, string leaderNick)> currentSelectStageData = new();
            foreach (var translateData in stageTranslateDatas)
            {
                if (translateData.Key.Split('-')[0] == stageTag)
                {
                    var translationDict = translateData.Value;
                    //假如当前词语没有对应语言的翻译或者翻译为空则默认使用中文
                    //获得关卡标签x-xx
                    string stageProcess = translateData.Key;

                    //获得对方卡画ID
                    string leadId = GetText(translationDict, "LeadId");
                    //获得对方名
                    string leaderName = GetText(translationDict, "LeaderName");
                    //获得对方称号
                    string leaderNick = GetText(translationDict, "LeaderNick");
                    //获得对方介绍
                    string leaderIntroduction = GetText(translationDict, "leaderIntroduction");
                    //获得关卡名
                    string stageName = GetText(translationDict, "StageName");
                    //获得关卡介绍
                    string stageIntroduction = GetText(translationDict, "StageIntroduction");
                  
                    currentSelectStageData.Add((stageProcess, stageName, leaderName, stageIntroduction, leadId,leaderNick));


                    string GetText( Dictionary<string,string> dict,string tag)=> dict.ContainsKey($"{tag}-{currentLanguage}") ? dict[$"{tag}-{currentLanguage}"] : $"{tag}-Ch";
                   
                }
            }

            return currentSelectStageData;
        }
        public static List<KeyWordModel> CheckKeyWord(string text)
        {
            if (gameTextTranslateDatas == null)
            {
                gameTextTranslateDatas = AssetBundleCommand.Load<TextAsset>("GameData", "Game-Text").text.ToObject<Dictionary<string, Dictionary<string, string>>>();
            }

            List<KeyWordModel> keyWordInfos = new List<KeyWordModel>();
            gameTextTranslateDatas.Values.ForEach(pair =>
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