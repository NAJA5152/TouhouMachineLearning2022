using System.Linq;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Command
{
    class CardLibraryCommand
    {
        /// <summary>
        /// 初始化牌库列表
        /// </summary>
        public static void Init()
        {
            //牌库卡牌列表
            //var showCardList = CardAssemblyManager.GetLastMultiCardInfos;
            Info.CardCompnentInfo.LibraryFilterCardList = CardAssemblyManager.GetLastMultiCardInfos;
            //如果当前是编辑卡组模式，则只显示指定阵营
            if (Info.CardCompnentInfo.isEditDeckMode)
            {
                //showCardList = showCardList.Where(card => card.cardCamp == Info.CardCompnentInfo.targetCamp).ToList();
                //showCardList = showCardList.Where(card => card.cardRank == GameEnum.CardRank.Silver).ToList();
                Info.CardCompnentInfo.LibraryFilterCardList = Info.CardCompnentInfo.LibraryFilterCardList.Where(card => card.cardCamp == Info.CardCompnentInfo.targetCamp|| card.cardCamp ==  GameEnum.Camp.Neutral).ToList();
                //Info.CardCompnentInfo.LibraryFilterCardList = Info.CardCompnentInfo.LibraryFilterCardList.Where(card => card.cardRank == GameEnum.CardRank.Silver).ToList();
            }
            int libraryCardNumber = Info.CardCompnentInfo.LibraryFilterCardList.Count();
            //如果处于卡组编辑状态，则对卡牌列表做个筛选
            //已生成卡牌列表
            int libraryModelNumber = Info.CardCompnentInfo.libraryCardModels.Count;
            Info.CardCompnentInfo.libraryCardModels.ForEach(model => model.SetActive(false));
            if (libraryCardNumber > libraryModelNumber)
            {
                for (int i = 0; i < libraryCardNumber - libraryModelNumber; i++)
                {
                    var newCardModel = UnityEngine.Object.Instantiate(Info.CardCompnentInfo.cardLibraryCardModel, Info.CardCompnentInfo.cardLibraryContent.transform);
                    Info.CardCompnentInfo.libraryCardModels.Add(newCardModel);
                }
            }
            for (int i = 0; i < libraryCardNumber; i++)
            {
                //卡牌信息集合
                //var info = CardAssemblyManager.lastMultiCardInfos[i];
                var info = Info.CardCompnentInfo.LibraryFilterCardList[i];
                //卡牌对应场景模型
                var newCardModel = Info.CardCompnentInfo.libraryCardModels[i];
                //卡牌id
                string cardId = info.cardID.ToString();
                //该id的卡牌持有数量
                int cardNum = GetHasCardNum(cardId);

                newCardModel.name = cardId;
                newCardModel.transform.localScale = Info.CardCompnentInfo.cardLibraryCardModel.transform.localScale;
                Sprite cardTex = info.icon.ToSprite();
                newCardModel.GetComponent<Image>().sprite = cardTex;
                newCardModel.transform.GetChild(1).GetComponent<Text>().text = info.TranslateName;
                newCardModel.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "X" + (cardNum > 9 ? "9+" : cardNum + "");
                newCardModel.GetComponent<Image>().color = new Color(1, 1, 1, cardNum == 0 ? 0.2f : 1);
                newCardModel.SetActive(true);
            }
        }
        public static int GetHasCardNum(string cardId) => Info.CardCompnentInfo.IsAdmin ? 3 : Info.AgainstInfo.onlineUserInfo.CardLibrary.ContainsKey(cardId) ? Info.AgainstInfo.onlineUserInfo.CardLibrary[cardId] : 0;
    }
}