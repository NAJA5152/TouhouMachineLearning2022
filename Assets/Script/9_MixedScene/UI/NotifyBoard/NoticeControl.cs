using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class NoticeControl : MonoBehaviour
    {
        public void Ok() => _ = Command.GameUI.NoticeCommand.OkAsync();
        public void Cancel() => _ = Command.GameUI.NoticeCommand.CancaelAsync();
        public void Input() => _ = Command.GameUI.NoticeCommand.InputAsync();
        private void Start() => _ = Command.GameUI.NoticeCommand.CloseAsync();
    }
}