using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.Manager.GameUI;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command.GameUI
{
    public class UiCommand : MonoBehaviour
    {
        //////////////////////////////////////////////////////////对战中游戏卡牌面板//////////////////////////////////////////////////////////////
        public static void SetCardBoardShow() => Info.GameUI.UiInfo.CardBoard.SetActive(true);
        public static void SetCardBoardHide() => Info.GameUI.UiInfo.CardBoard.SetActive(false);
        public static void CardBoardReload() => CardBoardCommand.CreatBoardCardActual();
        public void CardBoardClose() => Info.AgainstInfo.IsSelectCardOver = true;
        public static void SetCardBoardTitle(string Title) => Info.GameUI.UiInfo.CardBoard.transform.GetChild(1).GetComponent<Text>().text = Title;
        //////////////////////////////////////////////////////////回合阶段提示UI//////////////////////////////////////////////////////////////
        public static async Task NoticeBoardShow(string Title)
        {

            Info.GameUI.UiInfo.NoticeBoard.transform.GetChild(0).GetComponent<Text>().text = Title;
            Info.GameUI.UiInfo.NoticeBoard.GetComponent<Image>().color = Info.AgainstInfo.IsMyTurn ? new Color(0.2f, 0.5f, 1, 0.5f) : new Color(1, 0.2f, 0.2f, 0.5f);
            Info.GameUI.UiInfo.NoticeBoard.transform.localScale = new Vector3(1, 0, 1);
            Info.GameUI.UiInfo.NoticeBoard.SetActive(true);
            await CustomThread.TimerAsync(0.5f, runAction: process =>
            {
                Info.GameUI.UiInfo.NoticeBoard.transform.localScale = new Vector3(1, process * process, 1);
            });
            await Task.Delay(1000);
            await CustomThread.TimerAsync(0.5f, runAction: process =>
            {
                Info.GameUI.UiInfo.NoticeBoard.transform.localScale = new Vector3(1, 1 - process * process, 1);
            });
            Info.GameUI.UiInfo.NoticeBoard.SetActive(false);
        }

        //////////////////////////////////////////////////////////箭头//////////////////////////////////////////////////////////////
        public static void CreatFreeArrow()
        {
            GameObject newArrow = Instantiate(Info.GameUI.UiInfo.Arrow);
            newArrow.name = "Arrow-null";
            newArrow.GetComponent<ArrowManager>().InitArrow(Info.AgainstInfo.ArrowStartCard, Info.GameUI.UiInfo.ArrowEndPoint);
            Info.AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFreeArrow()
        {
            GameObject targetArrow = Info.AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == null);
            Info.AgainstInfo.ArrowList.Remove(targetArrow);
            Destroy(targetArrow);
        }
        public static void CreatFixedArrow(Card card)
        {
            GameObject newArrow = Instantiate(Info.GameUI.UiInfo.Arrow);
            newArrow.name = "Arrow-" + card.name;
            newArrow.GetComponent<ArrowManager>().InitArrow(Info.AgainstInfo.ArrowStartCard, Info.AgainstInfo.playerFocusCard);
            Info.AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFixedArrow(Card card)
        {
            GameObject targetArrow = Info.AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == card);
            Info.AgainstInfo.ArrowList.Remove(targetArrow);
            Destroy(targetArrow);
        }
        public static void DestoryAllArrow()
        {
            Info.AgainstInfo.ArrowList.ForEach(Destroy);
            Info.AgainstInfo.ArrowList.Clear();
        }
    }
}