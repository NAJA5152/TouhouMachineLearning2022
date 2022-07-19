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
            Info.PageCompnentInfo.isEditDeckMode = canChangeCard;
            Info.PageCompnentInfo.okButton.SetActive(canChangeCard);
            Info.PageCompnentInfo.cancelButton.SetActive(canChangeCard);
            Info.PageCompnentInfo.changeButton.SetActive(!canChangeCard);
            //如果当前状态是卡组列表改变模式
            if (Command.MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
            {
                Info.PageCompnentInfo.cardDeckNameModel.GetComponent<Text>().text = Info.PageCompnentInfo.tempDeck.DeckName;
            }
            else
            {
                Info.PageCompnentInfo.cardDeckNameModel.GetComponent<Text>().text = Info.AgainstInfo.onlineUserInfo.UseDeck.DeckName;
            }
            Log.Show("配置卡组名");
            if (isInitOptions)
            {
                //初始化领袖栏?可以舍去？
                Info.PageCompnentInfo.deckCardModels.ForEach(model =>
                {
                    if (model != null)
                    {
                        Object.Destroy(model);
                    }
                });
                Info.PageCompnentInfo.deckCardModels.Clear();
            }
            //如果领袖存在，载入对应卡图，否则加载空卡图
            if (Info.PageCompnentInfo.tempDeck.LeaderId!=0)
            {
                var cardTexture = Manager.CardAssemblyManager.GetLastCardInfo(Info.PageCompnentInfo.tempDeck.LeaderId).icon;
                Info.PageCompnentInfo.cardLeaderImageModel.GetComponent<Image>().material.mainTexture = cardTexture;
            }
            else
            {

            }
           
            Log.Show("配置领袖");
            int deskCardNumber = Info.PageCompnentInfo.distinctCardIds.Count();
            int deskModelNumber = Info.PageCompnentInfo.deckCardModels.Count;
            Info.PageCompnentInfo.deckCardModels.ForEach(model => model.SetActive(false));
            if (deskCardNumber > deskModelNumber)
            {
                for (int i = 0; i < deskCardNumber - deskModelNumber; i++)
                {
                    var newCardModel = Object.Instantiate(Info.PageCompnentInfo.cardDeckCardModel, Info.PageCompnentInfo.cardDeckContent.transform);
                    Info.PageCompnentInfo.deckCardModels.Add(newCardModel);
                }
            }
            Log.Show("新增牌组栏");

            //初始化卡牌栏
            for (int i = 0; i < Info.PageCompnentInfo.distinctCardIds.Count(); i++)
            {
                int cardID = Info.PageCompnentInfo.distinctCardIds[i];
                GameObject currentCardModel = Info.PageCompnentInfo.deckCardModels[i];

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
                    currentCardModel.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "x" + Info.PageCompnentInfo.tempDeck.CardIds.Count(id => id == cardID);

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
            Info.AgainstInfo.onlineUserInfo.UseDeck = Info.PageCompnentInfo.tempDeck;
            await Info.AgainstInfo.onlineUserInfo.UpdateDecksAsync();
            Command.CardListCommand.Init();
            Command.MenuStateCommand.RebackStare();
        }
        public static void CancelDeck()
        {
            Debug.Log("取消卡组修改");
            Info.PageCompnentInfo.tempDeck = Info.AgainstInfo.onlineUserInfo.UseDeck;
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
                    Info.PageCompnentInfo.tempDeck.DeckName = text;
                    Command.CardListCommand.Init();
                    await Task.Delay(100);
                }, inputField: Info.AgainstInfo.onlineUserInfo.UseDeck.DeckName);
            }
        }

        public static void FocusDeckCardOnMenu(GameObject cardModel)
        {
            int focusCardRank = Info.PageCompnentInfo.deckCardModels.IndexOf(cardModel);
            int cardID = Info.PageCompnentInfo.distinctCardIds[focusCardRank];
            CardAbilityPopupManager.focusCardID = cardID;
        }
        public static void LostFocusCardOnMenu()
        {
            CardAbilityPopupManager.focusCardID = 0;
        }
        public static void AddCardFromLibrary(GameObject clickCard)
        {
            if (Info.PageCompnentInfo.isEditDeckMode)
            {
                Debug.Log("添加卡牌" + clickCard.name);
                int clickCardId = int.Parse(clickCard.name);
                int usedCardIdsNum = Info.PageCompnentInfo.tempDeck.CardIds.Count(id => id == clickCardId);
                var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo(clickCardId);
                if (cardInfo.cardRank == CardRank.Leader)
                {
                    Info.PageCompnentInfo.tempDeck.LeaderId = cardInfo.cardID;
                    _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeSuccess);
                }
                else
                {
                    int allowAddNum = cardInfo.cardRank == GameEnum.CardRank.Copper ? 3 : 1;
                    if (usedCardIdsNum < Mathf.Min(allowAddNum, Command.CardLibraryCommand.GetHasCardNum(clickCard.name)))
                    {
                        Info.PageCompnentInfo.tempDeck.CardIds.Add(clickCardId);
                        Command.CardListCommand.Init(newTempDeck: Info.PageCompnentInfo.tempDeck, canChangeCard: true);
                        _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeSuccess);
                    }
                    else
                    {
                        _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeFailed);
                    }
                }
            }
            else
            {
                Debug.Log("当前为不可编辑模式");
                _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeFailed);
            }
        }
        public static void RemoveCardFromDeck(GameObject clickCard)
        {
            Debug.Log("准备移除卡牌");

            if (Info.PageCompnentInfo.isEditDeckMode)
            {
                Debug.Log("移除卡牌");
                _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeSuccess);
                int removeCardId = Info.PageCompnentInfo.distinctCardIds[Info.PageCompnentInfo.deckCardModels.IndexOf(clickCard)];
                Info.PageCompnentInfo.tempDeck.CardIds.Remove(removeCardId);
                Command.CardListCommand.Init(newTempDeck: Info.PageCompnentInfo.tempDeck, canChangeCard: true);
            }
            else
            {
                _ = Command.SoundEffectCommand.PlayAsync(SoundEffectType.ChangeFailed);
            }
        }
    }
}