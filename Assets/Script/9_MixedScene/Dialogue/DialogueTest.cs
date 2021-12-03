using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command.Dialogue;
using UnityEngine;
using static TouhouMachineLearningSummary.Info.DialgueInfo;

namespace TouhouMachineLearningSummary.Test
{
    public class DialogueTest : MonoBehaviour
    {
        public static DialogueTest Instance;
        private void Awake()
        {
            Instance = this;
        }
      
    }
}