using System.Collections.Generic;
using TouhouMachineLearningSummary.Control;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Command
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
            //Info.GameUI.UiInfo.CardBoard.transform.GetChild(1).GetComponent<Text>().text = Info.GameUI.UiInfo.CardBoardTitle;
            UiInfo.ShowCardLIstOnBoard.ForEach(Object.Destroy);
            List<Card> Cards = Info.AgainstInfo.cardBoardList;
            for (int i = 0; i < Cards.Count; i++)
            {
                var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(Cards[i].CardID);
                GameObject NewCard = Object.Instantiate(UiInfo.CardModel);

                NewCard.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Cards[i].CardTranslateAbility;
                NewCard.transform.GetChild(1).GetComponent<Text>().text = Cards[i].ShowPoint==0?"" : Cards[i].ShowPoint.ToString();
                NewCard.name = CardStandardInfo.TranslateName;

                ////修改文本为富文本
                //Info.GameUI.UiInfo.IntroductionTitle.text = Title;
                //Info.GameUI.UiInfo.IntroductionText.text = Text;
                //Info.GameUI.UiInfo.IntroductionEffect.text = Effect;

                NewCard.GetComponent<SelectCardManager>().Rank = i;
                NewCard.transform.SetParent(UiInfo.Content);
                Texture2D texture = CardStandardInfo.icon;
                NewCard.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                UiInfo.ShowCardLIstOnBoard.Add(NewCard);
            }
            UiInfo.Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Cards.Count * 325 + 200, 800);
        }
        //生成对局不存在的卡牌
        private static void CreatBoardCardVitual()
        {
            //Info.GameUI.UiInfo.CardBoard.transform.GetChild(1).GetComponent<Text>().text = Info.GameUI.UiInfo.CardBoardTitle;
            UiInfo.ShowCardLIstOnBoard.ForEach(Object.Destroy);
            List<int> CardIds = Info.AgainstInfo.cardBoardIDList;
            for (int i = 0; i < CardIds.Count; i++)
            {
                var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(CardIds[i]);
                GameObject NewCard = Object.Instantiate(UiInfo.CardModel);
                NewCard.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = CardStandardInfo.TranslateAbility;
                NewCard.transform.GetChild(1).GetComponent<Text>().text = CardStandardInfo.point == 0 ? "" : CardStandardInfo.point.ToString();
                //string Title = card.CardName;
                //string Text = card.CardIntroduction;
                //string Effect = "";
                //int Heigh = Text.Length / 13 * 15 + 100;
                //Info.GameUI.UiInfo.IntroductionTextBackground.sizeDelta = new Vector2(300, Heigh);
                ////修改文本为富文本
                //Info.GameUI.UiInfo.IntroductionTitle.text = Title;
                //Info.GameUI.UiInfo.IntroductionText.text = Text;
                //Info.GameUI.UiInfo.IntroductionEffect.text = Effect;

                NewCard.GetComponent<SelectCardManager>().Rank = i;
                NewCard.transform.SetParent(UiInfo.Content);
                Texture2D texture = CardStandardInfo.icon;
                NewCard.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                UiInfo.ShowCardLIstOnBoard.Add(NewCard);
            }
            UiInfo.Content.GetComponent<RectTransform>().sizeDelta = new Vector2(CardIds.Count * 325 + 200, 800);
        }
    }
}