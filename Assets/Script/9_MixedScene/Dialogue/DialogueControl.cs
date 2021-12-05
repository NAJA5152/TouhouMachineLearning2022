using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Info;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control.Dialogue
{
    /// <summary>
    /// 剧情演出控制器
    /// </summary>
    public class DialogueControl : MonoBehaviour
    {
        void Awake() => DialogueCommand.Load();
        public void ShowNextText()
        {
            if (!Info.DialogueInfo.SelectMode)
            {
                DialogueCommand.RunNextOperations();
            }
        }
        public void ShowLastText()
        {
            Info.DialogueInfo.instance.selectUi.SetActive(false);
            Info.DialogueInfo.CurrentPoint = Mathf.Max(0, Info.DialogueInfo.CurrentPoint - 2);
            DialogueCommand.RunNextOperations();
        }
        public void SetBranch(int index)
        {
            Info.DialogueInfo.SelectBranch = index;
            Info.DialogueInfo.instance.selectUi.SetActive(false);
            Info.DialogueInfo.CurrentPoint++;
            DialogueCommand.RunNextOperations();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(100, 100, 100, 100), "播放剧情"))
            {
                DialogueCommand.Play("1-1");
            }
            if (GUI.Button(new Rect(200, 100, 100, 100), "上一句话"))
            {
                ShowLastText();
            }
        }
    }
}