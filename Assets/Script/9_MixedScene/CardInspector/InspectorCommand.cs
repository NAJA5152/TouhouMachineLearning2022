using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Other;
using UnityEditor;
using UnityEngine;
using static TouhouMachineLearningSummary.Info.InspectorInfo;
using static TouhouMachineLearningSummary.Info.InspectorInfo.LevelLibrary;
using static TouhouMachineLearningSummary.Info.InspectorInfo.LevelLibrary.SectarianCardLibrary;

namespace TouhouMachineLearningSummary.Command
{

    public static class InspectorCommand
    {
        static bool IsAlreadyInitialized = false;

        public static void Init()
        {
            if (!IsAlreadyInitialized)
            {
                //���ؿ����ļ�
                InspectorInfo.CardTexture = new DirectoryInfo(@"Assets\GameResources\CardTex").GetFiles("*.png", SearchOption.AllDirectories).ToList();
                //������Ӫͼ��
                for (int i = 0; i < 5; i++)
                {
                    InspectorInfo.SectarianIcons[(Camp)i] = new FileInfo(@$"Assets\GameResources\Icon\{(Camp)i}.png").ToTexture2D();
                }
                //����Ʒ��ͼ��
                for (int i = 0; i < 4; i++)
                {
                    InspectorInfo.RankIcons[(CardRank)i] = new FileInfo(@$"Assets\GameResources\Icon\{(CardRank)i}.png").ToTexture2D();
                }
                IsAlreadyInitialized = true;
                InspectorCommand.LoadFromJson();
            }
        }

        public static void LoadFromJson()
        {
            //��ȡ�༭����Ϣ��Ϣ
            InspectorInfo cardLibraryInfo = InspectorInfo.Instance;

            //���ص���ģʽ������Ϣ
            string singleData = File.ReadAllText(@"Assets\GameResources\GameData\CardData-Single.json");
            cardLibraryInfo.singleModeCards.Clear();
            cardLibraryInfo.singleModeCards.AddRange(singleData.ToObject<List<CardModel>>().Select(card => card.Init(isSingle: true, isFromAssetBundle: !Application.isEditor)));
            //���ض���ģʽ������Ϣ
            string multiData = File.ReadAllText(@"Assets\GameResources\GameData\CardData-Multi.json");
            cardLibraryInfo.multiModeCards.Clear();
            cardLibraryInfo.multiModeCards.AddRange(multiData.ToObject<List<CardModel>>().Select(card => card.Init(isSingle: false, isFromAssetBundle: !Application.isEditor)));


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
            cardLibraryInfo.levelLibries.Add(new LevelLibrary(cardLibraryInfo.multiModeCards, "����"));
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
#if UNITY_EDITOR
            CardMenu.UpdateInspector();
#endif
            cardLibraryInfo.singleModeCards.ForEach(card => CreatScript(card.cardID));
            cardLibraryInfo.multiModeCards.ForEach(card => CreatScript(card.cardID));
        }
        public static void ClearCardData()
        {
            InspectorInfo.Instance.multiModeCards.Clear();
            InspectorInfo.Instance.singleModeCards.Clear();
            InspectorInfo.Instance.levelLibries = new();
#if UNITY_EDITOR
            CardMenu.UpdateInspector();
#endif
        }

        public static void CreatScript(int cardId)
        {
            string targetPath = Application.dataPath + $@"\Script\9_MixedScene\CardSpace\Card{cardId}.cs";

            if (!File.Exists(targetPath))
            {
                string OriginPath = Application.dataPath + @"\Script\9_MixedScene\CardSpace\Card0.cs";
                string cardName = "";
                var single = InspectorInfo.Instance.singleModeCards.FirstOrDefault(card => card.cardID == cardId);
                var multi = InspectorInfo.Instance.multiModeCards.FirstOrDefault(card => card.cardID == cardId);
                if (single != null)
                {
                    cardName = single.Name["Name-Ch"];
                }
                if (multi != null)
                {
                    cardName = multi.Name["Name-Ch"];
                }

                string ScriptText = File.ReadAllText(OriginPath, System.Text.Encoding.GetEncoding("GB2312")).Replace("Card0", "Card" + cardId).Replace("��������ģ��", cardName);
                File.Create(targetPath).Close();
                File.WriteAllText(targetPath, ScriptText, System.Text.Encoding.GetEncoding("GB2312"));
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }
    }
}