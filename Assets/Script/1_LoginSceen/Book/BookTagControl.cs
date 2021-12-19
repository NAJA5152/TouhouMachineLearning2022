using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Control
{
    public class BookTagControl : MonoBehaviour
    {
        int targetIndex;
        public Text ForntTagText;
        public Text BackTagText;
        public MenuState toMenuState;
        public void Init(int targetIndex, string tagText)
        {
            ForntTagText.text = string.Join("\n", tagText.ToCharArray());
            BackTagText.text = string.Join("\n", tagText.ToCharArray());
            this.targetIndex = targetIndex;
        }
        private void OnMouseDown()
        {
            Command.MenuStateCommand.ChangeToMainPage(toMenuState);
        }
    }
}