using System.Collections.Generic;
using System.IO;
using TouhouMachineLearningSummary.Extension;
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
                translateData = File.ReadAllText("Assets\\Resources\\CardData\\Game-Text.json").ToObject<Dictionary<string, Dictionary<string, string>>>();

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
            return "无法检索到改key，请核对";
        }
        //static string[] tagCsvData;
        //static string[] UITextgCsvData;
        //public static void Load()
        //{
        //    //tagCsvData = Resources.Load<TextAsset>("CardData/Tag").text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //    //UITextgCsvData = Resources.Load<TextAsset>("CardData/UiText").text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //}
        /// <summary>
        /// 根据中文调取对应语言的tag
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        
        /// <summary>
        /// 根据枚举体调取对应语言的tag
        /// </summary>
        /// <param name="cardTag"></param>
        /// <returns></returns>
        //public static string TransTag(this GameEnum.CardTag cardTag) => GetCsvData(tagCsvData, cardTag.ToString(), "En");
        //public static string TransUiText(this string text) => GetCsvData(UITextgCsvData, text);
        //private static string GetCsvData(string[] CsvData, string text, string defaultLanguage = "Ch")
        //{
        //    //默认中文列位置
        //    int defaultRank = CsvData[0].Split(',').ToList().IndexOf(defaultLanguage);
        //    //目标语言列位置
        //    int columnRank = CsvData[0].Split(',').ToList().IndexOf(currentLanguage);
        //    //目标语言行位置
        //    int rowRank = CsvData.ToList().IndexOf(CsvData.First(data => data.Split(',')[defaultRank] == text));
        //    string translateText = CsvData[rowRank].Split(',')[columnRank];
        //    return translateText == "" ? text : translateText;
        //}
    }
}