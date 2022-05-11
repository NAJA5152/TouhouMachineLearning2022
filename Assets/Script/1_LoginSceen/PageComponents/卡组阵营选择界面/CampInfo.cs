using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    public class CampInfo
    {
        public static List<SingleCampInfo> campInfos = new List<SingleCampInfo>();
        public static void Init()
        {
            //从配置文件载入相应阵营信息，先搞个假的
            campInfos.Clear();
            campInfos.Add(new SingleCampInfo(Camp.Taoism, "道教", "没有东西的空架子哦",Info.PageCompnentInfo.instance.TaoismTex));
            campInfos.Add(new SingleCampInfo(Camp.science, "科学", "没有东西的空架子哦", Info.PageCompnentInfo.instance.scienceTex));
            campInfos.Add(new SingleCampInfo(Camp.Buddhism, "佛教", "没有东西的空架子哦", Info.PageCompnentInfo.instance.BuddhismTex));
            campInfos.Add(new SingleCampInfo(Camp.Shintoism, "神道教", "没有东西的空架子哦", Info.PageCompnentInfo.instance.ShintoismTex));
        }
        public static SingleCampInfo GetCampInfo(Camp camp) => campInfos.FirstOrDefault(info => info.camp == camp);
        public class SingleCampInfo
        {
            public Camp camp;
            public string campName;
            public string campIntroduction;
            public Texture2D campTex;
            public SingleCampInfo(Camp camp, string campName, string campIntroduction, Texture2D campTex)
            {
                this.camp = camp;
                this.campName = campName;
                this.campIntroduction = campIntroduction;
                this.campTex = campTex;
            }
        }
    }
}