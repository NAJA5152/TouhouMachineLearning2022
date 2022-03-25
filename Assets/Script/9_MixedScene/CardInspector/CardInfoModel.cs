﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Model
{
    [Serializable]
    public class CardModel
    {
        [HorizontalGroup("Split", 55, LabelWidth = 70)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Right)]
        //[LabelText("卡片贴图")]
        public Texture2D icon;
        [VerticalGroup("Split/Meta")]
        [LabelText("ID")]
        public int cardID;
        [VerticalGroup("Split/Meta")]
        [LabelText("所属关卡")]
        public string level;
        [VerticalGroup("Split/Meta")]
        [LabelText("所属系列")]
        public string series;
        public Dictionary<string, string> Name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CardTags { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Describe { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Ability { get; set; } = new Dictionary<string, string>();

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("名字")]
        public string TranslateName => Name["Name-" + (Name.ContainsKey("Name-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("标签")]
        public string TranslateTags => CardTags[(CardTags.ContainsKey(TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("介绍")]
        public string TranslateDescribe => Describe["Describe-" + (Describe.ContainsKey("Describe-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("效果")]
        public string TranslateAbility => Ability["Ability-" + (Ability.ContainsKey("Ability-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];

        [VerticalGroup("Split/Meta")]
        [LabelText("点数")]
        public int point;
        [VerticalGroup("Split/Meta")]
        [LabelText("卡片类型"), EnumToggleButtons]
        public CardType cardType;
        [VerticalGroup("Split/Meta")]
        [LabelText("卡片等级"), EnumToggleButtons]
        public CardRank cardRank;
        [VerticalGroup("Split/Meta")]
        [LabelText("所属势力"), EnumToggleButtons]
        public Camp cardCamp;
        [VerticalGroup("Split/Meta")]
        [LabelText("部署区域"), EnumToggleButtons]
        public BattleRegion cardDeployRegion = BattleRegion.All;
        [VerticalGroup("Split/Meta")]
        [LabelText("部署所属"), EnumToggleButtons]
        public Territory cardDeployTerritory = Territory.My;


        public Sprite GetCardSprite() => icon.ToSprite();
        public CardModel() { }
        /// <summary>
        /// 根据卡牌参数补充对应的卡牌id，卡牌插画等
        /// </summary>
        /// <param name="isSingle"></param>
        public CardModel Init(bool isSingle)
        {
            if (isSingle)
            {
                cardID = int.Parse($"1{series.PadLeft(2, '0')}{(int)cardRank}{cardID.ToString().PadLeft(3, '0')}");
                icon = Resources.Load<Texture2D>("CardTex\\Single\\" + cardID) ?? Resources.Load<Texture2D>("CardTex\\default");
            }
            else
            {
                cardID = int.Parse($"2{series.PadLeft(2, '0')}{(int)cardRank}{cardID.ToString().PadLeft(3, '0')}");
                icon = Resources.Load<Texture2D>("CardTex\\Multiplayer\\" + cardID) ?? Resources.Load<Texture2D>("CardTex\\default");
            }
            //Debug.Log(cardID);
            return this;
        }
        [Button("打开脚本")]
        public void OpenCardScript()
        {
            string targetPath = Application.dataPath + $@"\Script\9_MixedScene\CardSpace\Card{cardID}.cs";
            CardInspectorCommand.CreatScript(cardID);
            System.Diagnostics.Process.Start(targetPath);
        }
    }
}
