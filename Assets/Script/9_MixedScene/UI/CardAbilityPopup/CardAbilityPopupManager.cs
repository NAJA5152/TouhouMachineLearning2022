using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    //控制悬停于卡牌时展现的能力悬浮框
    public class CardAbilityPopupManager : MonoBehaviour
    {
        public bool isOnMenu;//判断属于菜单场景还是战斗场景
        public static int focusCardID = -1;


        public Text Title => transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        public Text AbilityText => transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        public Text IntroductionText => transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        public RectTransform AbilityBackground => transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        public RectTransform IntroductionBackground => transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();

        float Cd;
        public Vector3 Bias;
        public Vector3 ViewportPoint => Camera.main.ScreenToViewportPoint(Input.mousePosition);
        public bool IsRight => ViewportPoint.x < 0.5;
        public bool IsDown => ViewportPoint.y < 0.5;
        void Update()
        {
            Bias = new Vector3(IsRight ? 0.1f : -0.1f, IsDown ? 0.1f : -0.1f);
            transform.position = Camera.main.ViewportToScreenPoint(ViewportPoint + Bias);
            //菜单场景下，获取卡牌id信息，并进行显示
            //对战场景下，获取卡牌实例信息，并进行显示
            if (isOnMenu)
            {
                if (focusCardID > 0 || Info.CardCompnentInfo.focusCamp != Camp.Neutral)
                {
                    Cd = Mathf.Min(0.25f, Cd + Time.deltaTime);
                }
                else
                {
                    Cd = 0;
                }
                if (Cd == 0.25f)
                {
                    if (Command.MenuStateCommand.HasState(MenuState.CardLibrary))
                    {
                        Command.CardDetailCommand.ChangeFocusCard(focusCardID);
                    }
                    if (Command.MenuStateCommand.HasState(MenuState.CampSelect))
                    {
                        Command.CardDetailCommand.ChangeFocusCamp();
                    }
                    else
                    {
                        ChangeIntroduction(focusCardID);
                        transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                if (Info.AgainstInfo.playerFocusCard != null && Info.AgainstInfo.playerFocusCard.IsCanSee)
                {
                    Cd = Mathf.Min(0.25f, Cd + Time.deltaTime);
                }
                else
                {
                    Cd = 0;
                }
                if (Cd == 0.25f)
                {
                    ChangeIntroduction(Info.AgainstInfo.playerFocusCard);
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        public void ChangeIntroduction<T>(T target)
        {
            string cardName = "";
            string ability = "";
            string Introduction = "";
            if (typeof(T) == typeof(int))
            {
                var cardInfo = CardAssemblyManager.GetLastCardInfo((int)(object)target);
                cardName = cardInfo.translateName;
                ability = cardInfo.translateAbility;
                IntroductionBackground.gameObject.SetActive(false);
            }
            else
            {
                Card card = (Card)(object)target;
                cardName = card.CardName;
                ability = card.CardIntroduction;
                int lines=0;
                card.cardStates.ForEach(state =>
                {
                    switch (state)
                    {
                        case CardState.Seal:
                            break;
                        case CardState.Invisibility:
                            break;
                        case CardState.Pry:
                            break;
                        case CardState.Close:
                            break;
                        case CardState.Fate:
                            break;
                        case CardState.Lurk:
                            break;
                        case CardState.Secret:
                            break;
                        case CardState.Furor:
                            break;
                        case CardState.Docile:
                            break;
                        case CardState.Poisoning:
                            break;
                        case CardState.Rely:
                            break;
                        case CardState.Water:
                            break;
                        case CardState.Fire:
                            break;
                        case CardState.Wind:
                            break;
                        case CardState.Soil:
                            break;
                        case CardState.Hold:
                            break;
                        case CardState.Congealbounds:
                            break;
                        default:
                            break;
                    }
                });
                card.cardFields.ToList().ForEach(field =>
                {
                    string newIntroduction = "";
                    switch (field.Key)
                    {
                        case CardField.Timer:
                            newIntroduction = $"计时({field.Value})：计数为0时触发效果";
                            break;
                        case CardField.Vitality:
                            newIntroduction = $"活力({field.Value})：增强两侧单位效果";
                            break;
                        case CardField.Apothanasia:
                            newIntroduction = $"延命({field.Value})：当单位点数为0时，回合结束时抵消一次进入墓地效果";
                            break;
                        case CardField.Chain:
                            newIntroduction = $"连锁({field.Value})：当前回合打出卡牌的数量";
                            break;
                        case CardField.Energy:
                            newIntroduction = $"能量({field.Value})：强化机械的触发效果，过多时触发\"超载\"效果";
                            break;
                        case CardField.Shield:
                            newIntroduction = $"护盾({field.Value})：受到\"伤害\"类型效果时时抵消相应点数的伤害";
                            break;
                        default:
                            break;
                    }
                    //算出单个介绍的长度+换行的长度
                    lines += newIntroduction.Length / 13 + 1;
                    Introduction += newIntroduction + "\n";
                });
                IntroductionBackground.gameObject.SetActive(true);
                IntroductionBackground.sizeDelta = new Vector2(300, lines* 15 + 100);
                IntroductionText.text = Introduction;
            }
            Title.text = cardName;
            AbilityBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
            //修改文本为富文本
            AbilityText.text = ability;
        }
    }
}