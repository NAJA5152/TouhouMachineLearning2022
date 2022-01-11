using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class AudioControl : MonoBehaviour
    {
        public GameEnum.GameAudioType audioType;
        private void OnMouseDown() => Command.AudioCommand.PlayAsync(audioType);
    }
}
