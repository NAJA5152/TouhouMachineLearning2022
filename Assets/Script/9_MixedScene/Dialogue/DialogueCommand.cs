using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Test;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    /// <summary>
    /// 剧情演出指令
    /// </summary>
    public class DialogueCommand
    {
        //public static void Load() => Info.DialogueInfo.DialogueModels = File.ReadAllText(@"Assets\Resources\GameData\Story.json").ToObject<List<DialogueModel>>();
        public static void Load() => Info.DialogueInfo.DialogueModels = Resources.Load<TextAsset>("GameData/Story").text .ToObject<List<DialogueModel>>();
        public static void Play(string tag)
        {
            Info.DialogueInfo.CurrentPoint = 0;
            Info.DialogueInfo.instance.dialogueCanvas.SetActive(true);
            Debug.LogError("对话组件开启");
            Info.DialogueInfo.currnetDialogueModel = Info.DialogueInfo.DialogueModels.FirstOrDefault(model => model.Tag == tag);
            if (Info.DialogueInfo.currnetDialogueModel != null)
            {
                RunNextOperations();
            }
            else
            {
                Debug.LogError("剧情加载失败");
            }
        }
        public static void End()
        {
            Info.DialogueInfo.instance.dialogueCanvas.SetActive(false);
            Debug.LogError("对话组件关闭");
        }
        public static async void RunNextOperations()
        {
            //Load();
            //Info.DialogueInfo.currnetDialogueModel = Info.DialogueInfo.DialogueModels.FirstOrDefault(model => model.Tag == "1-1");
            Debug.LogError($"当前对话{Info.DialogueInfo.CurrentPoint}，总对话{ Info.DialogueInfo.currnetDialogueModel.Operations.Count}");
            //如果没执行完则运行下一个指令，否则直接结束
            if (Info.DialogueInfo.CurrentPoint < Info.DialogueInfo.currnetDialogueModel.Operations.Count)
            {
                var currentOperations = Info.DialogueInfo.currnetDialogueModel.Operations[Info.DialogueInfo.CurrentPoint];
                if (currentOperations.Chara == "指令")//是指令的情况
                {
                    //解析和执行指令
                    string command = currentOperations.Text["Ch"];
                    if (command.StartsWith("select"))
                    {
                        Info.DialogueInfo.instance.selectUi.SetActive(true);
                        var options = command.Replace("select:", "").Split('@').ToList();
                        for (int i = 0; i < 3; i++)
                        {
                            if (i < options.Count)
                            {
                                Info.DialogueInfo.instance.selectUi.transform.GetChild(i).gameObject.SetActive(true);
                                Info.DialogueInfo.instance.selectUi.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = options[i];
                            }
                            else
                            {
                                Info.DialogueInfo.instance.selectUi.transform.GetChild(i).gameObject.SetActive(false);

                            }
                        }
                    }
                    else
                    {
                        if (command.StartsWith("rename"))
                        {
                            await Command.GameUI.NoticeCommand.ShowAsync("请输入你的名字", NotifyBoardMode.Input, inputAction: async (name) =>
                            {
                                await Info.AgainstInfo.onlineUserInfo.UpdateName(name);
                                //await Info.AgainstInfo.onlineUserInfo.UpdateUserStateAsync(0, 1);
                            }, inputField: "村中人");
                        }
                        if (command.StartsWith("music"))
                        {
                            var music = command.Replace("music:", "");
                            Debug.LogError("播放音乐" + music);
                        }
                        if (command.StartsWith("backImage"))
                        {
                            var music = command.Replace("backImage:", "");
                            Debug.LogError("切换背景图片" + music);
                        }
                        Info.DialogueInfo.CurrentPoint++;
                        RunNextOperations();
                    }

                }
                else//是对话的情况
                {
                    if (currentOperations.Position == "左侧")
                    {
                        Info.DialogueInfo.instance.left.GetComponent<Image>().sprite = Resources.Load<Texture2D>("Chara\\" + currentOperations.Chara).ToSprite();
                        Info.DialogueInfo.instance.left.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        Info.DialogueInfo.instance.right.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1);

                    }
                    else if (currentOperations.Position == "右侧")
                    {
                        Info.DialogueInfo.instance.right.GetComponent<Image>().sprite = Resources.Load<Texture2D>("Chara\\" + currentOperations.Chara).ToSprite();
                        Info.DialogueInfo.instance.left.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1);
                        Info.DialogueInfo.instance.right.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                    Info.DialogueInfo.instance.name.text = currentOperations.Chara;
                    Info.DialogueInfo.instance.text.text = currentOperations.Text["Ch"];
                    Info.DialogueInfo.CurrentPoint++;

                    //if (currentOperations.Position== "左侧")
                    //{
                    //    Info.DialogueInfo.instance.Left.gameObject.transform.localScale /= 1.1f;
                    //}
                    //else if (currentOperations.Position == "右侧")
                    //{
                    //    Info.DialogueInfo.instance.Right.gameObject.transform.localScale /= 1.1f;
                    //}
                }

            }
            else//读取完毕
            {
                End();
            };
        }
    }
}