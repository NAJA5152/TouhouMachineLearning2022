using System.Collections.Generic;
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
        public Text UserName;
        public Text Password;
        async void Start()
        {
            Manager.TakeLoopManager.Init();
            Command.Network.NetCommand.Init();
            await CardAssemblyManager.SetCurrentAssembly(""); //加载卡牌配置数据
            //UserLogin();//自动登录
            //TestBattleAsync();
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
                                CameraViewControl.MoveToInitView();
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
        public void UserRegister()
        {
            try
            {
                Command.Network.NetCommand.RegisterAsync(UserName.text, Password.text);
            }
            catch (System.Exception e)
            {

                Debug.LogException(e);
            }
        }

        public void UserLogin()
        {
            try
            {
                _ = Command.Network.NetCommand.LoginAsync(UserName.text, Password.text);

            }
            catch (System.Exception e)
            {

                Debug.LogException(e);
            }
        }

        public async Task TestBattleAsync()
        {
            AgainstManager.Init();
            AgainstManager.SetPvPMode(false);
            AgainstManager.SetTurnFirst(FirstTurn.PlayerFirst);
            AgainstManager.SetPlayerInfo(new PlayerInfo(
                    "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("gezi", 10001, new List<int>
                        {
                            20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,
                        })
                    })
                );
            AgainstManager.SetOpponentInfo(
               new PlayerInfo(
                    "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("gezi", 10001, new List<int>
                        {
                            20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,
                        })
                    })
               );
            Debug.Log("对战start");
            await AgainstManager.Start();
        }

        public void UserServerSelect() => Info.AgainstInfo.isHostNetMode = !Info.AgainstInfo.isHostNetMode;

        private void OnApplicationQuit() => Command.Network.NetCommand.Dispose();
    }
}