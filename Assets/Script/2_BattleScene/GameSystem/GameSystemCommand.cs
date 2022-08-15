using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class GameSystemCommand
    {
        /// <summary>
        /// 通知类型效果触发
        /// 会向对战中所有卡牌依次通知触发 xx前、xx时、xx后效果
        /// 一般由系统控制流程时触发，如回合开始时、小局开始时等
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task TriggerNotice(Event e)
        {
            foreach (var card in AgainstInfo.cardSet.CardList)
            {
                await Trigger(card, e[card][TriggerTime.Before]);
            }
            foreach (var card in AgainstInfo.cardSet.CardList)
            {
                await Trigger(card, e[card][TriggerTime.When]);
            }
            foreach (var card in AgainstInfo.cardSet.CardList)
            {
                await Trigger(card, e[card][TriggerTime.After]);
            }
        }

        /// <summary>
        /// 广播类型效果触发
        /// 在触发xx效果时
        /// 会向其他对战中其他卡牌广播通知"xx牌触发xx效果"卡牌，方便进行连锁判定
        /// 一般由卡牌效果触发，如打出部署时、部署时、死亡时等
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task TriggerBroadcast(Event e)
        {

            if (e.targetCards.Any())
            {
                //遍历所有触发对象，对每个对象目标外的全体对象广播xx效果前效果
                foreach (var targetCard in e.targetCards)
                {
                    foreach (var card in AgainstInfo.cardSet.CardList.Where(card => card != targetCard))
                    {
                        await Trigger(card, e[card][TriggerTime.Before]);
                    }
                }
                //如果以同时方式触发效果，则进行同时触发，并等待所有连锁效果触发完成后再继续触发xx效果后的效果
                if (e.triggerMeanWhile)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var card in e.targetCards)
                    {
                        tasks.Add(Trigger(card, e[card][TriggerTime.When]));
                    }
                    await Task.WhenAll(tasks.ToArray());
                }
                //如果以依次方式触发效果，则目标xx效果执行并等待所有连锁效果触发完成再触发下一个目标的xx效果，全部触发万抽后触发xx效果后的效果
                else
                {
                    foreach (var card in e.targetCards)
                    {
                        Debug.LogWarning($"当前执行 {e.triggerType}:{e.targetCards.IndexOf(card) + 1} {e.targetCards.Count}");
                        await Trigger(card, e[card][TriggerTime.When]);
                    }
                }
                //遍历所有触发对象，对每个对象目标外的全体对象广播XX之后效果
                foreach (var targetCard in e.targetCards)
                {
                    foreach (var card in AgainstInfo.cardSet.CardList.Where(card => card != targetCard))
                    {
                        await Trigger(card, e[targetCard][TriggerTime.After]);
                    }
                }
            }
            else
            {
                Debug.LogWarning("无生效目标");
            }

        }
        static async Task Trigger(Card NoticeCard, Event e)
        {
            foreach (var ability in NoticeCard.cardAbility[e.triggerTime][e.triggerType])
            {
                await ability(e);
            }
        }
    }
}