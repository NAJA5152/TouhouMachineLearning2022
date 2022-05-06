using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Command
{
    public class CardListCommand
    {

        //初始化牌组列表组件
        public static void Init(bool isInitOptions = true, Model.CardDeck newTempDeck = null, bool canChangeCard = false)
        {
            Log.Show("配置面板");
            //canChangeCard = Command.MenuStateCommand.GetCurrentState() == MenuState.CardListChange;
            Info.CardCompnentInfo.isEditDeckMode = canChangeCard;
            Info.CardCompnentInfo.okButton.SetActive(canChangeCard);
            Info.CardCompnentInfo.cancelButton.SetActive(canChangeCard);
            Info.CardCompnentInfo.changeButton.SetActive(!canChangeCard);
            //根据当前玩家牌组生成临时牌组
            //Info.CardCompnentInfo.tempDeck = newTempDeck ?? Info.AgainstInfo.UserInfo.UseDeck.Clone();
            //Debug.Log(Info.CardCompnentInfo.tempDeck.ToJson());
            if (Command.MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
            {
                Info.CardCompnentInfo.cardDeckNameModel.GetComponent<Text>().text = Info.CardCompnentInfo.tempDeck.DeckName;
            }
            else
            {
                Info.CardCompnentInfo.cardDeckNameModel.GetComponent<Text>().text = Info.AgainstInfo.onlineUserInfo.UseDeck.DeckName;
            }
            Log.Show("配置卡组名");
            if (isInitOptions)
            {
                //初始化领袖栏?可以舍去？
                Info.CardCompnentInfo.deckCardModels.ForEach(model =>
                {
                    if (model != null)
                    {
                        Object.Destroy(model);
                    }
                });
                Info.CardCompnentInfo.deckCardModels.Clear();
            }
            var cardTexture = Manager.CardAssemblyManager.GetLastCardInfo(Info.CardCompnentInfo.tempDeck.LeaderId).icon;
            Info.CardCompnentInfo.cardLeaderImageModel.GetComponent<Image>().material.mainTexture = cardTexture;
            Log.Show("配置领袖");
            int deskCardNumber = Info.CardCompnentInfo.distinctCardIds.Count();
            int deskModelNumber = Info.CardCompnentInfo.deckCardModels.Count;
            Info.CardCompnentInfo.deckCardModels.ForEach(model => model.SetActive(false));
            if (deskCardNumber > deskModelNumber)
            {
                for (int i = 0; i < deskCardNumber - deskModelNumber; i++)
                {
                    var newCardModel = Object.Instantiate(Info.CardCompnentInfo.cardDeckCardModel, Info.CardCompnentInfo.cardDeckContent.transform);
                    Info.CardCompnentInfo.deckCardModels.Add(newCardModel);
                }
            }
            Log.Show("新增牌组栏");

            //初始化卡牌栏
            for (int i = 0; i < Info.CardCompnentInfo.distinctCardIds.Count(); i++)
            {
                int cardID = Info.CardCompnentInfo.distinctCardIds[i];
                GameObject currentCardModel = Info.CardCompnentInfo.deckCardModels[i];

                var info = CardAssemblyManager.lastMultiCardInfos.FirstOrDefault(cardInfo => cardInfo.cardID == cardID);
                if (info != null)
                {
                    currentCardModel.transform.GetChild(0).GetComponent<Text>().text = info.TranslateName;
                    Sprite cardTex = info.icon.ToSprite();
                    currentCardModel.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = cardTex;
                    //设置数量
                    Color RankColor = Color.white;
                    switch (info.cardRank)
                    {
                        case GameEnum.CardRank.Leader: RankColor = new Color(0.98f, 0.9f, 0.2f); break;
                        case GameEnum.CardRank.Gold: RankColor = new Color(0.98f, 0.9f, 0.2f); break;
                        case GameEnum.CardRank.Silver: RankColor = new Color(0.75f, 0.75f, 0.75f); break;
                        case GameEnum.CardRank.Copper: RankColor = new Color(0.55f, 0.3f, 0.1f); break;
                    }
                    //品质
                    currentCardModel.transform.GetChild(2).GetComponent<Image>().color = RankColor;
                    //点数
                    int point = Manager.CardAssemblyManager.GetLastCardInfo(cardID).point;
                    currentCardModel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = point == 0 ? " " : point + "";
                    //数量
                    currentCardModel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "x" + Info.CardCompnentInfo.tempDeck.CardIds.Count(id => id == cardID);

                    currentCardModel.SetActive(true);
                }
                else
                {
                    Debug.Log(cardID + "查找失败");
                }
            }
            Log.Show("配置牌组");

        }
        public static async void SaveDeck()
        {
            Debug.Log("保存卡组");
            Info.AgainstInfo.onlineUserInfo.UseDeck = Info.CardCompnentInfo.tempDeck;
            await Info.AgainstInfo.onlineUserInfo.UpdateDecksAsync();
            Command.CardListCommand.Init();
            Command.MenuStateCommand.RebackStare();
        }
        public static void CancelDeck()
        {
            Debug.Log("取消卡组修改");
            Info.CardCompnentInfo.tempDeck = Info.AgainstInfo.onlineUserInfo.UseDeck;
            Command.CardListCommand.Init();
            Command.MenuStateCommand.RebackStare();
        }
        public static void RenameDeck()
        {
            if (Command.MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
            {
                _ = NoticeCommand.ShowAsync("重命名卡牌", NotifyBoardMode.Input, inputAction: async (text) =>
                {
                    Debug.Log("重命名卡组为" + text);
                    Info.CardCompnentInfo.tempDeck.DeckName = text;
                    Command.CardListCommand.Init();
                    await Task.Delay(100);
                }, inputField: Info.AgainstInfo.onlineUserInfo.UseDeck.DeckName);
            }
        }

        public static void FocusDeckCardOnMenu(GameObject cardModel)
        {
            int focusCardRank = Info.CardCompnentInfo.deckCardModels.IndexOf(cardModel);
            int cardID = Info.CardCompnentInfo.distinctCardIds[focusCardRank];
            CardAbilityPopupManager.focusCardID = cardID;
        }
        public static void LostFocusCardOnMenu()
        {
            CardAbilityPopupManager.focusCardID = 0;
        }
        public static void AddCardFromLibrary(GameObject clickCard)
        {
            if (Info.CardCompnentInfo.isEditDeckMode)
            {
                Debug.Log("添加卡牌" + clickCard.name);
                int clickCardId = int.Parse(clickCard.name);
                int usedCardIdsNum = Info.CardCompnentInfo.tempDeck.CardIds.Count(id => id == clickCardId);
                var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo(clickCardId);
                if (cardInfo.cardRank == CardRank.Leader)
                {
                    Info.CardCompnentInfo.tempDeck.LeaderId = cardInfo.cardID;
                    _ = Command.SoundEffectCommand.PlayAsync(UISoundEffectType.ChangeSuccess);
                }
                else
                {
                    int allowAddNum = cardInfo.cardRank == GameEnum.CardRank.Copper ? 3 : 1;
                    if (usedCardIdsNum < Mathf.Min(allowAddNum, Command.CardLibraryCommand.GetHasCardNum(clickCard.name)))
                    {
                        Info.CardCompnentInfo.tempDeck.CardIds.Add(clickCardId);
                        Command.CardListCommand.Init(newTempDeck: Info.CardCompnentInfo.tempDeck, canChangeCard: true);
                        _ = Command.SoundEffectCommand.PlayAsync(UISoundEffectType.ChangeSuccess);
                    }
                    else
                    {
                        _ = Command.SoundEffectCommand.PlayAsync(UISoundEffectType.ChangeFailed);
                    }
                }
            }
            else
            {
                Debug.Log("当前为不可编辑模式");
                _ = Command.SoundEffectCommand.PlayAsync(UISoundEffectType.ChangeFailed);
            }
        }
        public static void RemoveCardFromDeck(GameObject clickCard)
        {
            Debug.Log("准备移除卡牌");

            if (Info.CardCompnentInfo.isEditDeckMode)
            {
                Debug.Log("移除卡牌");
                _ = Command.SoundEffectCommand.PlayAsync(UISoundEffectType.ChangeSuccess);
                int removeCardId = Info.CardCompnentInfo.distinctCardIds[Info.CardCompnentInfo.deckCardModels.IndexOf(clickCard)];
                Info.CardCompnentInfo.tempDeck.CardIds.Remove(removeCardId);
                Command.CardListCommand.Init(newTempDeck: Info.CardCompnentInfo.tempDeck, canChangeCard: true);
            }
            else
            {
                _ = Command.SoundEffectCommand.PlayAsync(UISoundEffectType.ChangeFailed);
            }
        }
    }
}