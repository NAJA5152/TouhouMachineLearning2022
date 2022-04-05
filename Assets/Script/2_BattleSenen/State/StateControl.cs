using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Info;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class StateControl : MonoBehaviour
    {
        void Start() => _ = CreatAgainstProcess();
        public static async Task CreatAgainstProcess()
        {
            try
            {
                Manager.TaskLoopManager.Init();
                //如果位于跳转模式则直接跳过对局初始化阶段并在之后从对战记录初始化战场状态
                if (!AgainstInfo.isJumpMode){await StateCommand.AgainstStart();}
                while (true)
                {
                    //再联网对战情况下上传回合初始信息
                    Manager.AgainstSummaryManager.UploadRound();
                    //根据跳转的回合是否是第0回合（小局前置阶段）判断是否执行小局前抽卡操作
                    //在非跳转模式或者跳转目标为第0回合（小局前置阶段）时，会进入小局开始等待换牌阶段，否则直接略过
                    if (!AgainstInfo.isJumpMode || StateCommand.AgainstStateInit())
                    {
                        await StateCommand.RoundStart();
                        Debug.LogWarning("开始换牌");
                        await StateCommand.WaitForPlayerExchange();
                        Debug.LogWarning("结束换牌");
                        //await StateCommand.WaitForSelectProperty();
                    }
                    while (true)
                    {
                        await StateCommand.TurnStart();
                        Manager.AgainstSummaryManager.UploadStartPoint();
                        await StateCommand.WaitForPlayerOperation();
                        Manager.AgainstSummaryManager.UploadEndPoint();
                        if (Info.AgainstInfo.isBoothPass) { break; }
                        await StateCommand.TurnEnd();
                    }
                    await StateCommand.RoundEnd();
                    if (AgainstInfo.PlayerScore.P1Score == 2 || AgainstInfo.PlayerScore.P2Score == 2) { break; }
                    AgainstInfo.roundRank++;
                }
                await StateCommand.AgainstEnd();
            }
            catch (System.Exception ex) { Debug.LogError(ex.Message); }
        }
    }
}