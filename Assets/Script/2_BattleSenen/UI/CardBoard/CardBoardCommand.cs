using System.Collections.Generic;
using TMPro;
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
            UiInfo.ShowCardLIstOnBoard.ForEach(Object.Destroy);
            List<Card> Cards = Info.AgainstInfo.cardBoardList;
            for (int i = 0; i < Cards.Count; i++)
            {
                var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(Cards[i].CardID);

                GameObject NewCard = Object.Instantiate(UiInfo.CardModel);
                NewCard.transform.SetParent(UiInfo.Content);
                NewCard.SetActive(true);
                //设置对应立绘
                Texture2D texture = CardStandardInfo.icon;
                NewCard.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                //设置效果文本
                NewCard.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = Cards[i].TranslateAbility;
                //设置点数
                NewCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Cards[i].ShowPoint==0?"" : Cards[i].ShowPoint.ToString();
                //设置名字
                NewCard.name = CardStandardInfo.TranslateName;
                //设置对应次序
                NewCard.GetComponent<SelectCardManager>().Rank = i;
                //设置品质
                NewCard.GetComponent<Image>().color = CardStandardInfo.cardRank switch
                {
                    GameEnum.CardRank.Leader => Color.cyan,
                    GameEnum.CardRank.Gold => new Color(1,1,0,1),
                    GameEnum.CardRank.Silver => new Color(1, 1, 1, 1),
                    GameEnum.CardRank.Copper => new Color(1, 0.5f, 0, 1),
                    _ => Color.cyan,
                };
                UiInfo.ShowCardLIstOnBoard.Add(NewCard);
            }
            UiInfo.Content.GetComponent<RectTransform>().sizeDelta = new Vector2(Cards.Count * 275 + 000, UiInfo.Content.GetComponent<RectTransform>().sizeDelta.y);
        }
        //生成对局不存在的卡牌
        private static void CreatBoardCardVitual()
        {
            UiInfo.ShowCardLIstOnBoard.ForEach(Object.Destroy);
            List<int> CardIds = Info.AgainstInfo.cardBoardIDList;
            for (int i = 0; i < CardIds.Count; i++)
            {
                var CardStandardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(CardIds[i]);

                GameObject NewCard = Object.Instantiate(UiInfo.CardModel);
                NewCard.transform.SetParent(UiInfo.Content);
                NewCard.SetActive(true);
                //设置对应立绘
                Texture2D texture = CardStandardInfo.icon;
                NewCard.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                //设置效果文本
                NewCard.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = CardStandardInfo.TranslateAbility;
                //设置点数
                NewCard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = CardStandardInfo.point == 0 ? "" : CardStandardInfo.point.ToString();
                //设置名字
                NewCard.name = CardStandardInfo.TranslateName;
                //设置对应次序
                NewCard.GetComponent<SelectCardManager>().Rank = i;
                //设置品质
                NewCard.GetComponent<Image>().color = CardStandardInfo.cardRank switch
                {
                    GameEnum.CardRank.Leader => Color.cyan,
                    GameEnum.CardRank.Gold => Color.yellow,
                    GameEnum.CardRank.Silver => Color.gray,
                    GameEnum.CardRank.Copper => Color.magenta,
                    _ => Color.cyan,
                };
                UiInfo.ShowCardLIstOnBoard.Add(NewCard);
            }
            UiInfo.Content.GetComponent<RectTransform>().sizeDelta = new Vector2(CardIds.Count * 325 + 200, UiInfo.Content.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}