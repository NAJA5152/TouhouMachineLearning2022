using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace xls检测更新
{
    class Program
    {
        static DateTime lastChangeTime;
        static FileStream fs;
        static string direPath;
        static string filePath;
        static Dictionary<string, Dictionary<string, string>> textTranslate = new Dictionary<string, Dictionary<string, string>>();
        static List<string> supportLanguage = new List<string>();
       static bool a = true;
       static bool b = true;
       static bool c = true;
       static bool r1 => !(a && b && c);
       static bool r2 => (!a || !b || !c);
        static void Main(string[] args)
        {
            Console.WriteLine(r1);
            Console.WriteLine(r2);
            a = false;
            Console.WriteLine(r1);
            Console.WriteLine(r2);
            b = false;
            Console.WriteLine(r1);
            Console.WriteLine(r2);

            List<string> list = new List<string>() { null};
            Workbook workbook = new Workbook();
            direPath = Directory.GetCurrentDirectory().Replace(@"\OtherSolution\xls检测更新\bin\Debug\net6.0", "") + @"\Assets\Resources\GameData\";
            Console.WriteLine(Directory.GetCurrentDirectory());

            Console.WriteLine(direPath);

            filePath = direPath + "GameData.xlsx";
            Console.WriteLine(filePath);
            Task.Run(async () =>
            {
                while (true)
                {
                    if (lastChangeTime != new FileInfo(filePath).LastWriteTime)
                    {
                        fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        workbook.LoadFromStream(fs);
                        lastChangeTime = new FileInfo(filePath).LastWriteTime;
                        XlsToJson(workbook);
                        fs.Dispose();
                    }
                    await Task.Delay(100);
                }
            });
            while (true)
            {
                Console.ReadLine();
                fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                workbook.LoadFromStream(fs);
                XlsToJson(workbook);
                fs.Dispose();
            }
        }

        private static void XlsToJson(Workbook workbook)
        {
            //加载和储存游戏内文本的各种翻译
            var gameText = workbook.Worksheets["Game-Text"];
            int textColCount = gameText.Columns.Length;
            int textRowCount = gameText.Rows.Length;
            textTranslate = new Dictionary<string, Dictionary<string, string>>();
            supportLanguage.Clear();
            for (int rank = 2; rank <= textColCount; rank++)
            {
                supportLanguage.Add(gameText[1, rank].DisplayedText);
            }
            for (int i = 2; i <= textRowCount; i++)
            {
                Dictionary<string, string> singleTextTranslate = new Dictionary<string, string>();
                for (int j = 1; j <= textColCount; j++)
                {
                    singleTextTranslate[gameText[1, j].DisplayedText] = gameText[i, j].DisplayedText;
                }
                textTranslate[gameText[i, 1].DisplayedText] = singleTextTranslate;
            }
            Console.WriteLine(JsonConvert.SerializeObject(textTranslate, Formatting.Indented));
            File.WriteAllText(direPath + @"\Game-Text.json", JsonConvert.SerializeObject(textTranslate, Formatting.Indented));
            Console.WriteLine("///////////////");
            //加载和储存单人模式表格
            var singleCards = workbook.Worksheets["CardData-Single"];
            int singleColCount = singleCards.Columns.Length;
            int singleRowCount = singleCards.Rows.Length;
            var singleRange = singleCards.Range;
            List<CardModelInfo> singleCardList = new List<CardModelInfo>();
            for (int i = 2; i <= singleRowCount; i++)
            {
                singleCardList.Add(new CardModelInfo(singleCards, i));
            }
            File.WriteAllText(direPath + @"\CardData-Single.json", JsonConvert.SerializeObject(singleCardList.Where(cardInfo => cardInfo.isFinish).ToList(), Formatting.Indented));
            //加载和储存多人模式表格
            var multiCards = workbook.Worksheets["CardData-Multi"];
            int multiColCount = multiCards.Columns.Length;
            int multiRowCount = multiCards.Rows.Length;
            var multiRange = multiCards.Range;
            List<CardModelInfo> MultiCardList = new List<CardModelInfo>();
            for (int i = 2; i <= multiRowCount; i++)
            {
                MultiCardList.Add(new CardModelInfo(multiCards, i));
            }
            Console.WriteLine(JsonConvert.SerializeObject(MultiCardList, Formatting.Indented));

            File.WriteAllText(direPath + @"\CardData-Multi.json", JsonConvert.SerializeObject(MultiCardList.Where(cardInfo => cardInfo.isFinish).ToList(), Formatting.Indented));

            //加载和储存游戏对话文本系统和各种翻译
            DialogModel currentDialogModel = new DialogModel();
            List<DialogModel> dialogModels = new List<DialogModel>();
            var storyText = workbook.Worksheets["Story"];
            int storyColCount = storyText.Columns.Length;
            int storyRowCount = storyText.Rows.Length;
            for (int i = 2; i <= storyRowCount; i++)
            {
                string tag = storyText[i, 1].DisplayedText;
                string branch = storyText[i, 2].DisplayedText;
                string chara = storyText[i, 3].DisplayedText;
                string position = storyText[i, 4].DisplayedText;
                string face = storyText[i, 5].DisplayedText;
                Dictionary<string, string> text = new Dictionary<string, string>();
                if (tag != "")
                {
                    currentDialogModel.Tag = tag;
                }
                for (int j = 6; j < storyColCount; j++)
                {
                    text[storyText[1, j].DisplayedText] = storyText[i, j].DisplayedText;
                }
                if (chara != "")
                {
                    currentDialogModel.Operations.Add(new DialogModel.Operation(branch, chara, position, face, text));
                }
                else
                {
                    if (currentDialogModel.Operations.Any())
                    {
                        dialogModels.Add(currentDialogModel);
                        currentDialogModel = new DialogModel();
                    }
                }
            }
            //Console.WriteLine(JsonConvert.SerializeObject(dialogModels, Formatting.Indented));
            File.WriteAllText(direPath + @"\Story.json", JsonConvert.SerializeObject(dialogModels, Formatting.Indented));
            Console.WriteLine("///////////////");
        }
        class DialogModel
        {
            public string Tag { get; set; }
            public List<Operation> Operations { get; set; } = new List<Operation>();
            public class Operation
            {

                public Operation(string branch, string chara, string position, string face, Dictionary<string, string> text)
                {
                    Branch = branch;
                    Chara = chara;
                    Position = position;
                    Face = face;
                    Text = text;
                }
                public string Branch { get; set; }//分支
                public string Chara { get; set; }//角色
                public string Position { get; set; }//立绘位置
                public string Face { get; set; }//面部表情
                public Dictionary<string, string> Text { get; set; }//不同语言的文字
            }
        }
        class CardModelInfo
        {
            public int cardID;
            public string level;//单人卡牌用关卡区分
            public string series;//多人卡牌用系列区分
            public int point;
            public Dictionary<string, string> Name = new Dictionary<string, string>();
            public Dictionary<string, string> CardTags = new Dictionary<string, string>();
            public Dictionary<string, string> Ability = new Dictionary<string, string>();
            public Dictionary<string, string> Describe = new Dictionary<string, string>();
            public int cardType;
            public int cardCamp;
            public int cardRank;
            public int cardDeployRegion;
            public int cardDeployTerritory;
            public int ramification;
            public bool isFinish;

            public CardModelInfo(Worksheet cards, int i)
            {

                cardID = cards[i, cards.GetIndex("Id")].GetXlsData<int>();
                level = cards.GetIndex("Level") != 0 ? cards[i, cards.GetIndex("Level")].GetXlsData<string>() : "多人";
                series = cards.GetIndex("Series") != 0 ? cards[i, cards.GetIndex("Series")].GetXlsData<string>() : "";
                point = cards[i, cards.GetIndex("Point")].GetXlsData<int>();
                isFinish = cards[i, cards.GetIndex("Finish")].GetXlsData<int>() == 1;
                cardType = cards[i, cards.GetIndex("Type")].GetXlsData<string>().ToEnumIndex("单位", "特殊");
                cardCamp = cards[i, cards.GetIndex("Camp")].GetXlsData<string>().ToEnumIndex("中立", "道教", "神道教", "佛教", "科学");
                cardRank = cards[i, cards.GetIndex("Rank")].GetXlsData<string>().ToEnumIndex("领袖", "金", "银", "铜"); ;
                cardDeployRegion = cards[i, cards.GetIndex("Region")].GetXlsData<string>().ToEnumIndex("水", "火", "风", "土", "任意");
                if (cardDeployRegion == 4)
                {
                    cardDeployRegion = 99;
                }
                cardDeployTerritory = cards[i, cards.GetIndex("Territory")].GetXlsData<string>().ToEnumIndex("我方", "敌方");
                ramification = cards[i, cards.GetIndex("Ramification")].GetXlsData<string>().ToEnumIndex("正体", "衍生");
                foreach (var index in cards.GetIndexs("Name"))
                {
                    string key = cards[1, index].DisplayedText;
                    string value = cards[i, index].DisplayedText;
                    Name[key] = value;
                }
                //根据当前语言填充各种翻译的标签

                var ChineseTags = cards[i, cards.GetIndex("Tag")].GetXlsData<string>()?.Split(" ").ToList();
                if (ChineseTags != null)
                {
                    for (int rank = 0; rank < ChineseTags.Count; rank++)
                    {
                        var targetTagTranslate = textTranslate.ToList().FirstOrDefault(x => x.Value["Ch"] == ChineseTags[rank]).Value;
                        if (targetTagTranslate != null)
                        {
                            supportLanguage.ForEach(language =>
                            {
                                if (!CardTags.ContainsKey(language))
                                {
                                    CardTags[language] = "";
                                }
                                CardTags[language] = CardTags[language] + targetTagTranslate[language] + (rank == ChineseTags.Count - 1 ? "" : " ");
                            });
                        }
                    }
                }
                else
                {
                    //Console.WriteLine("无法查找到标签"+i+"-" + cards[i, cards.GetIndex("Tag")].DisplayedText);
                }


                foreach (var index in cards.GetIndexs("Ability"))
                {
                    string key = cards[1, index].DisplayedText;
                    string value = cards[i, index].DisplayedText;
                    Ability[key] = value;
                }
                foreach (var index in cards.GetIndexs("Describe"))
                {
                    string key = cards[1, index].DisplayedText;
                    string value = cards[i, index].DisplayedText;
                    Describe[key] = value;
                }
            }
        }
    }

    static class Extesion
    {
        public static int GetIndex(this Worksheet worksheet, string tag)
        {
            for (int i = 1; i <= worksheet.Columns.Length; i++)
            {
                if (worksheet[1, i].DisplayedText == tag)
                {
                    return i;
                }
            }
            return 0;
        }
        public static List<int> GetIndexs(this Worksheet worksheet, string tag)
        {
            List<int> indexs = new List<int>();
            for (int i = 1; i <= worksheet.Columns.Length; i++)
            {
                if (worksheet[1, i].DisplayedText.Contains(tag))
                {
                    indexs.Add(i);
                }
            }
            return indexs;
        }
        public static T GetXlsData<T>(this CellRange ranges)
        {
            if (ranges.DisplayedText != "")
            {
                return (T)Convert.ChangeType(ranges.Value, typeof(T).IsEnum ? typeof(int) : typeof(T));
            }
            else
            {
                return default;
            }
        }
        public static int ToEnumIndex(this string data, params string[] text)
        {
            return text.Contains(data) ? text.ToList().IndexOf(data) : 0;
        }
    }
}


