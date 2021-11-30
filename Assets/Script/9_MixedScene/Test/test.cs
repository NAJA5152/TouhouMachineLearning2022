using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using static UnityEngine.Mathf;

namespace TouhouMachineLearningSummary.Test
{
    public class test : MonoBehaviour
    {
        [ShowInInspector]
        public CardSet cardSet => AgainstInfo.cardSet;
        [ShowInInspector]
        public CardSet FiltercardSet;

        public string text;
        [Button("上传记录")]
        public void test0()
        {

            AgainstInfo.summary.Upload();
            AgainstInfo.summary.Explort();
            AgainstInfo.summary.Show();
        }
        [Button("下载记录")]
        public void test1()
        {
            var result = Command.Network.NetCommand.DownloadAgentSummaryAsync("0", 0, 100);
        }
        [Button("跳转到指定回合")]
        public void Jump(int totalTurnRank, bool isOnTheOffensive, bool isPlayer1)
        {
            //先加载
            Info.AgainstInfo.summary = AgainstSummaryManager.Load(1);
            Debug.LogWarning(Info.AgainstInfo.summary.ToJson());
            AgainstInfo.isPlayer1 = isPlayer1;
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
        public float a;
        public float b;
        public float c;
        private void Start()
        {

        }
        private void Update()
        {
            Matrix4x4 matrix = new Matrix4x4
                (
                new Vector4(Cos(a) * Cos(c) - Cos(b) * Sin(a) * Sin(c), -Cos(b) * Cos(c) * Sin(a) - Cos(a) * Sin(c), Sin(a) * Sin(b)),
                new Vector4(Cos(c) * Sin(a) + Cos(a) * Cos(b) * Sin(c), Cos(a) * Cos(b) * Cos(c) - Sin(a) * Sin(c), -Cos(a) * Sin(b)),
                new Vector4(Sin(b) * Sin(c), Cos(c) * Sin(b), Cos(b)),
                new Vector4(0, 0, 0, 0)
                );
            Debug.DrawLine(Vector3.zero, Vector3.right, Color.red);
            Debug.DrawLine(Vector3.zero, Vector3.up, Color.green);
            Debug.DrawLine(Vector3.zero, Vector3.forward, Color.blue);

            Debug.DrawLine(Vector3.zero, matrix * Vector3.right, Color.red);
            Debug.DrawLine(Vector3.zero, matrix * Vector3.up, Color.green);
            Debug.DrawLine(Vector3.zero, matrix * Vector3.forward, Color.blue);
            Debug.DrawLine(matrix *new Vector3(0,1,0), matrix * new Vector3(0, 1, 1), Color.white);
            Debug.DrawLine(matrix *new Vector3(0,1,0), matrix * new Vector3(1, 1, 0), Color.white);
            Debug.DrawLine(matrix *new Vector3(1,0,1), matrix * new Vector3(1, 1, 1), Color.white);
            Debug.DrawLine(matrix *new Vector3(0,0,1), matrix * new Vector3(1, 0, 1), Color.white);
            Debug.DrawLine(matrix *new Vector3(1,0,0), matrix * new Vector3(1, 0, 1), Color.white);
            Debug.DrawLine(matrix * new Vector3(0, 0, 1), matrix * new Vector3(0, 1, 1), Color.white);
            Debug.DrawLine(matrix * new Vector3(1, 0, 0), matrix * new Vector3(1, 1, 0), Color.white);
            Debug.DrawLine(matrix * new Vector3(0, 1, 1), matrix * new Vector3(1, 1, 1), Color.white);
            Debug.DrawLine(matrix * new Vector3(1, 1, 0), matrix * new Vector3(1, 1, 1), Color.white);

            if (Input.GetMouseButtonDown(1))
            {
                AgainstManager.Init();
                AgainstManager.SetReplayMode(11);
                AgainstManager.Start();
            }
        }
        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "旧版本效果"))
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
                AgainstManager.SetCardVersion("2021_10_14");
                Debug.Log("对战start");
                AgainstManager.Start();
            }
            if (GUI.Button(new Rect(0, 100, 100, 50), "新版本效果"))
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
                AgainstManager.SetCardVersion("");
                Debug.Log("对战start");
                AgainstManager.Start();
            }
            if (GUI.Button(new Rect(0, 150, 100, 50), "启动回放模式"))
            {
                AgainstManager.Init();
                AgainstManager.SetReplayMode(11);
                AgainstManager.Start();
            }
        }
    }
}

