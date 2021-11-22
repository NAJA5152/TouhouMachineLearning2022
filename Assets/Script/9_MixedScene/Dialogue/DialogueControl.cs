using TouhouMachineLearningSummary.Command.Dialogue;
using TouhouMachineLearningSummary.Info.Dialogue;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control.Dialogue
{
    /// <summary>
    /// 剧情演出控制器
    /// </summary>
    public class DialogueControl : MonoBehaviour
    {
        public GameObject DialogueUI;
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!DialogueUI.activeSelf)
                {
                    DialogueUI.SetActive(true);
                    DialogueCommand.Play(1, 1);
                }
                else
                {
                    DialgueInfo.instance.IsNext = true;
                }
            }
        }
    }
}