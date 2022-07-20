using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
using UnityEngine.UI;
using static TouhouMachineLearningSummary.Info.CampInfo;

namespace TouhouMachineLearningSummary.Command
{
    class CampSelectCommand
    {
        public static void Init()
        {
            //生成对应领袖
            //Info.CardCompnentInfo.leaderCardInfos
            //初始化信息来源
            Info.CampInfo.campInfos.Clear();
            Info.CampInfo.campInfos.Add(new SingleCampInfo(Camp.Taoism, "道教", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.TaoismTex));
            Info.CampInfo.campInfos.Add(new SingleCampInfo(Camp.science, "科学", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.scienceTex));
            Info.CampInfo.campInfos.Add(new SingleCampInfo(Camp.Buddhism, "佛教", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.BuddhismTex));
            Info.CampInfo.campInfos.Add(new SingleCampInfo(Camp.Shintoism, "神道教", "没有东西的空架子哦", Info.PageCompnentInfo.Instance.ShintoismTex));
            //根据实际阵营数量来生成模型
            for (int i = 0; i < Info.CampInfo.campInfos.Count; i++)
            {
                var newCampModel = UnityEngine.Object.Instantiate(Info.PageCompnentInfo.Instance.CampModel, Info.PageCompnentInfo.Instance.campContent.transform);
                Info.PageCompnentInfo.campCardModels.Add(newCampModel);
            }
            //设置每个卡牌的属性

            for (int i = 0; i < Info.CampInfo.campInfos.Count; i++)
            {
                //卡牌信息集合
                var info = Info.CampInfo.campInfos[i];
                //卡牌对应场景模型
                var newCardModel = Info.PageCompnentInfo.campCardModels[i];
                newCardModel.name = info.campName;
                newCardModel.transform.localScale = Info.PageCompnentInfo.Instance.CampModel.transform.localScale;
                //Sprite cardTex = Sprite.Create(info.campTex, new Rect(0, 0, info.campTex.width, info.campTex.height), Vector2.zero);
                newCardModel.transform.GetChild(0).GetComponent<Image>().sprite = info.campTex;
                newCardModel.transform.GetChild(2).GetComponent<Text>().text = info.campName;
                newCardModel.SetActive(true);
            }
        }
        public static void SelectCamp(GameObject campModel)
        {
            int selectRank = Info.PageCompnentInfo.campCardModels.IndexOf(campModel);
            Info.PageCompnentInfo.targetCamp = Info.CampInfo.campInfos[selectRank].camp;
        }
        //选择对应阵营的领袖
        public static void SelectLeader(GameObject campModel)
        {
            int selectRank = Info.PageCompnentInfo.campCardModels.IndexOf(campModel);
            Info.PageCompnentInfo.targetCamp = Info.CampInfo.campInfos[selectRank].camp;
        }
        public static void FocusCamp(GameObject campModel)
        {
            Info.PageCompnentInfo.isCampIntroduction = true;
            int selectRank = Info.PageCompnentInfo.campCardModels.IndexOf(campModel);
            Info.PageCompnentInfo.focusCamp = Info.CampInfo.campInfos[selectRank].camp;
            Debug.Log("注释焦点阵营" + Info.PageCompnentInfo.focusCamp);
            //int cardID = Info.CardCompnentInfo.multiModeCards[Info.CardCompnentInfo.deckCardModels.IndexOf(cardModel)].cardId;
            //Control.GameUI.IntroductionControl.focusCardID = cardID;
        }
        public static void LostFocusCamp()
        {
            Info.PageCompnentInfo.focusCamp = Camp.Neutral;
            //Control.GameUI.IntroductionControl.focusCardID = 0;
        }
    }
}
