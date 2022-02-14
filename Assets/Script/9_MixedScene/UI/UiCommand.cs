﻿using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.Manager.GameUI;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.Command
{
    public class UiCommand : MonoBehaviour
    {
        //////////////////////////////////////////////////////////对战中游戏卡牌面板//////////////////////////////////////////////////////////////
        public static void SetCardBoardShow() => UiInfo.CardBoard.SetActive(true);
        public static void SetCardBoardHide() => UiInfo.CardBoard.SetActive(false);
        public static void CardBoardReload() => CardBoardCommand.CreatBoardCardActual();
        public void CardBoardClose() => AgainstInfo.IsSelectCardOver = true;
        public static void SetCardBoardTitle(string Title) => UiInfo.CardBoard.transform.GetChild(1).GetComponent<Text>().text = Title;
        //////////////////////////////////////////////////////////回合阶段提示UI//////////////////////////////////////////////////////////////
        public static async Task NoticeBoardShow(string Title)
        {

            UiInfo.NoticeBoard.transform.GetChild(0).GetComponent<Text>().text = Title;
            UiInfo.NoticeBoard.GetComponent<Image>().color = AgainstInfo.IsMyTurn ? new Color(0.2f, 0.5f, 1, 0.5f) : new Color(1, 0.2f, 0.2f, 0.5f);
            UiInfo.NoticeBoard.transform.localScale = new Vector3(1, 0, 1);
            UiInfo.NoticeBoard.SetActive(true);
            await CustomThread.TimerAsync(0.5f, runAction: process =>
            {
                UiInfo.NoticeBoard.transform.localScale = new Vector3(1, process * process, 1);
            });
            await Task.Delay(1000);
            await CustomThread.TimerAsync(0.5f, runAction: process =>
            {
                UiInfo.NoticeBoard.transform.localScale = new Vector3(1, 1 - process * process, 1);
            });
            UiInfo.NoticeBoard.SetActive(false);
        }

        //////////////////////////////////////////////////////////箭头//////////////////////////////////////////////////////////////
        public static void CreatFreeArrow()
        {
            GameObject newArrow = Instantiate(UiInfo.Arrow);
            newArrow.name = "Arrow-null";
            newArrow.GetComponent<ArrowManager>().InitArrow(AgainstInfo.ArrowStartCard, UiInfo.ArrowEndPoint);
            AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFreeArrow()
        {
            GameObject targetArrow = AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == null);
            AgainstInfo.ArrowList.Remove(targetArrow);
            Destroy(targetArrow);
        }
        public static void CreatFixedArrow(Card card)
        {
            GameObject newArrow = Instantiate(UiInfo.Arrow);
            newArrow.name = "Arrow-" + card.name;
            newArrow.GetComponent<ArrowManager>().InitArrow(AgainstInfo.ArrowStartCard, AgainstInfo.playerFocusCard);
            AgainstInfo.ArrowList.Add(newArrow);
        }
        public static void DestoryFixedArrow(Card card)
        {
            GameObject targetArrow = AgainstInfo.ArrowList.First(arrow => arrow.GetComponent<ArrowManager>().targetCard == card);
            AgainstInfo.ArrowList.Remove(targetArrow);
            Destroy(targetArrow);
        }
        public static void DestoryAllArrow()
        {
            AgainstInfo.ArrowList.ForEach(Destroy);
            AgainstInfo.ArrowList.Clear();
        }
    }
}