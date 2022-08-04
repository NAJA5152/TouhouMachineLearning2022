using Sirenix.OdinInspector;
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
        public GameObject cardIcon;
        private void OnMouseEnter()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                AgainstInfo.playerFocusCard = thisCard;
                _ = SoundEffectCommand.PlayAsync(SoundEffectType.CardSelect);
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
        private void OnMouseDown()
        {
            if (thisCard.isPrepareToPlay && !EventSystem.current.IsPointerOverGameObject())
            {
                AgainstInfo.playerPrePlayCard = thisCard;
            }
        }
        private void OnMouseUp()
        {
            if (AgainstInfo.playerPrePlayCard != null && !EventSystem.current.IsPointerOverGameObject())
            {
                if (AgainstInfo.PlayerFocusRegion != null && AgainstInfo.PlayerFocusRegion.name == "下方_墓地")
                {
                    Info.AgainstInfo.playerDisCard = Info.AgainstInfo.playerPrePlayCard;
                }
                //将卡牌放回（不做处理）
                else if (Info.AgainstInfo.PlayerFocusRegion != null && (AgainstInfo.PlayerFocusRegion.name == "下方_领袖" || AgainstInfo.PlayerFocusRegion.name == "下方_手牌"))
                {
                }
                else
                {
                    Info.AgainstInfo.playerPlayCard = Info.AgainstInfo.playerPrePlayCard;
                }
                Info.AgainstInfo.playerPrePlayCard = null;
            }
        }
        private void OnMouseOver()
        {
            //鼠标悬浮于卡牌上右键时可加载对应卡牌效果
            if (Input.GetMouseButtonUp(1) && thisCard.IsCanSee)
            {
                CardAbilityBoardManager.Manager.LoadCardFromGameCard(gameObject);
            }
            //鼠标悬浮于卡牌上左键时可换出卡牌面板
            if (Input.GetMouseButtonUp(0))
            {
                if (thisCard.CurrentRegion == GameRegion.Grave)
                {
                    UiCommand.SetCardBoardOpen(CardBoardMode.Temp);
                    UiCommand.SetCardBoardTitle(thisCard.CurrentOrientation == Orientation.Up ? "敌方墓地" : "我方墓地");
                    CardBoardCommand.LoadTempBoardCardList(
                        GameSystem.InfoSystem.AgainstCardSet[thisCard.CurrentOrientation][thisCard.CurrentRegion]
                        .CardList
                        );
                }
                if (thisCard.CurrentRegion == GameRegion.Deck && thisCard.CurrentOrientation == Orientation.Down)
                {
                    UiCommand.SetCardBoardOpen(CardBoardMode.Temp);
                    UiCommand.SetCardBoardTitle("我方卡组");
                    CardBoardCommand.LoadTempBoardCardList(
                        GameSystem.InfoSystem.AgainstCardSet[thisCard.CurrentOrientation][thisCard.CurrentRegion]
                        .CardList
                        //为了debug暂时以真实顺序排序
                        //.OrderBy(card => card.CardRank)
                        //.ThenBy(card => card.ShowPoint)
                        .ToList()
                        );
                }
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
                gapMaterial.SetFloat("_gapWidth", Mathf.Lerp(1.5f, 10f, process));
                cardMaterial.SetFloat("_gapWidth", Mathf.Lerp(1.5f, 10f, process));

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
        //弹出状态图标附加提示
        [Button]
        public async Task ShowStateIcon(CardState cardState)
        {
            cardIcon.GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(cardState);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(400);
            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        //弹出状态图标清除提示
        [Button]
        public async Task ShowStateIconBreak(CardState cardState)
        {
            cardIcon.GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(cardState);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.1f, runAction: (process) =>
            {
                cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1 - process * 1.5f);
                Debug.Log(cardIcon.GetComponent<Image>().material.GetFloat("_BreakStrength"));
                cardIcon.GetComponent<Image>().material.SetFloat("_Bias", process / 5);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        //弹出字段图标附加提示
        [Button]
        public async Task ShowFieldIcon(CardField cardField)
        {
            cardIcon.GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(cardField);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(800);
            await CustomThread.TimerAsync(0.4f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
            });
        }
        //弹出字段图标清除提示
        [Button]
        public async Task ShowFieldIconBreak(CardField cardField)
        {
            cardIcon.GetComponent<Image>().sprite = Command.UiCommand.GetFieldAndStateSprite(cardField);
            cardIcon.GetComponent<Image>().material.SetFloat("_Bias", 0);
            cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1);

            await CustomThread.TimerAsync(0.2f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - process * 30);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.1f, runAction: (process) =>
            {
                cardIcon.GetComponent<Image>().material.SetFloat("_BreakStrength", 1 - process * 1.5f);
                Debug.Log(cardIcon.GetComponent<Image>().material.GetFloat("_BreakStrength"));
                cardIcon.GetComponent<Image>().material.SetFloat("_Bias", process / 5);
            });
            await Task.Delay(200);
            await CustomThread.TimerAsync(0.4f, runAction: (process) =>
            {
                cardIcon.GetComponent<CanvasGroup>().alpha = 1 - process;
                cardIcon.transform.localPosition = new Vector3(0, 0, -1 - (1 - process) * 30);
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