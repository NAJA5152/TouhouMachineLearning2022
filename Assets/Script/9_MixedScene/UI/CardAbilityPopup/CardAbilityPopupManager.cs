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
                if (focusCardID > 0 || Info.PageCompnentInfo.focusCamp != Camp.Neutral)
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
                cardName = cardInfo.TranslateName;
                ability = cardInfo.TranslateAbility;
                IntroductionBackground.gameObject.SetActive(false);
            }
            else
            {
                Card card = (Card)(object)target;
                cardName = card.TranslateName;
                ability = card.TranslateAbility;
                int lineCount = 0;
                card.cardStates.ForEach(state =>
                {
                    string newIntroduction = (state.ToString() + "_Introduction").TranslationGameText();
                    //算出单个状态介绍的长度+换行的长度
                    newIntroduction.Split('\n').ToList().ForEach(singleRowText =>
                    {
                        lineCount += singleRowText.Length / 13 + 1;
                    });
                    Introduction += newIntroduction + "\n";
                });
                //Debug.Log("状态栏行数"+lineCount);
                card.cardFields.ToList().ForEach(field =>
                {
                    string newIntroduction = (field.Key.ToString()).TranslationGameText( IsGetIntroduction:true).Replace("$Point$", field.Value.ToString());
                    //算出单个字段介绍的长度+换行的长度
                    newIntroduction.Split('\n').ToList().ForEach(singleRowText =>
                    {
                        lineCount += singleRowText.Length / 13 + 1;
                    });
                    Introduction += newIntroduction + "\n";
                });
                if (lineCount>0)
                {
                    IntroductionBackground.gameObject.SetActive(true);
                    IntroductionBackground.sizeDelta = new Vector2(300, lineCount * 15 + 100);
                    IntroductionText.text = Introduction;
                }
                else
                {
                    IntroductionBackground.gameObject.SetActive(false);
                }
            }
            Title.text = cardName;
            AbilityBackground.sizeDelta = new Vector2(300, (ability.Length / 13+1) * 15 + 100);
            //修改文本为富文本
            AbilityText.text = ability;
        }
    }
}