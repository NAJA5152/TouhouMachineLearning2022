using System.Collections.Generic;
using TouhouMachineLearningSummary.Control.GameUI;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Command.GameUI
{
    public class CardBoardCommand
    {
        public static void LoadBoardCardList(List<int> cardIds)
        {
            Info.AgainstInfo.cardBoardIDList = cardIds;
            CreatBoardCardVitual();
        }
        public static void LoadBoardCardList(List<Card> cards)
        {
            Info.AgainstInfo.cardBoardList = cards;
            CreatBoardCardActual();
        }
        public void Replace(int num, Card card)
        {

        }
        //生成对局存在的卡牌
        public static void CreatBoardCardActual()
        {
            Info.GameUI.UiInfo.CardBoard.transform.GetChild(1).GetComponent<Text>().text = Info.GameUI.UiInfo.CardBoardTitle;
            Info.GameUI.UiInfo.ShowCardLIstOnBoard.ForEach(GameObject.Destroy);
            List<Card> Cards = Info.AgainstInfo.cardBoardList;
            for (int i = 0; i < Cards.Count; i++)
            {
                var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(Cards[i].cardID);
                GameObject NewCard = GameObject.Instantiate(Info.GameUI.UiInfo.CardModel);

                NewCard.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = CardStandardInfo.translateAbility;
                NewCard.name = CardStandardInfo.translateName;

                ////修改文本为富文本
                //Info.GameUI.UiInfo.IntroductionTitle.text = Title;
                //Info.GameUI.UiInfo.IntroductionText.text = Text;
                //Info.GameUI.UiInfo.IntroductionEffect.text = Effect;

                NewCard.GetComponent<BoardCardControl>().Rank = i;
                NewCard.transform.SetParent(Info.GameUI.UiInfo.Constant);
                Texture2D texture = CardStandardInfo.icon;
                NewCard.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                Info.GameUI.UiInfo.ShowCardLIstOnBoard.Add(NewCard);
            }
            Info.GameUI.UiInfo.Constant.GetComponent<RectTransform>().sizeDelta = new Vector2(Cards.Count * 325 + 200, 800);
        }
        //生成对局不存在的卡牌
        private static void CreatBoardCardVitual()
        {
            Info.GameUI.UiInfo.CardBoard.transform.GetChild(1).GetComponent<Text>().text = Info.GameUI.UiInfo.CardBoardTitle;
            Info.GameUI.UiInfo.ShowCardLIstOnBoard.ForEach(GameObject.Destroy);
            List<int> CardIds = Info.AgainstInfo.cardBoardIDList;
            for (int i = 0; i < CardIds.Count; i++)
            {
                var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(CardIds[i]);
                GameObject NewCard = GameObject.Instantiate(Info.GameUI.UiInfo.CardModel);
                NewCard.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = CardStandardInfo.translateAbility;

                //string Title = card.CardName;
                //string Text = card.CardIntroduction;
                //string Effect = "";
                //int Heigh = Text.Length / 13 * 15 + 100;
                //Info.GameUI.UiInfo.IntroductionTextBackground.sizeDelta = new Vector2(300, Heigh);
                ////修改文本为富文本
                //Info.GameUI.UiInfo.IntroductionTitle.text = Title;
                //Info.GameUI.UiInfo.IntroductionText.text = Text;
                //Info.GameUI.UiInfo.IntroductionEffect.text = Effect;

                NewCard.GetComponent<BoardCardControl>().Rank = i;
                NewCard.transform.SetParent(Info.GameUI.UiInfo.Constant);
                Texture2D texture = CardStandardInfo.icon;
                NewCard.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                Info.GameUI.UiInfo.ShowCardLIstOnBoard.Add(NewCard);
            }
            Info.GameUI.UiInfo.Constant.GetComponent<RectTransform>().sizeDelta = new Vector2(CardIds.Count * 325 + 200, 800);
        }
    }
}