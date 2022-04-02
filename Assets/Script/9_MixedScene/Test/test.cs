using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using static UnityEngine.Mathf;

namespace TouhouMachineLearningSummary.Test
{
    public class test : MonoBehaviour
    {
        [Button("测试")]
        public void A() => _ = B();
        [Button("测试")]
        public async Task B()
        {
            Debug.Log("1");
            await Task.Delay(1000);
            Debug.Log("2");
            await Task.Delay(3000);
            Debug.Log("3");
        }

        [ShowInInspector]
        public CardSet cardSet => AgainstInfo.cardSet;
        [ShowInInspector]
        public CardSet FiltercardSet;
        [ShowInInspector]
        public Texture2D tex;
        public string text;
        [Button("截图")]
        public void CaptureScreen(string name) => ScreenCapture.CaptureScreenshot(@"Assets/Art/Scene/" + name + ".png");
        [Button("上传记录")]
        public void test0()
        {

            AgainstInfo.summary.Explort();
            AgainstInfo.summary.Show();
        }
        [Button("下载拥有记录")]
        public void test1()
        {
            var result = Command.NetCommand.DownloadOwnerAgentSummaryAsync("0", 0, 100);
            Debug.Log(result.ToJson());
        }
        [Button("下载所有记录")]
        public void test2()
        {
            var result = Command.NetCommand.DownloadAllAgentSummaryAsync(0, 100);
            Debug.Log(result.ToJson());
        }
        [Button("跳转到指定回合")]
        public void Jump(int totalTurnRank, bool isOnTheOffensive, bool isPlayer1)
        {
            //先加载
            Info.AgainstInfo.summary = AgainstSummaryManager.Load("");
            Debug.LogWarning(Info.AgainstInfo.summary.ToJson());
            AgainstInfo.IsPlayer1 = isPlayer1;
            //然后跳转
            _ = AgainstInfo.summary.JumpToTurnAsync(totalTurnRank, isOnTheOffensive);
        }
        [Button]
        public void useLanguage(GameEnum.Language language)
        {
            TranslateManager.currentLanguage = language.ToString();
        }
        [Button("翻译标签")]
        public void ShowText(GameEnum.CardTag tag)
        {
            text = tag.ToString().Translation();
        }
        [Button("查找集合")]

        public void filterCardSet(List<GameEnum.CardTag> tags)
        {
            FiltercardSet = cardSet[tags.ToArray()];
        }
        private void OnGUI()
        {
            //if (GUI.Button(new Rect(0, 0, 100, 50), "翻页模拟效果"))
            //{
            //    Command.BookCommand.SimulateFilpPage(true);
            //}
            //if (GUI.Button(new Rect(0, 100, 100, 50), "新版本效果"))
            //{
            //    AgainstManager.Init();
            //    //AgainstManager.SetPvPMode(false);
            //    //AgainstManager.SetTurnFirst(FirstTurn.PlayerFirst);
            //    AgainstManager.AutoSetPlayerInfo(new PlayerInfo(
            //             "NPC", "gezi", "yaya", "",
            //            new List<CardDeck>
            //            {
            //            new CardDeck("gezi", 10001, new List<int>
            //            {
            //                20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,
            //            })
            //            })
            //        );
            //    AgainstManager.AutoSetOpponentInfo(
            //       new PlayerInfo(
            //             "NPC", "gezi", "yaya", "",
            //            new List<CardDeck>
            //            {
            //            new CardDeck("gezi", 10001, new List<int>
            //            {
            //                20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,20002,
            //            })
            //            })
            //       );
            //    //AgainstManager.SetCardVersion("");
            //    Debug.Log("对战start");
            //    AgainstManager.AutoStart();
            //}
            //if (GUI.Button(new Rect(0, 150, 100, 50), "启动回放模式"))
            //{
            //    AgainstManager.Init();
            //    AgainstManager.AutoStart();
            //}
        }
    }
}

