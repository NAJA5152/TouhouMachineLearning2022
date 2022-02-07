﻿using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    //控制悬右键卡牌时展现的详情面板
    public class CardAbilityBoardManager : MonoBehaviour
    {
        //需求功能
        //切换上下一张卡牌
        //显示当前卡牌的各种信息
        enum LoadType
        {
            FromLibrary,
            FromCardList,
            FromCard,
        }
        int currentRank { get; set; } = 0;
        Card CurrentGameCard { get; set; }
        LoadType CurrentLoadType { get; set; }
        public Image Icon => transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        public Text Name => transform.GetChild(0).GetChild(1).GetChild(2).GetChild(1).GetComponent<Text>();
        public Text Tag => transform.GetChild(0).GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>();
        public Text AbilityText => transform.GetChild(0).GetChild(1).GetChild(3).GetChild(1).GetComponent<Text>();
        public Text IntroductionText => transform.GetChild(0).GetChild(1).GetChild(4).GetChild(1).GetComponent<Text>();
        public RectTransform AbilityBackground => transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        public RectTransform IntroductionBackground => transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();

        public void Show() => transform.GetChild(0).gameObject.SetActive(true);
        public void Close() => transform.GetChild(0).gameObject.SetActive(false);
        
        public void LoadCardsIdsFromCardList(GameObject cardModel)
        {
            CurrentLoadType = LoadType.FromCardList;
            currentRank = Info.CardCompnentInfo.deckCardModels.IndexOf(cardModel);
            ChangeIntroduction(Info.CardCompnentInfo.distinctCardIds[currentRank]);
            Show();
        }
        public void LoadCardsIdsFromCardLibrary(GameObject cardModel)
        {
            CurrentLoadType = LoadType.FromLibrary;
            currentRank = Info.CardCompnentInfo.libraryCardModels.IndexOf(cardModel);
            ChangeIntroduction(Info.CardCompnentInfo.LibraryFilterCardList[currentRank].cardID);
            Show();
        }
        public void LoadCardFromGameCard(GameObject cardModel)
        {
            CurrentGameCard = cardModel.GetComponent<Card>();
            CurrentLoadType = LoadType.FromCard;
            currentRank = CurrentGameCard.belongCardList.IndexOf(CurrentGameCard);
            ChangeIntroduction(CurrentGameCard);
            Show();
        }
        public void LoadLastCardInfo()
        {
            switch (CurrentLoadType)
            {
                case LoadType.FromLibrary:
                    currentRank = Mathf.Max(0, currentRank-1);
                    ChangeIntroduction(Info.CardCompnentInfo.LibraryFilterCardList[currentRank].cardID);
                    break;
                case LoadType.FromCardList:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(Info.CardCompnentInfo.distinctCardIds[currentRank]);
                    break;
                case LoadType.FromCard:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(CurrentGameCard.belongCardList[currentRank]);
                    break;
                default:
                    break;
            }
        }
        public void LoadNextCardInfo()
        {
            switch (CurrentLoadType)
            {
                case LoadType.FromLibrary:
                    currentRank = Mathf.Min(Info.CardCompnentInfo.LibraryFilterCardList.Count-1, currentRank + 1);
                    ChangeIntroduction(Info.CardCompnentInfo.LibraryFilterCardList[currentRank].cardID);
                    break;
                case LoadType.FromCardList:
                    currentRank = Mathf.Min(Info.CardCompnentInfo.distinctCardIds.Count - 1, currentRank + 1);
                    ChangeIntroduction(Info.CardCompnentInfo.distinctCardIds[currentRank]);
                    break;
                case LoadType.FromCard:
                    currentRank = Mathf.Min(CurrentGameCard.belongCardList.Count - 1, currentRank + 1);
                    ChangeIntroduction(CurrentGameCard.belongCardList[currentRank]);
                    break;
                default:
                    break;
            }
        }
        public void ChangeIntroduction<T>(T target)
        {
            string ability = "";
            string Introduction = "";
            if (typeof(T) == typeof(int))
            {
                var cardInfo = CardAssemblyManager.GetLastCardInfo((int)(object)target);
                Icon.sprite = cardInfo.icon.ToSprite();
                Name.text = cardInfo.translateName;
                ability = cardInfo.translateAbility;
                //IntroductionBackground.gameObject.SetActive(false);
            }
            else
            {
                Card card = (Card)(object)target;
                Icon.sprite = card.Icon.ToSprite();
                Name.text = card.CardName;
                ability = card.CardIntroduction;
                card.cardFields.ToList().ForEach(field =>
                {
                    switch (field.Key)
                    {
                        case CardField.Timer:
                            break;
                        case CardField.Vitality:
                            Introduction += $"活力：增强两侧单位效果{field.Value}";
                            break;
                        case CardField.Apothanasia:
                            break;
                        default:
                            break;
                    }
                });
                //IntroductionBackground.gameObject.SetActive(true);
                //IntroductionBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
                IntroductionText.text = Introduction;
            }
            //AbilityBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
            //修改文本为富文本
            AbilityText.text = ability;
        }
    }
}