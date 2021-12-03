using TouhouMachineLearningSummary.Command.Dialogue;
using TouhouMachineLearningSummary.Info;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control.Dialogue
{
    /// <summary>
    /// 剧情演出控制器
    /// </summary>
    public class DialogueControl : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!Info.DialgueInfo.instance.DialogueCanvas.activeSelf)
                {
                    Info.DialgueInfo.instance.DialogueCanvas.SetActive(true);
                }
                else
                {
                    DialgueInfo.instance.RunNextOperations = true;
                }
            }
        }
    }
}