using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Thread;
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
        public static void Load()
        {
            if (Application.isEditor)
            {
                Info.DialogueInfo.DialogueModels = File.ReadAllText(@"Assets\GameResources\GameData\Story.json").ToObject<List<DialogueModel>>();
            }
            else
            {
                Info.DialogueInfo.DialogueModels = AssetBundleCommand.Load<TextAsset>("GameData", "Story").text.ToObject<List<DialogueModel>>();
            }
        }
        //重新加载新的剧情文件，并调整对话指针
        public static void ReLoad()
        {
            Load();
            DialogueInfo.currnetDialogueModel = Info.DialogueInfo.DialogueModels.FirstOrDefault(model => model.Tag == $"{DialogueInfo.StageTag}-{DialogueInfo.StageRank}");
            if (DialogueInfo.currnetDialogueModel != null)
            {
                int newDialogueCount = Info.DialogueInfo.currnetDialogueModel.Operations.Count;
                DialogueInfo.CurrentPoint = Mathf.Min(DialogueInfo.CurrentPoint, newDialogueCount);
                RunNextOperations();
            }
            else
            {
                Debug.LogError("剧情重载失败");
            }
        }

        public static async Task Play(string stageTag, int stageRank)
        {

            DialogueInfo.CurrentPoint = 0;
            DialogueInfo.StageTag = stageTag;
            DialogueInfo.StageRank = stageRank;
            DialogueInfo.isLeftCharaActive = false;
            DialogueInfo.isRightCharaActive = false;
            Transform left = Info.DialogueInfo.instance.left.transform;
            for (int i = 0; i < left.childCount; i++)
            {
                left.GetChild(i).gameObject.SetActive(false);
            }
            Transform right = Info.DialogueInfo.instance.right.transform;
            for (int i = 0; i < right.childCount; i++)
            {
                right.GetChild(i).gameObject.SetActive(false);
            }

            DialogueInfo.instance.dialogueCanvas.SetActive(true);
            Debug.LogError("对话组件开启");
            //加载剧情文本
            DialogueInfo.currnetDialogueModel = DialogueInfo.DialogueModels.FirstOrDefault(model => model.Tag == $"{stageTag}-{stageRank}");
            if (DialogueInfo.currnetDialogueModel != null)
            {
                await RunNextOperations();
            }
            else
            {
                Debug.LogError("剧情加载失败");
                DialogueInfo.instance.dialogueCanvas.SetActive(false);
            }
        }
        /// <summary>
        /// 传入播放剧情参数，若当前剧情与玩家节点相等则解锁下个阶段剧情
        /// </summary>
        /// <param name="stageTag"></param>
        /// <param name="stageRank"></param>
        /// <returns></returns>
        public static async Task UnlockAsync(string stageTag, int stageRank)
        {
            if (AgainstInfo.onlineUserInfo.GetStage(stageTag) == stageRank)
            {
                Debug.LogWarning("玩家进度更新至" + stageTag + "-" + stageRank + 1);
                await AgainstInfo.onlineUserInfo.UpdateUserStateAsync(stageTag, stageRank + 1);
            }
        }
        public static async Task RunNextOperations()
        {
            Debug.LogWarning($"当前对话{Info.DialogueInfo.CurrentPoint}，总对话{ Info.DialogueInfo.currnetDialogueModel.Operations.Count}");
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
                        //到选择选项时停止自动跳转
                        DialogueInfo.IsJump = false;
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

                        //暂停界面，直到玩家选择完毕
                        while (!DialogueInfo.IsSelectOver)
                        {
                            await Task.Delay(100);
                        }
                        DialogueInfo.IsSelectOver = false;
                        await RunNextOperations();
                    }
                    else
                    {
                        if (command.StartsWith("rename"))
                        {
                            //到命名选项时停止自动跳转
                            DialogueInfo.IsJump = false;
                            await NoticeCommand.ShowAsync("请输入你的名字", NotifyBoardMode.Input, inputAction: async (name) =>
                            {
                                await Info.AgainstInfo.onlineUserInfo.UpdateName(name);
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
                        await RunNextOperations();
                    }
                }
                else//是对话的情况
                {
                    if (currentOperations.Position == "左侧")
                    {
                        //Info.DialogueInfo.instance.left.GetComponent<Image>().sprite = AssetBundleCommand.Load<Texture2D>("Charactar", currentOperations.Chara).ToSprite();
                        //Info.DialogueInfo.instance.left.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        //Info.DialogueInfo.instance.right.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1);

                        //关闭所有左侧live2d
                        Transform left = Info.DialogueInfo.instance.left.transform;
                        for (int i = 0; i < left.childCount; i++)
                        {
                            left.GetChild(i).gameObject.SetActive(false);
                        }
                        //先判断上个激活的立绘是否存在且是右侧，如果是则变灰移回原位或者关闭
                        if (Info.DialogueInfo.targetLive2dChara != null && Info.DialogueInfo.isRightCharaActive)
                        {
                            Debug.Log("关闭右侧立绘");
                            Transform target = Info.DialogueInfo.targetLive2dChara;
                            _ = CustomThread.TimerAsync(0.2f, process =>
                            {
                                target.localPosition = new Vector3((1 - process) * -100, 0, 0);
                                target.GetComponent<Live2dManager>().ToActive(1 - process);
                            });
                            Info.DialogueInfo.isRightCharaActive = false;
                        }
                        //获取左侧立绘，没激活则激活
                        Info.DialogueInfo.targetLive2dChara = Info.DialogueInfo.instance.left.transform.Find(currentOperations.Chara);
                        Info.DialogueInfo.targetLive2dChara.gameObject.SetActive(true);
                        if (!Info.DialogueInfo.isLeftCharaActive)
                        {
                            Debug.Log("打开左侧立绘");
                            Transform target = Info.DialogueInfo.targetLive2dChara;
                            _ = CustomThread.TimerAsync(0.5f, process =>
                            {
                                target.localPosition = new Vector3(process * 100, 0, 0);
                                target.GetComponent<Live2dManager>().ToActive(process);
                            });
                            Info.DialogueInfo.isLeftCharaActive = true;
                        }
                    }
                    else if (currentOperations.Position == "右侧")
                    {
                        //Info.DialogueInfo.instance.right.GetComponent<Image>().sprite = AssetBundleCommand.Load<Texture2D>("Charactar", currentOperations.Chara).ToSprite();
                        //Info.DialogueInfo.instance.left.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f, 1);
                        //Info.DialogueInfo.instance.right.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        Transform right = Info.DialogueInfo.instance.right.transform;
                        for (int i = 0; i < right.childCount; i++)
                        {
                            right.GetChild(i).gameObject.SetActive(false);
                        }
                        //先判断上个激活的立绘是否存在且是左侧，如果是则变灰移回原位
                        if (Info.DialogueInfo.targetLive2dChara != null && Info.DialogueInfo.isLeftCharaActive)
                        {
                            Debug.Log("关闭左侧立绘");
                            Transform target = Info.DialogueInfo.targetLive2dChara;
                            _ = CustomThread.TimerAsync(0.2f, process =>
                            {
                                target.localPosition = new Vector3((1 - process) * 100, 0, 0);
                                target.GetComponent<Live2dManager>().ToActive(1 - process);
                            });
                            Info.DialogueInfo.isLeftCharaActive = false;
                        }
                        //获取右侧立绘，没激活则激活
                        Info.DialogueInfo.targetLive2dChara = Info.DialogueInfo.instance.right.transform.Find(currentOperations.Chara);
                        Info.DialogueInfo.targetLive2dChara.gameObject.SetActive(true);
                        if (!Info.DialogueInfo.isRightCharaActive)
                        {
                            Debug.Log("打开右侧立绘");
                            Transform target = Info.DialogueInfo.targetLive2dChara;
                            _ = CustomThread.TimerAsync(0.5f, process =>
                            {
                                target.localPosition = new Vector3(process * -100, 0, 0);
                                target.GetComponent<Live2dManager>().ToActive(process);
                            });
                            Info.DialogueInfo.isRightCharaActive = true;
                        }
                    }
                    if (currentOperations.Face != "")
                    {
                        Info.DialogueInfo.targetLive2dChara.GetComponent<Live2dManager>().Play(currentOperations.Face);
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
                    DialogueInfo.IsShowNextText = false;
                    while (!DialogueInfo.IsShowNextText || DialogueInfo.IsJump)
                    {
                        await Task.Delay(100);
                    }
                    await RunNextOperations();
                }

            }
            else//读取完毕
            {
                Info.DialogueInfo.instance.dialogueCanvas.SetActive(false);
                Debug.LogError("对话组件关闭");
            };
        }
    }
}