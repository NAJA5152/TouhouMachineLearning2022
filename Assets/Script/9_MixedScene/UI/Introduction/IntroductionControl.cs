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


        public Text IntroductionTitle => transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        public Text IntroductionText => transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        public Text IntroductionEffect => transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        public RectTransform IntroductionTextBackground => transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        public RectTransform IntroductionEffectBackground => transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();

        float Cd;
        public Vector3 Bias;
        public Vector3 ViewportPoint => Camera.main.ScreenToViewportPoint(Input.mousePosition);
        public bool IsRight => ViewportPoint.x < 0.5;
        public bool IsDown => ViewportPoint.y < 0.5;
        void Update()
        {
            Bias = new Vector3(IsRight ? 0.1f : -0.1f, IsDown ? 0.1f : -0.1f);
            transform.position = Camera.main.ViewportToScreenPoint(ViewportPoint + Bias);
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
        public void ChangeIntroduction(int cardID)
        {
            var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo(cardID);
            string Title = cardInfo.translateName;
            string Text = cardInfo.translateAbility;
            string Effect = "";
            int Heigh = Text.Length / 13 * 15 + 100;
            IntroductionTextBackground.sizeDelta = new Vector2(300, Heigh);
            //修改文本为富文本
            IntroductionTitle.text = Title;
            IntroductionText.text = Text;
            IntroductionEffect.text = Effect;
        }
        public void ChangeIntroduction(Card target)
        {
            string Title = target.CardName;
            string Text = target.CardIntroduction;
            string Effect = "";
            int Heigh = Text.Length / 13 * 15 + 100;
            IntroductionTextBackground.sizeDelta = new Vector2(300, Heigh);
            //修改文本为富文本
            IntroductionTitle.text = Title;
            IntroductionText.text = Text;
            IntroductionEffect.text = Effect;
        }
        //public  void ChangeIntroduction<T>(T target)
        //{
        //    string Title;
        //    string Text;
        //    if (typeof(T) == typeof(int))
        //    {
        //        var cardInfo = Command.CardInspector.CardLibraryCommand.GetCardStandardInfo((int) target);
        //        Title = card.CardName;
        //        Text = card.CardIntroduction;
        //    }
        //    else
        //    {
        //        Card cardInfo = (Card)target;
        //        Title = cardInfo.CardName;
        //        Text = cardInfo.CardIntroduction;
        //    }


        //    string Effect = "";
        //    int Heigh = Text.Length / 13 * 15 + 100;
        //    Info.GameUI.UiInfo.IntroductionTextBackground.sizeDelta = new Vector2(300, Heigh);
        //    //修改文本为富文本
        //    IntroductionTitle.text = Title;
        //    IntroductionText.text = Text;
        //    IntroductionEffect.text = Effect;
        //}
    }
}