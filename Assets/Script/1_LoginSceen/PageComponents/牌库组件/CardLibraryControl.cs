using TouhouMachineLearningSummary.Manager;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class CardLibraryControl : MonoBehaviour
    {
        public void FocusLibraryCardOnMenu(GameObject cardModel) =>GameUI.IntroductionControl.focusCardID = CardAssemblyManager.lastMultiCardInfos[Info.CardCompnentInfo.libraryCardModels.IndexOf(cardModel)].cardID;
        public void FocusDeckCardOnMenu(GameObject cardModel) =>GameUI.IntroductionControl.focusCardID = CardAssemblyManager.lastMultiCardInfos[Info.CardCompnentInfo.deckCardModels.IndexOf(cardModel)].cardID;
        public void LostFocusCardOnMenu() => Control.GameUI.IntroductionControl.focusCardID = 0;
    }
}