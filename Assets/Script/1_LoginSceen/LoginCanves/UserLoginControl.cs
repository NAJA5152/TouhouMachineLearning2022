using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
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
        bool IsAleardyLogin { get; set; } = false;
        public static bool IsEnterRoom { get; set; } = false;
        async void Start()
        {
            await AssetBundleCommand.Init();
            
            _ = CardAssemblyManager.SetCurrentAssembly(""); //加载卡牌配置数据
            DialogueCommand.Load();
            TaskLoopManager.Init();
            await CameraViewManager.MoveToViewAsync(0, true);
            //初始化场景物体状态，如果已登录，则进入到指定页，否则进入初始场景
            await BookCommand.InitAsync(IsAleardyLogin);
            if (!IsAleardyLogin)
            {

                //UserLogin();//自动登录
                await Task.Delay(1000);
                //await TestReplayAsync();
                //await TestBattleAsync();
            }
        }
        private void Update()
        {
            //临时，显示当前的ui路径
            if (Input.GetKeyDown(KeyCode.S))
            {
                MenuStateCommand.ShowStare();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (MenuStateCommand.GetCurrentStateRank())
                {
                    case (1)://如果当前状态为登录前，则关闭程序
                        {
                            _ = NoticeCommand.ShowAsync("退出游戏？",
                            okAction: async () =>
                            {
                                Application.Quit();
                            });
                            break;
                        }
                    case (2)://如果当前状态为第主级页面，则询问并退出登录
                        {
                            _ = NoticeCommand.ShowAsync("退出登录",
                            okAction: async () =>
                            {
                                IsAleardyLogin = false;
                                await Manager.CameraViewManager.MoveToViewAsync(0);
                                MenuStateCommand.RebackStare();
                                MenuStateCommand.ChangeToMainPage(MenuState.Login);
                                await BookCommand.SetCoverStateAsync(false);
                                UiInfo.loginCanvas.SetActive(true);
                            });
                            break;
                        }
                    default://如果当前状态为多级页面，则返回上级（个别页面需要询问）
                        {
                            //如果是组牌模式，则询问是否返回上一页，否则直接返回上一页
                            if (MenuStateCommand.GetCurrentState() == MenuState.CardListChange)
                            {
                                _ = NoticeCommand.ShowAsync("不保存卡组？",
                                okAction: async () =>
                                {
                                    MenuStateCommand.RebackStare();
                                });
                            }
                            else
                            {
                                MenuStateCommand.RebackStare();
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
                int result = await NetCommand.RegisterAsync(Account.text, Password.text);
                switch (result)
                {
                    case (1): await NoticeCommand.ShowAsync("注册成功", NotifyBoardMode.Ok); break;
                    case (-1): await NoticeCommand.ShowAsync("账号已存在", NotifyBoardMode.Ok); break;
                    default: await NoticeCommand.ShowAsync("注册发生异常", NotifyBoardMode.Ok); break;
                }
            }
            catch (Exception e) { Debug.LogException(e); }
        }

        public async void UserLogin()
        {
            try
            {
                bool isSuccessLogin = await NetCommand.LoginAsync(Account.text, Password.text);
                if (isSuccessLogin)
                {
                    PlayerInfo.UserState onlineUserState = AgainstInfo.onlineUserInfo.OnlineUserState;
                    if (onlineUserState.Step == 0 && onlineUserState.Rank == 0)
                    {
                        DialogueCommand.Play("0-0");
                        await AgainstInfo.onlineUserInfo.UpdateUserStateAsync(0, 1);
                    }
                    Manager.UserInfoManager.Refresh();
                    await BookCommand.InitToOpenStateAsync();
                    _ = NetCommand.CheckRoomAsync(Account.text, Password.text);
                }
                else
                {
                    await NoticeCommand.ShowAsync("账号或密码错误，请重试", NotifyBoardMode.Ok);
                }
            }
            catch (Exception e) { Debug.LogException(e); }
        }
        public async Task TestReplayAsync()
        {
            var summarys = await NetCommand.DownloadOwnerAgentSummaryAsync(AgainstInfo.onlineUserInfo.Account, 0, 20);
            Manager.AgainstManager.ReplayStart(summarys.Last());
        }
        public async Task TestBattleAsync()
        {

            await Manager.CameraViewManager.MoveToViewAsync(2);
            MenuStateCommand.AddState(MenuState.WaitForBattle);
            BookCommand.SimulateFilpPage(true);//开始翻书

            AgainstModeType targetAgainstMode = AgainstModeType.Story;
            PlayerInfo sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            PlayerInfo virtualOpponentInfo = new PlayerInfo(
                  "NPC", "神秘的妖怪", "yaya", "",
                  new List<CardDeck>
                  {
                        new CardDeck("萌新的第一套卡组", 2000001, new List<int>
                        {
                            2001001,2001002,2001003,2001004,
                            2002001,2002002,2002003,2002004,2002005,2002006,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                        })
                  });

            _ = NoticeCommand.ShowAsync("少女排队中~", NotifyBoardMode.Cancel, cancelAction: async () =>
            {
                BookCommand.SimulateFilpPage(false);//开始翻书
                await Task.Delay(2000);
                await Manager.CameraViewManager.MoveToViewAsync(1);
                MenuStateCommand.RebackStare();
                await NetCommand.LeaveHoldOnList(AgainstModeType.Story, sampleUserInfo.Account);
            });
            //配置对战模式
            Manager.AgainstManager.Init();
            Manager.AgainstManager.SetAgainstMode(targetAgainstMode);
            //开始排队
            await NetCommand.JoinHoldOnList(targetAgainstMode, sampleUserInfo, virtualOpponentInfo);
        }
        public void UserServerSelect() => AgainstInfo.isHostNetMode = !Info.AgainstInfo.isHostNetMode;
        private void OnApplicationQuit() => NetCommand.Dispose();
        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "回放测试"))
            {
                TestReplayAsync();
            }
            if (GUI.Button(new Rect(0, 100, 100, 50), "对战测试"))
            {
                TestBattleAsync();
            }
        }
    }
}