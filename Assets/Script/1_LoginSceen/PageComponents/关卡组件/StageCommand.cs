using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouMachineLearningSummary.Command
{
    internal class StageCommand
    {
        //根据进度字典初始化map状态
        public void Init(string tag)
        {
            var stageInfos = Info.AgainstInfo.onlineUserInfo.Stage;


        }
        public void SelectStage(string tag)
        {
            int rank = Info.AgainstInfo.onlineUserInfo.GetStage(tag);
            //控制页面显示

        }
    }
}
