using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Control
{
    public class UserLoginControl : MonoBehaviour
    {
        public Text Account;
        public Text Password;

        bool IsAleardyLogin = false;
        public static bool IsEnterRoom = false;
        async void Start()
        {
            Manager.TaskLoopManager.Init();
            await Manager.CameraViewManager.MoveToViewAsync(0, true);
            //初始化场景物体状态，如果已登录，则进入到指定页，否则进入初始场景
            await Command.BookCommand.InitAsync(IsAleardyLogin);
            //
            if (!IsAleardyLogin)
            {
                Command.Network.NetCommand.Init();
                await CardAssemblyManager.SetCurrentAssembly(""); //加载卡牌配置数据
                UserLogin();//自动登录
                await Task.Delay(1000);
                await TestBattleAsync();
                //await Command.BookCommand.InitAsync();
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Command.MenuStateCommand.ShowStare();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (Command.MenuStateCommand.GetCurrentStateRank())
                {
                    case (1)://如果当前状态为登录前，则关闭程序
                        {
                            _ = Command.GameUI.NoticeCommand.ShowAsync("退出游戏？",
                            okAction: async () =>
                            {
                                Application.Quit();
                            });
                            break;
                        }
                    case (2)://如果当前状态为第主级页面，则询问并退出登录
                        {
                            _ = Command.GameUI.NoticeCommand.ShowAsync("退出登录",
                            okAction: async () =>
                            {
                                await CameraViewManager.MoveToViewAsync(0);
                                Command.MenuStateCommand.RebackStare();
                                Command.MenuStateCommand.ChangeToMainPage(MenuState.Login);
                                await Command.BookCommand.SetCoverStateAsync(false);
                                Info.GameUI.UiInfo.loginCanvas.SetActive(true);
                            }
                            );
                            break;
                        }
                    default://如果当前状态为多级页面，则返回上级（个别页面需要询问）
                        {
                            //如果是组牌模式，则询问是否返回上一页，否则直接返回上一页
                            if (Command.MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
                            {
                                _ = Command.GameUI.NoticeCommand.ShowAsync("不保存卡组？",
                                okAction: async () =>
                                {
                                    Command.MenuStateCommand.RebackStare();
                                });
                            }
                            else
                            {
                                Command.MenuStateCommand.RebackStare();
                            }
                            break;
                        }
                }
            }
        }
        public async void UserRegister()
        {
            try
            {
                int result = await Command.Network.NetCommand.RegisterAsync(Account.text, Password.text);
                switch (result)
                {
                    case (1): await Command.GameUI.NoticeCommand.ShowAsync("注册成功", NotifyBoardMode.Ok); break;
                    case (-1): await Command.GameUI.NoticeCommand.ShowAsync("账号已存在", NotifyBoardMode.Ok); break;
                    default: await Command.GameUI.NoticeCommand.ShowAsync("注册发生异常", NotifyBoardMode.Ok); break;
                }
            }
            catch (System.Exception e) { Debug.LogException(e); }
        }

        public async void UserLogin()
        {
            try
            {
                bool isSuccessLogin = await Command.Network.NetCommand.LoginAsync(Account.text, Password.text);
                if (isSuccessLogin)
                {
                    PlayerInfo.UserState onlineUserState = Info.AgainstInfo.onlineUserInfo.OnlineUserState;
                    if (onlineUserState.Step == 0 && onlineUserState.Rank == 0)
                    {
                        Command.DialogueCommand.Play("0-0");
                        await Info.AgainstInfo.onlineUserInfo.UpdateUserStateAsync(0, 1);
                    }
                    Manager.UserInfoManager.Refresh();
                    await Command.BookCommand.InitToOpenStateAsync();
                    _ = Command.Network.NetCommand.CheckRoomAsync(Account.text, Password.text);
                }
                else
                {
                    await Command.GameUI.NoticeCommand.ShowAsync("账号或密码错误，请重试", NotifyBoardMode.Ok);
                }
            }
            catch (System.Exception e) { Debug.LogException(e); }
        }
        public async Task TestBattleAsync()
        {
            await Manager.CameraViewManager.MoveToViewAsync(2);
            Command.MenuStateCommand.AddState(MenuState.WaitForBattle);
            Command.BookCommand.SimulateFilpPage(true);//开始翻书
            PlayerInfo userInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            _ = Command.GameUI.NoticeCommand.ShowAsync("排队中", NotifyBoardMode.Cancel, cancelAction: async () =>
            {
                Command.BookCommand.SimulateFilpPage(false);//开始翻书
                await Task.Delay(2000);
                await Manager.CameraViewManager.MoveToViewAsync(1);
                Command.MenuStateCommand.RebackStare();
                await Command.Network.NetCommand.LeaveHoldOnList(AgainstModeType.Story, userInfo.Account);

            });
            var virtualOpponentInfo = new PlayerInfo(
                  "NPC", "神秘的妖怪", "yaya", "",
                  new List<CardDeck>
                  {
                                new CardDeck("gezi", 20001, new List<int>
                                {
                                    20002,20003,20004,20005,
                                    20006,20007,20008,20009,20010,20011,
                                    20012,20013,20014,20015,20016,
                                    20012,20013,20014,20015,20016,
                                    20012,20013,20014,20015,20016,
                                })
                  });
            await Command.Network.NetCommand.JoinHoldOnList(AgainstModeType.Story, userInfo, virtualOpponentInfo);
            //(PlayerInfo opponentInfo, bool IsOnTheOffensive) = await Command.Network.NetCommand.JoinHoldOnList(AgainstModeType.Story, userInfo, virtualOpponentInfo);
        }
        public void UserServerSelect() => Info.AgainstInfo.isHostNetMode = !Info.AgainstInfo.isHostNetMode;
        private void OnApplicationQuit() => Command.Network.NetCommand.Dispose();
    }
}