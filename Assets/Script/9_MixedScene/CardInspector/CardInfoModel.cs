using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Command.CardLibrary;
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
        [LabelText("名字")]
        [ShowInInspector]
        [Sirenix.Serialization.OdinSerialize]
        public Dictionary<string, string> name = new Dictionary<string, string>();

        [ShowInInspector]
        [LabelText("卡片介绍")]
        public Dictionary<string, string> describe = new Dictionary<string, string>();
        [ShowInInspector]
        [LabelText("卡牌效果")]
        public Dictionary<string, string> ability = new Dictionary<string, string>();
        //public string translateName => name["Name-" + (name.ContainsKey("Name-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];
        public string translateName
        {
            get
            {
                string name = (this.name.ContainsKey("Name-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch");
                return this.name["Name-" + name];
            }
        }

        public string translateDescribe => describe["Describe-" + (describe.ContainsKey("Describe-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];
        public string translateAbility => ability["Ability-" + (ability.ContainsKey("Ability-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];
        public List<KeyValuePair<string, string>> a;
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
        [LabelText("卡片标签"), EnumToggleButtons]
        public string cardTag = "";
        [HideInInspector]
        public string imageUrl;
        public CardModel() { }
        /// <summary>
        /// 根据卡牌参数补充对应的卡牌id，卡牌插画等
        /// </summary>
        /// <param name="isSingle"></param>
        public CardModel Init(bool isSingle)
        {
            a = name.ToList();
            if (isSingle)
            {
                cardID = cardID + 10000;
            }
            else
            {
                cardID = cardID + 20000;
            }
            icon = Resources.Load<Texture2D>("CardTex\\" + imageUrl);
            return this;
        }
        [Button("打开脚本")]
        public void OpenCardScript()
        {
            string targetPath = Application.dataPath + $@"\Script\9_MixedScene\CardSpace\Card{cardID}.cs";
            CardLibraryCommand.CreatScript(cardID);
            System.Diagnostics.Process.Start(targetPath);
        }
        public Sprite GetCardSprite() =>icon.ToSprite();
    }
}
