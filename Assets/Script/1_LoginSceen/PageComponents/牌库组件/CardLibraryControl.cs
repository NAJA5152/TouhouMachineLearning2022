using TouhouMachineLearningSummary.Manager;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class CardLibraryControl : MonoBehaviour
    {
        public void FocusLibraryCardOnMenu(GameObject cardModel) => CardAbilityPopupManager.focusCardID = CardAssemblyManager.lastMultiCardInfos[Info.PageCompnentInfo.libraryCardModels.IndexOf(cardModel)].cardID;
        public void FocusDeckCardOnMenu(GameObject cardModel) => CardAbilityPopupManager.focusCardID = CardAssemblyManager.lastMultiCardInfos[Info.PageCompnentInfo.deckCardModels.IndexOf(cardModel)].cardID;
        public void LostFocusCardOnMenu() => CardAbilityPopupManager.focusCardID = 0;
    }
}