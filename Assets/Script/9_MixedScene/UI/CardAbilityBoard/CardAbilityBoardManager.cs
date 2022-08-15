using TMPro;
using TouhouMachineLearningSummary.Extension;
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
        enum LoadType { FromLibrary, FromCardList, FromCard, }
        private void Awake() => Manager = this;
        int currentRank { get; set; } = 0;
        Card CurrentGameCard { get; set; }
        LoadType CurrentLoadType { get; set; }
        public static CardAbilityBoardManager Manager { get; set; }
        public Image Texture => transform.GetChild(0).GetChild(1).GetComponent<Image>();
        public TextMeshProUGUI TagText => transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        public TextMeshProUGUI Name => transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
        public TextMeshProUGUI AbilityText => transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>();
        public TextMeshProUGUI DescribeText => transform.GetChild(0).GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>();
        public void Show() => transform.GetChild(0).gameObject.SetActive(true);
        public void Close() => transform.GetChild(0).gameObject.SetActive(false);

        public void LoadCardsIdsFromCardList(GameObject cardModel)
        {
            CurrentLoadType = LoadType.FromCardList;
            currentRank = Info.PageCompnentInfo.deckCardModels.IndexOf(cardModel);
            ChangeIntroduction(Info.PageCompnentInfo.distinctCardIds[currentRank]);
            Show();
        }
        public void LoadCardsIdsFromCardLibrary(GameObject cardModel)
        {
            CurrentLoadType = LoadType.FromLibrary;
            currentRank = Info.PageCompnentInfo.libraryCardModels.IndexOf(cardModel);
            ChangeIntroduction(Info.PageCompnentInfo.LibraryFilterCardList[currentRank].cardID);
            Show();
        }
        public void LoadCardFromGameCard(GameObject cardModel)
        {
            CurrentGameCard = cardModel.GetComponent<Card>();
            CurrentLoadType = LoadType.FromCard;
            currentRank = CurrentGameCard.BelongCardList.IndexOf(CurrentGameCard);
            ChangeIntroduction(CurrentGameCard);
            Show();
        }
        public void LoadLastCardInfo()
        {
            switch (CurrentLoadType)
            {
                case LoadType.FromLibrary:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(Info.PageCompnentInfo.LibraryFilterCardList[currentRank].cardID);
                    break;
                case LoadType.FromCardList:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(Info.PageCompnentInfo.distinctCardIds[currentRank]);
                    break;
                case LoadType.FromCard:
                    currentRank = Mathf.Max(0, currentRank - 1);
                    ChangeIntroduction(CurrentGameCard.BelongCardList[currentRank]);
                    break;
                default:
                    break;
            }
        }
        public void LoadNextCardInfo()
        {
            switch (CurrentLoadType)
            {
                case LoadType.FromLibrary:
                    currentRank = Mathf.Min(Info.PageCompnentInfo.LibraryFilterCardList.Count - 1, currentRank + 1);
                    ChangeIntroduction(Info.PageCompnentInfo.LibraryFilterCardList[currentRank].cardID);
                    break;
                case LoadType.FromCardList:
                    currentRank = Mathf.Min(Info.PageCompnentInfo.distinctCardIds.Count - 1, currentRank + 1);
                    ChangeIntroduction(Info.PageCompnentInfo.distinctCardIds[currentRank]);
                    break;
                case LoadType.FromCard:
                    currentRank = Mathf.Min(CurrentGameCard.BelongCardList.Count - 1, currentRank + 1);
                    ChangeIntroduction(CurrentGameCard.BelongCardList[currentRank]);
                    break;
                default:
                    break;
            }
        }
        public void ChangeIntroduction<T>(T target)
        {
            //对当前能力的说明
            string ability = "";
            //对当前状态和字段的说明
            string Introduction = "";
            if (typeof(T) == typeof(int))
            {
                var cardInfo = CardAssemblyManager.GetLastCardInfo((int)(object)target);
                Texture.sprite = cardInfo.icon.ToSprite();
                Name.text = cardInfo.TranslateName;
                DescribeText.text = cardInfo.TranslateDescribe;
                ability = cardInfo.TranslateAbility;
                ability = KeyWordManager.ReplaceAbilityKeyWord(ability);
                AbilityText.text = ability;
                TagText.text = cardInfo.TranslateTags;
            }
            else
            {
                Card card = (Card)(object)target;
                Texture.sprite = card.Icon.ToSprite();
                Name.text = card.TranslateName;
                DescribeText.text = card.TranslateDescribe;
                ability = card.TranslateAbility;
                ability = KeyWordManager.ReplaceAbilityKeyWord(ability);
                AbilityText.text = ability;
                TagText.text = card.TranslateTags;
                //IntroductionBackground.gameObject.SetActive(true);
                //IntroductionBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
            }
            //AbilityBackground.sizeDelta = new Vector2(300, ability.Length / 13 * 15 + 100);
            //修改文本为富文本
        }
    }
}