﻿using System.Linq;
using TMPro;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class CardLibraryManager : MonoBehaviour
    {
        /// <summary>
        /// 初始化牌库列表
        /// </summary>
        public static void Init()
        {
            //牌库卡牌列表
            Info.PageCompnentInfo.LibraryFilterCardList = CardAssemblyManager.LastMultiCardInfos;
            //如果当前是编辑卡组模式，则只显示指定阵营
            if (Info.PageCompnentInfo.isEditDeckMode)
            {
                Info.PageCompnentInfo.LibraryFilterCardList = Info.PageCompnentInfo.LibraryFilterCardList.Where(card => card.cardCamp == Info.PageCompnentInfo.selectCamp || card.cardCamp == GameEnum.Camp.Neutral).ToList();
            }
            int libraryCardNumber = Info.PageCompnentInfo.LibraryFilterCardList.Count();
            //如果处于卡组编辑状态，则对卡牌列表做个筛选
            //已生成卡牌列表
            //清空所有已销毁卡牌
            Info.PageCompnentInfo.libraryCardModels = Info.PageCompnentInfo.libraryCardModels.Where(model => model != null).ToList();
            int libraryModelNumber = Info.PageCompnentInfo.libraryCardModels.Count;
            var s = Info.PageCompnentInfo.libraryCardModels;
            Info.PageCompnentInfo.libraryCardModels.ForEach(model => model.SetActive(false));
            if (libraryCardNumber > libraryModelNumber)
            {
                for (int i = 0; i < libraryCardNumber - libraryModelNumber; i++)
                {
                    var newCardModel = Instantiate(Info.PageCompnentInfo.cardLibraryCardModel, Info.PageCompnentInfo.cardLibraryContent.transform);
                    Info.PageCompnentInfo.libraryCardModels.Add(newCardModel);
                }
            }
            for (int i = 0; i < libraryCardNumber; i++)
            {
                //卡牌信息集合
                var info = Info.PageCompnentInfo.LibraryFilterCardList[i];
                //卡牌对应场景模型
                var newCardModel = Info.PageCompnentInfo.libraryCardModels[i];
                //卡牌id
                string cardId = info.cardID.ToString();
                //该id的卡牌持有数量
                int cardNum = GetHasCardNum(cardId);

                newCardModel.name = cardId;
                newCardModel.transform.localScale = Info.PageCompnentInfo.cardLibraryCardModel.transform.localScale;
                Sprite cardTex = info.icon.ToSprite();
                newCardModel.GetComponent<Image>().sprite = cardTex;
                newCardModel.transform.GetChild(1).GetComponent<Text>().text = info.TranslateName;
                newCardModel.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "X" + (cardNum > 9 ? "9+" : cardNum + "");
                newCardModel.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = info.point == 0 ? "" : info.point.ToString();
                newCardModel.GetComponent<Image>().color = new Color(1, 1, 1, cardNum == 0 ? 0.2f : 1);
                newCardModel.SetActive(true);
            }
        }
        public static int GetHasCardNum(string cardId) => Info.PageCompnentInfo.IsAdmin ? 3 : Info.AgainstInfo.onlineUserInfo.CardLibrary.ContainsKey(cardId) ? Info.AgainstInfo.onlineUserInfo.CardLibrary[cardId] : 0;
        public void FocusLibraryCardOnMenu(GameObject cardModel)
        {
            CardAbilityPopupManager.focusCardID = CardAssemblyManager.LastMultiCardInfos[Info.PageCompnentInfo.libraryCardModels.IndexOf(cardModel)].cardID;
            if (Command.MenuStateCommand.HasState(MenuState.CardLibrary))
            {
                Command.CardDetailCommand.ChangeFocusCard(CardAbilityPopupManager.focusCardID);
                //CardAbilityPopupManager.ChangeIntroduction(CardAbilityPopupManager.focusCardID);
            }
        }
        public void FocusDeckCardOnMenu(GameObject cardModel) => CardAbilityPopupManager.focusCardID = CardAssemblyManager.LastMultiCardInfos[Info.PageCompnentInfo.deckCardModels.IndexOf(cardModel)].cardID;
        public void LostFocusCardOnMenu() => CardAbilityPopupManager.focusCardID = 0;
    }
}