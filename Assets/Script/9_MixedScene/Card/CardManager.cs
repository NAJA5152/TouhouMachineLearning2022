﻿using Sirenix.OdinInspector;
using System.Linq;
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
        int gap_step = 0;
        Card thisCard => GetComponent<Card>();
        GameObject gap => transform.GetChild(1).gameObject;
        Material gapMaterial => gap.GetComponent<Renderer>().material;
        Material cardMaterial => GetComponent<Renderer>().material;
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
        private void Update()
        {
            if (gap_step == 1)
            {
                gapMaterial.SetFloat("_gapWidth", Mathf.Lerp(gapMaterial.GetFloat("_gapWidth"), 1.5f, Time.deltaTime * 20));
            }
            else if (gap_step == 2)
            {
                gapMaterial.SetFloat("_gapWidth", Mathf.Lerp(gapMaterial.GetFloat("_gapWidth"), 10, Time.deltaTime * 2));
                cardMaterial.SetFloat("_gapWidth", Mathf.Lerp(cardMaterial.GetFloat("_gapWidth"), 10, Time.deltaTime * 2));
            }

        }
        public void CreatGap()
        {
            gap.SetActive(true);
            gap_step = 1;
        }
        public void FoldGap()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            gap_step = 2;
        }
        public void DestoryGap()
        {
            gap.SetActive(false);
            gap_step = 0;
            Destroy(gameObject);
        }
        //弹出状态变动提示
        [Button]
        public async void ShowTips<T>(T text, Color color)
        {
            string showText = "";
            if (typeof(T) == typeof(int))
            {
                showText = text.ToString();
            }
            else if (typeof(T) == typeof(string))
            {
                showText =string.Join("\n",text.ToString().ToCharArray());
            }
            cardTips.GetComponent<Text>().text = showText;
            cardTips.GetComponent<Text>().color = color;

            await CustomThread.TimerAsync(0.2f, runAction: (process) => //在0.4秒内不断移动并降低透明度
            {
                cardTips.GetComponent<CanvasGroup>().alpha = process;
                cardTips.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await CustomThread.TimerAsync(0.2f, runAction: (process) => //在0.4秒内不断移动并降低透明度
            {
                cardTips.GetComponent<CanvasGroup>().alpha = 1 - process;
            });
        }
        //弹出伤害变动提示
        [Button]
        public void ShowTipsPoint(int point, Color color) => ShowTips(point, color);
        //弹出伤害变动提示
        [Button]
        public void ShowTipsString(string text, Color color) => ShowTips(text, color);
    }
}