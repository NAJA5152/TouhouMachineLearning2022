using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.UI;
using static TouhouMachineLearningSummary.Info.CampInfo;

namespace TouhouMachineLearningSummary.Command
{
    class CampSelectCommand
    {
        public static void InitCamp()
        {
            Info.PageCompnentInfo.selectCardModels.ForEach(Object.Destroy);
            Info.PageCompnentInfo.selectCardModels.Clear();
            //初始化信息来源
            Info.CampInfo.campInfos.Clear();
            Info.CampInfo.campInfos.Add(new(Camp.Taoism, "道教", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.TaoismTex));
            Info.CampInfo.campInfos.Add(new(Camp.science, "科学", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.scienceTex));
            Info.CampInfo.campInfos.Add(new(Camp.Buddhism, "佛教", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.BuddhismTex));
            Info.CampInfo.campInfos.Add(new(Camp.Shintoism, "神道教", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.ShintoismTex));
            Info.CampInfo.campInfos.Add(new(Camp.Neutral, "中立", "请选择一个阵营", Info.PageCompnentInfo.Instance.NeutralTex));

            //根据实际阵营数量来生成模型
            for (int i = 0; i < Info.CampInfo.campInfos.Count - 1; i++)
            {
                var newCampModel = Object.Instantiate(Info.PageCompnentInfo.Instance.CampModel, Info.PageCompnentInfo.Instance.modelContent.transform);
                Info.PageCompnentInfo.selectCardModels.Add(newCampModel);
            }
            //设置每个卡牌的属性
            for (int i = 0; i < Info.CampInfo.campInfos.Count - 1; i++)
            {
                //卡牌信息集合
                var info = Info.CampInfo.campInfos[i];
                //卡牌对应场景模型
                var s = Info.PageCompnentInfo.selectCardModels;
                var newCardModel = Info.PageCompnentInfo.selectCardModels[i];
                newCardModel.name = info.campName;
                newCardModel.transform.localScale = Info.PageCompnentInfo.Instance.CampModel.transform.localScale;
                newCardModel.transform.GetChild(0).GetComponent<Image>().sprite = info.campTex;
                newCardModel.transform.GetChild(2).GetComponent<Text>().text = info.campName;
                newCardModel.SetActive(true);
            }
        }
        public static void InitLeader()
        {
            Info.PageCompnentInfo.selectCardModels.ForEach(Object.Destroy);
            Info.PageCompnentInfo.selectCardModels.Clear();
            Info.CampInfo.leaderInfos = Manager.CardAssemblyManager
                    .LastMultiCardInfos
                    .Where(card => card.cardRank == CardRank.Leader)
                    .Where(card => card.cardCamp == Info.PageCompnentInfo.selectCamp || card.cardCamp == Camp.Neutral)
                    .ToList();

            //根据实际领袖数量来生成模型
            for (int i = 0; i < leaderInfos.Count(); i++)
            {
                var newCampModel = Object.Instantiate(Info.PageCompnentInfo.Instance.LeaderModel, Info.PageCompnentInfo.Instance.modelContent.transform);
                Info.PageCompnentInfo.selectCardModels.Add(newCampModel);
            }
            //设置每个卡牌的属性
            for (int i = 0; i < leaderInfos.Count(); i++)
            {
                //卡牌信息集合
                var info = leaderInfos[i];
                //卡牌对应场景模型
                var newCardModel = Info.PageCompnentInfo.selectCardModels[i];
                newCardModel.name = info.TranslateName;
                newCardModel.transform.localScale = Info.PageCompnentInfo.Instance.CampModel.transform.localScale;
                newCardModel.transform.GetChild(0).GetComponent<Image>().sprite = info.GetCardSprite();
                newCardModel.transform.GetChild(2).GetComponent<Text>().text = info.TranslateName;
                newCardModel.SetActive(true);
            }
        }
        //左侧显焦点阵营
        public static void FocusCamp(GameObject campModel)
        {
            Info.PageCompnentInfo.isCampIntroduction = true;
            int selectRank = Info.PageCompnentInfo.selectCardModels.IndexOf(campModel);
            Info.PageCompnentInfo.focusCamp = Info.CampInfo.campInfos[selectRank].camp;
            Command.CardDetailCommand.ChangeFocusCamp();
        }
        //确定所选阵营
        public static void SelectCamp(GameObject campModel)
        {
            int selectRank = Info.PageCompnentInfo.selectCardModels.IndexOf(campModel);
            Info.PageCompnentInfo.selectCamp = Info.CampInfo.campInfos[selectRank].camp;
        }
        //左侧变更会所选阵营
        public static void LostFocusCamp()
        {
            Info.PageCompnentInfo.focusCamp = Info.PageCompnentInfo.selectCamp;
            Command.CardDetailCommand.ChangeFocusCamp();
        }

        //左侧显焦点领袖
        public static void FocusLeader(GameObject campModel)
        {
            Info.PageCompnentInfo.isCampIntroduction = true;
            int selectRank = Info.PageCompnentInfo.selectCardModels.IndexOf(campModel);
            Info.PageCompnentInfo.focusLeaderID = Info.CampInfo.leaderInfos[selectRank].cardID;
            Command.CardDetailCommand.ChangeFocusLeader();
        }
        //选择对应阵营的领袖
        public static void SelectLeader(GameObject campModel)
        {
            int selectRank = Info.PageCompnentInfo.selectCardModels.IndexOf(campModel);
            Info.PageCompnentInfo.selectLeaderID = Info.CampInfo.leaderInfos[selectRank].cardID;
        }
        //左侧变更会所选领袖
        public static void LostFocusLeader()
        {
            Info.PageCompnentInfo.focusLeaderID = Info.PageCompnentInfo.selectLeaderID;
            Command.CardDetailCommand.ChangeFocusLeader();
        }
    }
}
