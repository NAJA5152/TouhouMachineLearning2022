using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine.SceneManagement;

namespace TouhouMachineLearningSummary.Manager
{
    /// <summary>
    /// 配置对战的环境
    /// </summary>
    public partial class AgainstManager
    {
        // Start is called before the first frame update
        string LoadAssemblyDate = "";
        static PlayerInfo defaultPlayerInfo => new PlayerInfo("默认NPC", "无名", "",
            new List<CardDeck>()
            {
             new CardDeck("默认卡组", 10001, new List<int>
                        {
                             10015, 10016, 10012,10013,
                             10014, 10015, 10016,10002,10008,10004,
                             10007, 10006,10008,
                             10004, 10012, 10010,
                             10011,10012, 10014,
                             10007,10006, 10016,
                             10008, 10014,10005,
                        })
            });
        public static void Init()
        {
            SetPlayerInfo(null);
            SetOpponentInfo(null);
            Info.AgainstInfo.isReplayMode = false;
            Info.AgainstInfo.summary = new AgainstSummaryManager();
        }
        public static async Task Start()
        {
            await CardAssemblyManager.SetCurrentAssembly(Info.AgainstInfo.summary.assemblyVerision);
            if (Info.AgainstInfo.userDeck == null)
            {
                SetPlayerInfo(defaultPlayerInfo);
            }
            if (Info.AgainstInfo.opponentDeck == null)
            {
                SetOpponentInfo(defaultPlayerInfo);
            }
            SceneManager.LoadSceneAsync(1);
        }
        //设置我方卡组
        public static void SetPlayerInfo(PlayerInfo userInfo) => Info.AgainstInfo.currentUserInfo = userInfo;
        ////设置对方卡组
        public static void SetOpponentInfo(PlayerInfo opponentInfo) => Info.AgainstInfo.currentOpponentInfo = opponentInfo;
        //设置先手方
        public static void SetTurnFirst(FirstTurn firstTurn)
        {
            switch (firstTurn)
            {
                case FirstTurn.PlayerFirst: Info.AgainstInfo.isMyTurn = true; break;
                case FirstTurn.OpponentFirst: Info.AgainstInfo.isMyTurn = false; break;
                case FirstTurn.Random: Info.AgainstInfo.isMyTurn = Info.AgainstInfo.isPlayer1; break;
                default: break;
            }
        }
        //设置对局类型
        public static void SetPvPMode(bool isPvPMode) => Info.AgainstInfo.isPVP = isPvPMode;
        //设置初始胜利小局数
        public static void SetScore(int Score1, int Score2)
        {
            //Info.AgainstInfo.isPVP = isPvPMode;
        }
        public static void SetReplayMode(int summaryID)
        {
            Info.AgainstInfo.isReplayMode = true;
            Info.AgainstInfo.summary = AgainstSummaryManager.Load(summaryID);
        }
        //等待加载指定版本卡牌配置与代码
        public static async Task SetCardVersion(string date)
        {
            Info.AgainstInfo.summary.assemblyVerision = date;
        }
    }
}