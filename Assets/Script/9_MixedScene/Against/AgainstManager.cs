using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine.SceneManagement;
using System.Linq;

namespace TouhouMachineLearningSummary.Manager
{
    /// <summary>
    /// 配置对战的环境
    /// </summary>
    public partial class AgainstManager
    {
        //对战加载的卡牌数据版本
        //static string LoadAssemblyVerision = "";
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


            AutoSetPlayerInfo(null);
            AutoSetOpponentInfo(null);
            //LoadAssemblyVerision = "";
            Info.AgainstInfo.isReplayMode = false;
            Info.AgainstInfo.summary = null;
        }


        /////////////////////////////////////////////////////////////////////自定义配置///////////////////////////////////////////////////////////
        /// <summary>
        /// 设置特殊对战规则
        /// </summary>
        /// <param name="rules"></param>
        public static void SetRules(params bool[] rules) => Console.WriteLine("待定");
        /// <summary>
        /// 设置回放模式
        /// </summary>
        /// <param name="rules"></param>
        public static async void ReplayStart(AgainstSummaryManager summary)
        {
            Info.AgainstInfo.isReplayMode = true;
            Info.AgainstInfo.summary = summary;
            //LoadAssemblyVerision = Info.AgainstInfo.summary.AssemblyVerision;
            Info.AgainstInfo.IsPlayer1 = true;//待完善，默认1号玩家
            Info.AgainstInfo.currentUserInfo = summary.Player1Info;
            Info.AgainstInfo.currentOpponentInfo = summary.Player2Info;
            Info.AgainstInfo.IsMyTurn = Info.AgainstInfo.IsPlayer1;
            await CardAssemblyManager.SetCurrentAssembly(Info.AgainstInfo.summary.AssemblyVerision);
            SceneManager.LoadSceneAsync("2_BattleScene");

        }
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

        //设置对局类型
        //public static void SetPvPMode(bool isPvPMode) => Info.AgainstInfo.IsPVP = isPvPMode;
        //设置初始胜利小局数
        //public static void SetScore(int Score1, int Score2)
        //{
        //    //Info.AgainstInfo.isPVP = isPvPMode;
        //}
        /////////////////////////////////////////////////////////////////////默认配置///////////////////////////////////////////////////////////
        /// <summary>
        /// 设置客户端扮演的角色，玩家1为第一回合先攻，玩家二为后攻
        /// </summary>
        /// <param name="xx"></param>
        public static void AutoSetRole(bool isPlayer1) => Info.AgainstInfo.IsPlayer1 = isPlayer1;

        //设置我方卡组
        public static void AutoSetPlayerInfo(PlayerInfo userInfo) => Info.AgainstInfo.currentUserInfo = userInfo;
        ////设置对方卡组
        public static void AutoSetOpponentInfo(PlayerInfo opponentInfo) => Info.AgainstInfo.currentOpponentInfo = opponentInfo;
        public static async Task AutoStart()
        {
            Info.AgainstInfo.IsMyTurn = Info.AgainstInfo.IsPlayer1;
            await CardAssemblyManager.SetCurrentAssembly("");
            if (Info.AgainstInfo.userDeck == null)
            {
                AutoSetPlayerInfo(defaultPlayerInfo);
            }
            if (Info.AgainstInfo.opponentDeck == null)
            {
                AutoSetOpponentInfo(defaultPlayerInfo);
            }
            await Manager.CameraViewManager.MoveToViewAsync(3);
            SceneManager.LoadSceneAsync("2_BattleScene");
        }
    }
}