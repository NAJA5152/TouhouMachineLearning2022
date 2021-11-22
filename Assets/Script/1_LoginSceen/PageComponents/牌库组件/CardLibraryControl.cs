using TouhouMachineLearningSummary.Manager;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class CardLibraryControl : MonoBehaviour
    {
        public void FocusLibraryCardOnMenu(GameObject cardModel)
        {
            int cardID = CardAssemblyManager.lastMultiCardInfos[Info.CardCompnentInfo.libraryCardModels.IndexOf(cardModel)].cardID;
            Control.GameUI.IntroductionControl.focusCardID = cardID;
        }
        public void FocusDeckCardOnMenu(GameObject cardModel)
        {
            int cardID = CardAssemblyManager.lastMultiCardInfos[Info.CardCompnentInfo.deckCardModels.IndexOf(cardModel)].cardID;
            Control.GameUI.IntroductionControl.focusCardID = cardID;
        }
        public void LostFocusCardOnMenu()
        {
            Control.GameUI.IntroductionControl.focusCardID = 0;
        }
    }
}

