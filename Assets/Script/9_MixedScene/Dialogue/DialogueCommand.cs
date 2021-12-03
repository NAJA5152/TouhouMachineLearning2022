using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Test;
using UnityEngine;
using static TouhouMachineLearningSummary.Info.DialgueInfo;

namespace TouhouMachineLearningSummary.Command.Dialogue
{
    /// <summary>
    /// 剧情演出指令
    /// </summary>
    public class DialogueCommand
    {
        public static void Load() => Info.DialgueInfo.DialogueModels = File.ReadAllText(@"Assets\Resources\CardData\CardData-Single.json").ToObject<List<DialogueModel>>();
        public static void Play(string tag)
        {
            CurrentPoint = 0;
            Info.DialgueInfo.instance.DialogueCanvas.SetActive(true);
            var targetDialogue = Info.DialgueInfo.DialogueModels.FirstOrDefault(model => model.Tag == tag);
            if (targetDialogue != null)
            {
                int maxCount = targetDialogue.Operations.Count();
            }
            else
            {
                Debug.LogError("剧情加载失败");
            }
        }
        public static void End()
        {
            Info.DialgueInfo.instance.DialogueCanvas.SetActive(false);
        }
        public static void RunNextOperations()
        {
            //如果没执行完则运行下一个指令，否则直接结束
            if (Info.DialgueInfo.CurrentPoint < Info.DialgueInfo.currnetDialogueModel.Operations.Count)
            {
                var currentOperations = Info.DialgueInfo.currnetDialogueModel.Operations[Info.DialgueInfo.CurrentPoint];
                if (currentOperations.Chara=="指令")
                {
                    //解析指令

                    RunNextOperations();
                }
                Info.DialgueInfo.CurrentPoint++;
            }
            else
            {
                End();
            }
            if (Info.DialgueInfo.currnetDialogueModel.Operations.)
            {

            }
            targetDialogue.Operations.ForEach(operations =>
            {
                if (operations.)
                {

                }
            });
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
                while (!instance.RunNextOperations)
                {
                    Debug.Log("yaya");
                }
            });
            instance.RunNextOperations = false;
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