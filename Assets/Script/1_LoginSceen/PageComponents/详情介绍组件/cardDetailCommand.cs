using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    class cardDetailCommand
    {
        public static void ChangeFocusCard(int cardID)
        {
            if (cardID==1)
            {
                Info.CardCompnentInfo.targetCardName.GetComponent<Text>().text = "";
                Info.CardCompnentInfo.targetCardTag.GetComponent<Text>().text = "";
                Info.CardCompnentInfo.targetCardAbility.GetComponent<Text>().text = "";
            }
            else
            {
                var cardInfo = Manager.CardAssemblyManager.GetLastCardInfo(cardID);
                Info.CardCompnentInfo.targetCardTexture.GetComponent<Image>().sprite =cardInfo.GetCardSprite();
                Info.CardCompnentInfo.targetCardName.GetComponent<Text>().text = cardInfo.translateName;
                Info.CardCompnentInfo.targetCardTag.GetComponent<Text>().text = cardInfo.cardTag;
                Info.CardCompnentInfo.targetCardAbility.GetComponent<Text>().text = cardInfo.translateAbility;
            }
        }
        internal static void ChangeFocusCamp()
        {
            Debug.LogError("已注释");
            var campInfo = Info.CampInfo.GetCampInfo(Info.CardCompnentInfo.focusCamp);
            Info.CardCompnentInfo.targetCardTexture.GetComponent<Image>().sprite = Sprite.Create(campInfo.campTex, new Rect(0, 0, campInfo.campTex.width, campInfo.campTex.height), Vector2.zero);
            Info.CardCompnentInfo.targetCardName.GetComponent<Text>().text = campInfo.campName;
            Info.CardCompnentInfo.targetCardTag.GetComponent<Text>().text ="强化 反制";
            Info.CardCompnentInfo.targetCardAbility.GetComponent<Text>().text = campInfo.campIntroduction;
        }
    }
}