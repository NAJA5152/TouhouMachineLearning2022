﻿using Sirenix.OdinInspector;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    public class CardManager : MonoBehaviour
    {
        Card thisCard => GetComponent<Card>();
        Material cardMaterial => GetComponent<Renderer>().material;
        public GameObject gap;
        Material gapMaterial => gap.GetComponent<Renderer>().material;
        public GameObject cardTips;
        private void OnMouseEnter()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                AgainstInfo.playerFocusCard = thisCard;
                NetCommand.AsyncInfo(NetAcyncType.FocusCard);
            }
        }
        private void OnMouseExit()
        {
            if (AgainstInfo.playerFocusCard == thisCard)
            {
                AgainstInfo.playerFocusCard = null;
                NetCommand.AsyncInfo(NetAcyncType.FocusCard);
            }
        }
        private void OnMouseDown() => CardCommand.OnMouseDown(thisCard);
        private void OnMouseUp() => CardCommand.OnMouseUp(thisCard);
        private void OnMouseOver()
        {
            if (Input.GetMouseButtonUp(1) && thisCard.IsCanSee)
            {
                CardAbilityBoardManager.Manager.LoadCardFromGameCard(gameObject);
            }
        }
        [Button("除外")]
        public async Task CreatGapAsync()
        {
            gap.SetActive(true);
            await CustomThread.TimerAsync(0.8f, runAction: (process) =>
            {
                gapMaterial.SetFloat("_gapWidth", Mathf.Lerp(10f, 1.5f, process));
            });
            await Task.Delay(1200);
            transform.GetChild(0).gameObject.SetActive(false);
            await CustomThread.TimerAsync(0.6f, runAction: (process) =>
            {
                gapMaterial.SetFloat("_gapWidth", Mathf.Lerp(1.5f,10f,  process));
                cardMaterial.SetFloat("_gapWidth", Mathf.Lerp(1.5f,10f,  process));

            });
            gap.SetActive(false);
            Command.CardCommand.RemoveCard(thisCard);
            Destroy(gameObject);
        }
        //弹出点数或状态变动提示
        [Button]
        public async Task ShowTips(string text, Color color, bool isVertical = true)
        {
            if (isVertical)
            {
                text = string.Join("\n", text.ToString().ToCharArray());
            }
            cardTips.GetComponent<Text>().text = text;
            cardTips.GetComponent<Text>().color = color;

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardTips.GetComponent<CanvasGroup>().alpha = process;
                cardTips.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(800);
            await CustomThread.TimerAsync(0.4f, runAction: (process) =>
            {
                cardTips.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardTips.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        [Button]
        public async Task Test()
        {
            await ShowTips("+1", Color.green, false);
            await ShowTips("-10", Color.red, false);
            await ShowTips("-3", Color.white, false);
            await ShowTips("支柱", Color.cyan);
            await ShowTips("活力", Color.yellow);
            await ShowTips("封印", new Color(1, 1, 1));
            await ShowTips("中毒", new Color(1, 0, 1));
            await ShowTips("掉SAN", new Color(0, 0.2f, 0));
            await ShowTips("魅了", new Color(1, 0, 0));
        }
    }
}