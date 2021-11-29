using System.Threading.Tasks;
using TouhouMachineLearningSummary.Test.DialgueInfo;
using UnityEngine;
using static TouhouMachineLearningSummary.Info.Dialogue.DialgueInfo;

namespace TouhouMachineLearningSummary.Command.Dialogue
{
    /// <summary>
    /// 剧情演出指令
    /// </summary>
    public class DialogueCommand
    {
        public static void Play(int step, int rank)
        {
            foreach (var methond in typeof(DialogueTest).GetMethods())
            {
                foreach (Dial info in methond.GetCustomAttributes(typeof(Dial), false))
                {
                    if (info.step == step && info.rank == rank)
                    {
                        methond.Invoke(DialogueTest.Instance, new object[] { });
                    }
                }
            }
        }
        public static void voice(int v)
        {
            Debug.Log($"n播放音乐{v}");
        }
        public static async Task Say(string message, Chara chara, bool IsLeft = true, int FaceNum = 0)
        {
            Debug.Log($"{message}                 角色为{chara}，立绘位置在{(IsLeft ? "左边" : "右边")}");
            instance.Text.text = chara + ":" + message;
            if (IsLeft)
            {
                //DialgueInfos.Left.GetComponent<Live2d>().FaceRank = FaceNum;
                instance.Left.gameObject.transform.localScale *= 1.1f;
            }
            else
            {
                //DialgueInfos.Right.GetComponent<Live2d>().FaceRank = FaceNum;
                instance.Right.gameObject.transform.localScale *= 1.1f;
            }
            await Task.Run(() =>
            {
                while (!instance.IsNext)
                {
                    Debug.Log("yaya");
                }
            });
            instance.IsNext = false;
            if (IsLeft)
            {
                instance.Left.gameObject.transform.localScale /= 1.1f;
            }
            else
            {
                instance.Right.gameObject.transform.localScale /= 1.1f;
            }
        }
    }
}