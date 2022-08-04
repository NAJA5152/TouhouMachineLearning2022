using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        static PlayerInfo defaultPlayerInfo => new PlayerInfo("NPC", "默认NPC", "无名", "",
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
            //初始化对战信息
            Info.AgainstInfo.roundRank = 1;
            //小局回合信息
            Info.AgainstInfo.turnRank = 0;
            Info.AgainstInfo.totalTurnRank = 0;

            Info.AgainstInfo.isUpPass = false;
            Info.AgainstInfo.isDownPass = false;
            Info.AgainstInfo.IsReplayMode = false;
            Info.AgainstInfo.PlayerScore = (0, 0);
            Info.AgainstInfo.summary = null;
        }
        /////////////////////////////////////////////////////////////////////自定义配置///////////////////////////////////////////////////////////
        /// <summary>
        /// 设置特殊对战规则
        /// </summary>
        /// <param name="rules"></param>
        public static void SetRules(params bool[] rules) => Console.WriteLine("待定");

        /// <summary>
        /// 设置对战模式
        /// </summary>
        /// <param name="xx"></param>
        public static void SetAgainstMode(AgainstModeType mode)
        {
            if (mode == AgainstModeType.Story || mode == AgainstModeType.Practice)
            {
                Info.AgainstInfo.IsPVP = false;
            }

            if (mode == AgainstModeType.Casual || mode == AgainstModeType.Rank || mode == AgainstModeType.Arena)
            {
                Info.AgainstInfo.IsPVP = true;
            }
        }
        /// <summary>
        /// 设置先后手,0随机 1先手，2后手，只在单人模式有效
        /// </summary>
        /// <param name="xx"></param>
        public static void SetFirstMode(int mode) => Info.AgainstInfo.FirstMode = mode;
        /////////////////////////////////////////////////////////////////////默认配置///////////////////////////////////////////////////////////
        /// <summary>
        /// 以线上模式开始对局，会上传对战记录，可被观战，包括PVP和PVE
        /// 设置客户端扮演的角色，玩家1为第一回合先攻，玩家二为后攻
        /// 设置我方卡组
        /// 设置对方卡组
        /// </summary>
        /// <returns></returns>
        public static async Task OnlineStart(bool isPlayer1, bool isMyTurn = false, PlayerInfo userInfo = null, PlayerInfo opponentInfo = null)
        {
            Info.AgainstInfo.IsPlayer1 = isPlayer1;
            Info.AgainstInfo.currentUserInfo = (userInfo == null ? defaultPlayerInfo : userInfo);
            Info.AgainstInfo.currentOpponentInfo = (opponentInfo == null ? defaultPlayerInfo : opponentInfo);
            Info.AgainstInfo.IsMyTurn = isMyTurn;
            Info.AgainstInfo.CurrentCardScriptsVersion = "";
            await CardAssemblyManager.SetCurrentAssembly(Info.AgainstInfo.CurrentCardScriptsVersion);
            await Manager.CameraViewManager.MoveToViewAsync(3);
            SceneManager.LoadSceneAsync("2_BattleScene");
        }
        /// <summary>
        /// 以回放模式开始对局
        /// </summary>
        /// <param name="rules"></param>
        public static async void ReplayStart(AgainstSummaryManager summary)
        {
            Info.AgainstInfo.IsReplayMode = true;
            Info.AgainstInfo.summary = summary;
            //LoadAssemblyVerision = Info.AgainstInfo.summary.AssemblyVerision;
            Info.AgainstInfo.IsPlayer1 = true;//待完善，默认1号玩家
            Info.AgainstInfo.currentUserInfo = summary.Player1Info;
            Info.AgainstInfo.currentOpponentInfo = summary.Player2Info;
            Info.AgainstInfo.IsMyTurn = Info.AgainstInfo.IsPlayer1;
            Info.AgainstInfo.CurrentCardScriptsVersion = Info.AgainstInfo.summary.AssemblyVerision;
            await CardAssemblyManager.SetCurrentAssembly(Info.AgainstInfo.CurrentCardScriptsVersion);
            SceneManager.LoadSceneAsync("2_BattleScene");
        }
    }
}