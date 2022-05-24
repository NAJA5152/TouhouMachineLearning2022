using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TouhouMachineLearningSummary.Manager;

namespace TouhouMachineLearningSummary.Command
{
    internal class StageCommand
    {
        public static void SelectStage(string Stage)
        {
            //获得标签对应的所有阶段的关卡信息
            Info.PageCompnentInfo.currentSelectStages = Stage.TranslationStageText();
            //获得玩家的线上进度
            int rank = Info.AgainstInfo.onlineUserInfo.GetStage(Stage);
            //控制右侧阶段信息的显示
            var content = Info.PageCompnentInfo.Instance.stageModel.transform.parent;
            int stageStepCurrentCount = content.childCount;
            int stageStepMaxCount = Info.PageCompnentInfo.currentSelectStages.Count;
            var stageModel = Info.PageCompnentInfo.Instance.stageModel;

            for (int i = stageStepCurrentCount; i < stageStepMaxCount; i++)
            {
                UnityEngine.Object.Instantiate(stageModel, content);
            }
            //生成对应数量的小关

            //前n位修改名称和可见性
            for (int i = 0; i < stageStepCurrentCount; i++)
            {
                if (i < stageStepMaxCount)
                {
                    content.GetChild(i).gameObject.SetActive(true);
                    content.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = Info.PageCompnentInfo.currentSelectStages[i].stageName;
                }
                else
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
            }
            //默认点开初始
            SelectStep(0);
        }
        public static void SelectStep(int step)
        {
            var targetStageInfo = Info.PageCompnentInfo.currentSelectStages[step];
            //控制左侧领袖信息的显示
            Info.PageCompnentInfo.Instance.leaderName.text = targetStageInfo.stageOpLeaderName;
            Info.PageCompnentInfo.Instance.leaderNick.text = targetStageInfo.leaderNick;
            Info.PageCompnentInfo.Instance.leaderIntroduction.text = targetStageInfo.stageOpIntroduction;
            //控制下侧关卡信息的显示
            Info.PageCompnentInfo.Instance.stageIntroduction.text = targetStageInfo.stageOpIntroduction;
        }
    }
}


