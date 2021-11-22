using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using UnityEngine;
namespace TouhouMachineLearningSummary.Control
{
    public class CardEffectStackControl : MonoBehaviour
    {
        //触发xx效果时，先对目标卡牌以外的卡牌触发在xx效果前对应效果，然后对所有目标同时触发xx效果，最后对目标卡牌以外的卡牌触发在xx效果后对应效果
        public static async Task TriggerBroadcast(TriggerInfo triggerInfo)
        {
            try
            {
                //遍历所有触发对象，对每个对象目标外的全体对象广播xx效果前效果
                foreach (var targetCard in triggerInfo.targetCards)
                {
                    foreach (var card in AgainstInfo.cardSet.CardList.Where(card => card != targetCard))
                    {
                        await Trigger(triggerInfo[card][TriggerTime.Before]);
                    }
                }
                //如果以同时方式触发效果，则进行同时触发，并等待所有连锁效果触发完成后再继续触发xx效果后的效果
                if (triggerInfo.triggerMeanWhile)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var card in triggerInfo.targetCards)
                    {
                        tasks.Add(Trigger(triggerInfo[card][TriggerTime.When]));
                    }

                    await Task.WhenAll(tasks.ToArray());
                }
                //如果以依次方式触发效果，则目标xx效果执行并等待所有连锁效果触发完成再触发下一个目标的xx效果，全部触发万抽后触发xx效果后的效果
                else
                {
                    foreach (var card in triggerInfo.targetCards)
                    {
                        await Trigger(triggerInfo[card][TriggerTime.When]);
                    }
                }
                //遍历所有触发对象，对每个对象目标外的全体对象广播XX之后效果
                foreach (var targetCard in triggerInfo.targetCards)
                {
                    foreach (var card in AgainstInfo.cardSet.CardList.Where(card => card != targetCard))
                    {
                        await Trigger(triggerInfo[card][TriggerTime.After]);
                    }
                }
            }
            catch (Exception e) { Debug.LogError("!!!!!!!!!!!!!!效果栈出现严重错误!!!!!!!!!!!!!!!!!!!!!/n" + e); }
        }
        // 触发卡牌的指定效果
        public static async Task Trigger(TriggerInfo triggerInfo)
        {
            //Debug.Log("触发" + triggerInfo.triggerTime + "-" + triggerInfo.triggerType);
            foreach (var ability in triggerInfo.targetCard.cardAbility[triggerInfo.triggerTime][triggerInfo.triggerType])
            {
                await ability(triggerInfo);
            }
            //Debug.Log("触发完毕" + triggerInfo.triggerTime + "-" + triggerInfo.triggerType);
        }
    }
}