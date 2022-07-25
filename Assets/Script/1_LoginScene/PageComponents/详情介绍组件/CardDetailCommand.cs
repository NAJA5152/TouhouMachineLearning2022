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
        public static void ChangeFocusCamp()
        {
            var campInfo = Info.CampInfo.GetCampInfo(Info.PageCompnentInfo.focusCamp);
            Info.PageCompnentInfo.targetCardTexture.GetComponent<Image>().sprite = campInfo.campTex;
            Info.PageCompnentInfo.targetCardName.GetComponent<Text>().text = campInfo.campName;
            Info.PageCompnentInfo.targetCardTag.GetComponent<Text>().text ="强化 反制";
            Info.PageCompnentInfo.targetCardAbility.GetComponent<Text>().text = campInfo.campIntroduction;
        }
        public static void ChangeFocusLeader()
        {
            var leaderInfo = Info.CampInfo.GetLeaderInfo(Info.PageCompnentInfo.focusLeaderID);
            Info.PageCompnentInfo.targetCardTexture.GetComponent<Image>().sprite = leaderInfo.GetCardSprite();
            Info.PageCompnentInfo.targetCardName.GetComponent<Text>().text = leaderInfo.TranslateName;
            Info.PageCompnentInfo.targetCardTag.GetComponent<Text>().text = leaderInfo.TranslateTags;
            Info.PageCompnentInfo.targetCardAbility.GetComponent<Text>().text = leaderInfo.TranslateAbility;
        }
    }
}