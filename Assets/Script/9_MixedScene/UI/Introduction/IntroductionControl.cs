using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Control.GameUI
{

    public class IntroductionControl : MonoBehaviour
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
                if (focusCardID > 0 || Info.CardCompnentInfo.focusCamp != GameEnum.Camp.Neutral)
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
                var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo((int)(object)target);
                cardName = cardInfo.translateName;
                ability = cardInfo.translateAbility;
                IntroductionBackground.gameObject.SetActive(false);
            }
            else
            {
                Card card = (Card)(object)target;
                cardName = card.CardName;
                ability = card.CardIntroduction;
                card.cardFields.ToList().ForEach(field =>
                {
                    switch (field.Key)
                    {
                        case CardField.Timer:
                            break;
                        case CardField.Vitality:
                            Introduction += $"活力：增强两侧单位效果{field.Value}";
                            break;
                        case CardField.Apothanasia:
                            break;
                        default:
                            break;
                    }
                });
                IntroductionBackground.gameObject.SetActive(true);
                IntroductionBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
                IntroductionText.text = Introduction;
            }
            Title.text = cardName;
            AbilityBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
            //修改文本为富文本
            AbilityText.text = ability;
        }
    }
}