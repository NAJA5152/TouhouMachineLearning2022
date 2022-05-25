//using CardInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Info.CardInspector;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEditor;
using UnityEngine;
using static TouhouMachineLearningSummary.Info.CardInspector.CardLibraryInfo;
using static TouhouMachineLearningSummary.Info.CardInspector.CardLibraryInfo.LevelLibrary;
using static TouhouMachineLearningSummary.Info.CardInspector.CardLibraryInfo.LevelLibrary.SectarianCardLibrary;

namespace TouhouMachineLearningSummary.Command
{

    public static class CardInspectorCommand
    {
        public static CardLibraryInfo GetLibraryInfo() => Resources.Load<CardLibraryInfo>("SaveData");
        //初始化每个牌库的每个关卡所包含的卡牌
        public static async Task InitAsync()
        {
            if (!Application.isPlaying)
            {
                await AssetBundleCommand.Init(false);
            }
            CardLibraryInfo cardLibraryInfo = GetLibraryInfo();
            cardLibraryInfo.levelLibries = new List<LevelLibrary>();
            cardLibraryInfo.includeLevel.ForEach(level => cardLibraryInfo.levelLibries.Add(new LevelLibrary(cardLibraryInfo.singleModeCards, level)));
            foreach (var levelLibrart in cardLibraryInfo.levelLibries.Where(library => library.isSingleMode))
            {
                levelLibrart.sectarianCardLibraries = new List<SectarianCardLibrary>();
                foreach (var sectarian in levelLibrart.includeSectarian)
                {
                    levelLibrart.sectarianCardLibraries.Add(new SectarianCardLibrary(levelLibrart.cardModelInfos, sectarian));

                    foreach (var sectarianLibrary in levelLibrart.sectarianCardLibraries)
                    {
                        foreach (var rank in sectarianLibrary.includeRank)
                        {
                            sectarianLibrary.rankLibraries.Add(new RankLibrary(sectarianLibrary.cardModelInfos, rank));
                        }
                    }
                }
            }
            cardLibraryInfo.levelLibries.Add(new LevelLibrary(cardLibraryInfo.multiModeCards, "多人"));
            foreach (var levelLibrart in cardLibraryInfo.levelLibries.Where(library => !library.isSingleMode))
            {
                levelLibrart.sectarianCardLibraries = new List<SectarianCardLibrary>();
                foreach (var sectarian in levelLibrart.includeSectarian)
                {
                    levelLibrart.sectarianCardLibraries.Add(new SectarianCardLibrary(levelLibrart.cardModelInfos, sectarian));

                    foreach (var sectarianLibrary in levelLibrart.sectarianCardLibraries)
                    {
                        foreach (var rank in sectarianLibrary.includeRank)
                        {
                            sectarianLibrary.rankLibraries.Add(new RankLibrary(sectarianLibrary.cardModelInfos, rank));
                        }
                    }
                }
            }
        }
        public static async void LoadFromJson()
        {

            /////////////////////////////////////////////新版/////////////////////////////////////
            //加载单人模式卡牌信息
            string singleData = File.ReadAllText(@"Assets\GameResources\Scene1Resource\GameData\CardData-Single.json");
            GetLibraryInfo().singleModeCards.Clear();
            GetLibraryInfo().singleModeCards.AddRange(singleData.ToObject<List<CardModel>>().Select(card => card.Init(true)));
            //加载多人模式卡牌信息
            string multiData = File.ReadAllText(@"Assets\GameResources\Scene1Resource\GameData\CardData-Multi.json");
            GetLibraryInfo().multiModeCards.Clear();
            GetLibraryInfo().multiModeCards.AddRange(multiData.ToObject<List<CardModel>>().Select(card => card.Init(false)));
            await InitAsync();
            Refresh();
            GetLibraryInfo().singleModeCards.ForEach(card => CreatScript(card.cardID));
            GetLibraryInfo().multiModeCards.ForEach(card => CreatScript(card.cardID));
        }

        public static void Refresh()
        {
#if UNITY_EDITOR
            CardInspector.CardMenu.UpdateInspector();
#endif
        }
        //public static void SaveToCsv()
        //{


        //}
        public static void ClearCardData()
        {
            GetLibraryInfo().multiModeCards.Clear();
            GetLibraryInfo().singleModeCards.Clear();
            foreach (var cardLibrarie in GetLibraryInfo().levelLibries)
            {
                foreach (var sIngleSectarianLibrary in cardLibrarie.sectarianCardLibraries)
                {
                    sIngleSectarianLibrary.cardModelInfos.Clear();
                }
            }
#if UNITY_EDITOR
            CardInspector.CardMenu.UpdateInspector();
#endif
        }

        public static void CreatScript(int cardId)
        {
            string targetPath = Application.dataPath + $@"\Script\9_MixedScene\CardSpace\Card{cardId}.cs";

            if (!File.Exists(targetPath))
            {
                string OriginPath = Application.dataPath + @"\Script\9_MixedScene\CardSpace\Card0.cs";
                string cardName = "";
                var single = GetLibraryInfo().singleModeCards.FirstOrDefault(card => card.cardID == cardId);
                var multi = GetLibraryInfo().multiModeCards.FirstOrDefault(card => card.cardID == cardId);
                if (single != null)
                {
                    cardName = single.Name["Name-Ch"];
                }
                if (multi != null)
                {
                    cardName = multi.Name["Name-Ch"];
                }

                string ScriptText = File.ReadAllText(OriginPath, System.Text.Encoding.GetEncoding("GB2312")).Replace("Card0", "Card" + cardId).Replace("卡牌生成模板", cardName);
                File.Create(targetPath).Close();
                File.WriteAllText(targetPath, ScriptText, System.Text.Encoding.GetEncoding("GB2312"));
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }
    }
}