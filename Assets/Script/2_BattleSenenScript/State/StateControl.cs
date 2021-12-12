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
                Manager.TaskLoopManager.Init() ;
                //如果位于跳转模式则直接跳过对局初始化阶段并在之后从对战记录初始化战场状态

                if (!AgainstInfo.isJumpMode)
                {
                    await StateCommand.AgainstStart();
                }
                for (; AgainstInfo.roundRank <= 3; AgainstInfo.roundRank++)
                {
                    AgainstInfo.summary.AddRound();
                    //根据跳转的回合是否是第0回合（小局前置阶段）判断是否执行小局前抽卡操作
                    //在非跳转模式或者跳转目标为第0回合（小局前置阶段）时，会进入小局开始等待换牌阶段，否则直接略过
                    if (!AgainstInfo.isJumpMode || StateCommand.AgainstStateInit())
                    {
                        await StateCommand.RoundStart();
                        await StateCommand.WaitForPlayerExchange();
                        //await StateCommand.WaitForSelectProperty();
                    }
                    while (true)
                    {
                        //在跳转到目标为非第0回合（小局前置阶段）时，会根据目标清空所有
                        //if (AgainstInfo.isJumpMode)
                        //{
                        //    StateCommand.AgainstStateInit();
                        //}
                        await StateCommand.TurnStart();
                        UnityEngine.Debug.LogWarning("计算双方起始点数差");
                        AgainstInfo.summary.AddStartPoint();
                        await StateCommand.WaitForPlayerOperation();
                        if (Info.AgainstInfo.isBoothPass) { break; }
                        UnityEngine.Debug.LogWarning("计算双方结束点数差");
                        AgainstInfo.summary.AddEndPoint();
                        await StateCommand.TurnEnd();
                    }
                    await StateCommand.RoundEnd();
                }
                await StateCommand.AgainstEnd();
                //Debug.Log("结束对局");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }

        }
    }
}
