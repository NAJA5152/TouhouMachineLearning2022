using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Control
{
    public class BookTagControl : MonoBehaviour
    {
        public Text TagText;
        public MenuState toMenuState;
        public void Init(string tagText)
        {
            TagText.text = string.Join("\n", tagText.ToCharArray());
        }
        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Command.MenuStateCommand.ChangeToMainPage(toMenuState);
            }
        }
    }
}