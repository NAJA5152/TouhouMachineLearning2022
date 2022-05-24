using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //默认点开初始
            SelectStep(0);
        }
        public static void SelectStep(int step)
        {
            var targetStageInfo= Info.PageCompnentInfo.currentSelectStages[step];
            //控制左侧领袖信息的显示
            //控制下侧关卡信息的显示
        }
    }
}


