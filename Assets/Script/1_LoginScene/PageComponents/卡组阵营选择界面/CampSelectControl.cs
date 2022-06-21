using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    public class CampSelectControl : MonoBehaviour
    {
        public void Reback() => Command.MenuStateCommand.RebackStare();
        public void SelectCamp(GameObject model) => Command.CampSelectCommand.SelectCamp(model);
        public void CreatDeck() => Command.DeckBoardCommand.CreatDeck();
        public void FocusCamp(GameObject campModel) => Command.CampSelectCommand.FocusCamp(campModel);
        public void LostFocusCamp() => Command.CampSelectCommand.LostFocusCamp();
    }
}