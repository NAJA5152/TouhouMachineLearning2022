using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Manager
{
    //控制悬右键卡牌时展现的详情面板
    public class CardAbilityBoardManager : MonoBehaviour
    {
        //需求功能
        //切换上下一张卡牌
        //显示当前卡牌的各种信息

        public bool isOnMenu;//判断属于菜单场景还是战斗场景
        public static int focusCardID = -1;
        List<int> cardIds = new List<int>();

        public Text Title => transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        public Text AbilityText => transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>();
        public Text IntroductionText => transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        public RectTransform AbilityBackground => transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        public RectTransform IntroductionBackground => transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
       
        void Update()
        {
         
           
        }
        public void LoadCards()
        {

        }
        public void LoadCardsIds()
        {

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