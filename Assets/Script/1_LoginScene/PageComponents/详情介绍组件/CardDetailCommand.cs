using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    class CardDetailCommand
    {
        public static void ChangeFocusCard(int cardID)
        {
            if (cardID==1)
            {
                Info.PageCompnentInfo.targetCardName.GetComponent<Text>().text = "";
                Info.PageCompnentInfo.targetCardTag.GetComponent<Text>().text = "";
                Info.PageCompnentInfo.targetCardAbility.GetComponent<Text>().text = "";
            }
            else
            {
                var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo(cardID);
                Info.PageCompnentInfo.targetCardTexture.GetComponent<Image>().sprite =cardInfo.GetCardSprite();
                Info.PageCompnentInfo.targetCardName.GetComponent<Text>().text = cardInfo.TranslateName;
                Info.PageCompnentInfo.targetCardTag.GetComponent<Text>().text = cardInfo.TranslateTags;
                Info.PageCompnentInfo.targetCardAbility.GetComponent<Text>().text = cardInfo.TranslateAbility;
            }
        }
        internal static void ChangeFocusCamp()
        {
            var campInfo = Info.CampInfo.GetCampInfo(Info.PageCompnentInfo.focusCamp);
           // Info.PageCompnentInfo.targetCardTexture.GetComponent<Image>().sprite = Sprite.Create(campInfo.campTex, new Rect(0, 0, campInfo.campTex.width, campInfo.campTex.height), Vector2.zero);
            Info.PageCompnentInfo.targetCardTexture.GetComponent<Image>().sprite = campInfo.campTex;
            Info.PageCompnentInfo.targetCardName.GetComponent<Text>().text = campInfo.campName;
            Info.PageCompnentInfo.targetCardTag.GetComponent<Text>().text ="强化 反制";
            Info.PageCompnentInfo.targetCardAbility.GetComponent<Text>().text = campInfo.campIntroduction;
        }
    }
}