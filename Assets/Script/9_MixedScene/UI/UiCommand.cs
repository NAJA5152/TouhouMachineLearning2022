using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.Command
{
    public class UiCommand
    {
        //////////////////////////////////////////////////////////状态与字段UI//////////////////////////////////////////////////////////////
        public static Sprite GetCardStateSprite(CardState cardState) => AssetBundleCommand.Load<Sprite>("FieldAndState\\" + cardState.ToString());
        public static Sprite GetCardFieldSprite(CardField cardField) => AssetBundleCommand.Load<Sprite>("FieldAndState\\" + cardField.ToString());
        public static Texture2D GetCardStateTexture(CardState cardState) => AssetBundleCommand.Load<Texture2D>("FieldAndState\\" + cardState.ToString());
        public static Texture2D GetCardFieldTexture(CardField cardField) => AssetBundleCommand.Load<Texture2D>("FieldAndState\\" + cardField.ToString());

        //////////////////////////////////////////////////////////对战中游戏卡牌面板//////////////////////////////////////////////////////////////

        static Text UiText => UiInfo.CardBoard.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        static GameObject HideButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(0).gameObject;
        static GameObject JumpButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(1).gameObject;
        static GameObject ShowButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(2).gameObject;
        static GameObject CloseButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(3).gameObject;
        static GameObject BackImage => UiInfo.CardBoard.transform.GetChild(0).gameObject;
        public static void SetCardBoardTitle(string Title) => UiText.text = Title;

        public static void SetCardBoardOpen(CardBoardMode mode)
        {
            UiInfo.CardBoard.SetActive(true);
            HideButton.SetActive(false);
            JumpButton.SetActive(false);
            ShowButton.SetActive(false);
            CloseButton.SetActive(false);
            switch (mode)
            {
                case CardBoardMode.Default:
                    CloseButton.SetActive(true);
                    break;
                case CardBoardMode.Select:
                    HideButton.SetActive(true);
                    JumpButton.SetActive(true);
                    break;
                case CardBoardMode.ExchangeCard:
                    HideButton.SetActive(true);
                    JumpButton.SetActive(true);
                    break;
                case CardBoardMode.ShowOnly:
                    break;
            }
        }

        public static void SetCardBoardClose() => UiInfo.CardBoard.SetActive(false);
        public static void SetCardBoardHide()
        {
            BackImage.SetActive(false);
            HideButton.SetActive(false);
            JumpButton.SetActive(false);
            ShowButton.SetActive(true);
            CloseButton.SetActive(false);
        }
        public static void SetCardBoardShow(CardBoardMode mode)
        {
            BackImage.SetActive(true);
            HideButton.SetActive(true);
            JumpButton.SetActive(true);
            ShowButton.SetActive(false);
        }
        public static void CardBoardReload() => CardBoardCommand.CreatBoardCardActual();
        public static void CardBoardSelectOver() => AgainstInfo.IsSelectCardOver = true;
        //////////////////////////////////////////////////////////回合阶段提示UI//////////////////////////////////////////////////////////////
        public static async Task NoticeBoardShow(string Title)
        {

            UiInfo.NoticeBoard.transform.GetChild(0).GetComponent<Text>().text = Title;
            UiInfo.NoticeBoard.GetComponent<Image>().color = AgainstInfo.IsMyTurn ? new Color(0.2f, 0.5f, 1, 0.5f) : new Color(1, 0.2f, 0.2f, 0.5f);
            UiInfo.NoticeBoard.transform.localScale = new Vector3(1, 0, 1);
            UiInfo.NoticeBoard.SetActive(true);
            await CustomThread.TimerAsync(0.3f, runAction: process =>
            {
                UiInfo.NoticeBoard.transform.localScale = new Vector3(1, process * process, 1);
            });
            await Task.Delay(500);
            await CustomThread.TimerAsync(0.3f, runAction: process =>
            {
                UiInfo.NoticeBoard.transform.localScale = new Vector3(1, 1 - process * process, 1);
            });
            UiInfo.NoticeBoard.SetActive(false);
        }

        //////////////////////////////////////////////////////////箭头//////////////////////////////////////////////////////////////
        public static void CreatFreeArrow()
        {
            GameObject newArrow = GameObject.Instantiate(UiInfo.Arrow);
            newArrow.name = "Arrow-null";
            newArrow.GetComponent<ArrowManager>().InitArrow(AgainstInfo.ArrowStartCard, UiInfo.ArrowEndPoint);
            AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFreeArrow()
        {
            GameObject targetArrow = AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == null);
            AgainstInfo.ArrowList.Remove(targetArrow);
            GameObject.Destroy(targetArrow);
        }
        public static void CreatFixedArrow(Card card)
        {
            GameObject newArrow = GameObject.Instantiate(UiInfo.Arrow);
            newArrow.name = "Arrow-" + card.name;
            newArrow.GetComponent<ArrowManager>().InitArrow(AgainstInfo.ArrowStartCard, AgainstInfo.playerFocusCard);
            AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFixedArrow(Card card)
        {
            GameObject targetArrow = AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == card);
            AgainstInfo.ArrowList.Remove(targetArrow);
            GameObject.Destroy(targetArrow);
        }
        public static void DestoryAllArrow()
        {
            AgainstInfo.ArrowList.ForEach(GameObject.Destroy);
            AgainstInfo.ArrowList.Clear();
        }
    }
}