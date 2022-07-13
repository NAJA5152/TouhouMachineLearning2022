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
        public static Sprite GetFieldAndStateSprite<T>(T cardField)
        {
            Sprite targetSprite =  AssetBundleCommand.Load<Sprite>("FieldAndState", cardField.ToString());
            targetSprite ??= AssetBundleCommand.Load<Sprite>("FieldAndState", "None");
            return targetSprite;
        }
        //////////////////////////////////////////////////////////对战中游戏卡牌面板//////////////////////////////////////////////////////////////
        static Text UiText => UiInfo.CardBoard.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        static GameObject HideButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(0).gameObject;
        static GameObject JumpButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(1).gameObject;
        static GameObject ShowButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(2).gameObject;
        static GameObject CloseButton => UiInfo.CardBoard.transform.GetChild(1).GetChild(3).gameObject;
        static GameObject BackImage => UiInfo.CardBoard.transform.GetChild(0).gameObject;
        /// <summary>
        /// 修改展示板标题
        /// </summary>
        /// <param name="Title"></param>
        public static void SetCardBoardTitle(string Title) => UiText.text = Title;
        /// <summary>
        /// 打开展示板，并根据模式显示相应按钮
        /// </summary>
        /// <param name="mode"></param>
        public static void SetCardBoardOpen(CardBoardMode mode)
        {
            UiInfo.CardBoard.SetActive(true);
            BackImage.SetActive(false);
            HideButton.SetActive(false);
            JumpButton.SetActive(false);
            ShowButton.SetActive(false);
            CloseButton.SetActive(false);
            switch (mode)
            {
                case CardBoardMode.Default:
                    CloseButton.SetActive(true);
                    break;
                    //选择或换牌模式下
                case CardBoardMode.Select:
                case CardBoardMode.ExchangeCard:
                    HideButton.SetActive(true);
                    JumpButton.SetActive(true);
                    UiInfo.lastCardBoardMode = mode;
                    break;
                case CardBoardMode.ShowOnly:
                    break;
            }
        }
        public static void SetCardBoardClose()
        {
            UiInfo.CardBoard.SetActive(false);
            if (UiInfo.isCardBoardHide)
            {
                SetCardBoardHide();
            }
        }
        public static void SetCardBoardHide()
        {
            BackImage.SetActive(false);
            HideButton.SetActive(false);
            JumpButton.SetActive(false);
            ShowButton.SetActive(true);
            CloseButton.SetActive(false);
            //设置卡牌面板为隐藏模式
            UiInfo.isCardBoardHide = true;
        }
        public static void SetCardBoardShow()
        {
            string title = UiInfo.lastCardBoardMode == CardBoardMode.Select ? "Remaining".TranslationGameText() + Info.AgainstInfo.ExChangeableCardNum : "";
            UiCommand.SetCardBoardTitle(title);
            SetCardBoardOpen(UiInfo.lastCardBoardMode);
            CardBoardCommand.CreatBoardCardActual();
            //BackImage.SetActive(true);
            //HideButton.SetActive(true);
            //JumpButton.SetActive(true);
            //ShowButton.SetActive(false);
        }
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