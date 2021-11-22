using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class AudioControl : MonoBehaviour
    {
        public GameEnum.GameAudioType audioType;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnMouseDown() => Command.AudioCommand.PlayAsync(audioType);
    }
}
