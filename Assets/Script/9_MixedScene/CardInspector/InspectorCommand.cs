//using CardInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info.CardInspector;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEditor;
using UnityEngine;
using static TouhouMachineLearningSummary.Info.CardInspector.InspectorInfo;
using static TouhouMachineLearningSummary.Info.CardInspector.InspectorInfo.LevelLibrary;
using static TouhouMachineLearningSummary.Info.CardInspector.InspectorInfo.LevelLibrary.SectarianCardLibrary;

namespace TouhouMachineLearningSummary.Command
{

    public static class InspectorCommand
    {
        public static async void LoadFromJson()
        {
            /////////////////////////////////////////////�°�/////////////////////////////////////
            //��ȡ�༭����Ϣ��Ϣ
            InspectorInfo cardLibraryInfo = InspectorInfo.Instance;
            //���ؿ����ļ�
            InspectorInfo.CardTexture = new DirectoryInfo(@"Assets\GameResources\Scene1Resource").GetFiles("*.png", SearchOption.AllDirectories).ToList();
            //������Ӫͼ��
            for (int i = 0; i < 5; i++)
            {
                InspectorInfo.SectarianIcons[(Camp)i] = new FileInfo(@$"Assets\GameResources\Scene1Resource\Icon\{(Camp)i}.png").ToTexture2D();
            }
            //����Ʒ��ͼ��
            for (int i = 0; i < 4; i++)
            {
                InspectorInfo.RankIcons[(CardRank)i] = new FileInfo(@$"Assets\GameResources\Scene1Resource\Icon\{(CardRank)i}.png").ToTexture2D();
            }
            //���ص���ģʽ������Ϣ
            string singleData = File.ReadAllText(@"Assets\GameResources\Scene1Resource\GameData\CardData-Single.json");
            cardLibraryInfo.singleModeCards.Clear();
            cardLibraryInfo.singleModeCards.AddRange(singleData.ToObject<List<CardModel>>().Select(card => card.Init(isSingle: true, isFromAssetBundle: false)));
            //���ض���ģʽ������Ϣ
            string multiData = File.ReadAllText(@"Assets\GameResources\Scene1Resource\GameData\CardData-Multi.json");
            cardLibraryInfo.multiModeCards.Clear();
            cardLibraryInfo.multiModeCards.AddRange(multiData.ToObject<List<CardModel>>().Select(card => card.Init(isSingle: false, isFromAssetBundle: false)));


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
            CardInspector.CardMenu.UpdateInspector();
            InspectorInfo.Instance.singleModeCards.ForEach(card => CreatScript(card.cardID));
            InspectorInfo.Instance.multiModeCards.ForEach(card => CreatScript(card.cardID));
        }
        public static void ClearCardData()
        {
            InspectorInfo.Instance.multiModeCards.Clear();
            InspectorInfo.Instance.singleModeCards.Clear();
            InspectorInfo.Instance.levelLibries = new ();
            //foreach (var cardLibrarie in InspectorInfo.Instance.levelLibries)
            //{
            //    cardLibrarie.cardModelInfos.Clear();
            //    //���ÿ������
            //    foreach (var sIngleSectarianLibrary in cardLibrarie.sectarianCardLibraries)
            //    {
            //        sIngleSectarianLibrary.cardModelInfos.Clear();
            //        foreach (var sIngleSectarianLibrary in sIngleSectarianLibrary)
            //        {
            //            sIngleSectarianLibrary.cardModelInfos.Clear();
            //        }
            //    }
            //}
            CardInspector.CardMenu.UpdateInspector();
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