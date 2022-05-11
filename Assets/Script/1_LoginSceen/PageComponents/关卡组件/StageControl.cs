using UnityEngine;

namespace TouhouMachineLearningSummary.Control
{
    public class StageControl:MonoBehaviour
    {
        //选择欲加载关卡
        public void SelectStage(string tag) => Command.StageCommand.SelectStage(tag);
        //选择欲加载关卡阶段
        public void SelectStep(GameObject stageButton) => Info.PageCompnentInfo.CurrentSelectStageTag=stageButton.name;
    }
}
