using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Control
{
    public class BookTagControl : MonoBehaviour
    {
        public Text ForntTagText;
        public Text BackTagText;
        public MenuState toMenuState;
        public void Init(string tagText)
        {
            ForntTagText.text = string.Join("\n", tagText.ToCharArray());
            BackTagText.text = string.Join("\n", tagText.ToCharArray());
        }
        private void OnMouseDown() => Command.MenuStateCommand.ChangeToMainPage(toMenuState);
    }
}