using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Control;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

/// <summary>
/// 高层api
/// </summary>
namespace TouhouMachineLearningSummary.GameSystem
{
    /// <summary>
    /// 改变卡牌点数的相关机制
    /// </summary>
    public class PointSystem
    {
        //增益
        public static async Task Gain(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Gain]);
        //伤害
        public static async Task Hurt(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Hurt]);
        //治愈
        public static async Task Cure(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Cure]);
        //重置
        public static async Task Reset(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Reset]);
        //强化
        public static async Task Strengthen(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Strengthen]);
        //削弱
        public static async Task Weak(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Weak]);
        //摧毁
        public static async Task Destory(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Destory]);
    }
    /// <summary>
    /// 转移卡牌位置、所属区域的相关机制
    /// </summary>
    public class TransSystem
    {
        public static async Task DrawCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Draw]);
        public static async Task PlayCard(TriggerInfo triggerInfo, bool isAnsy = true)
        {
            if (triggerInfo.targetCard != null)
            {
                await Command.CardCommand.PlayCard(triggerInfo.targetCard, isAnsy);
                await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Play]);
            }
        }
        // 回手
        public static async Task RecycleCard(TriggerInfo triggerInfo)
        {

        }
        // 复活
        public static async Task ReviveCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Revive]);
        // 移动卡牌
        public static async Task MoveCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Move]);
        // 移动至墓地(不触发移动效果)
        public static async Task MoveToGrave(TriggerInfo triggerInfo)
        {
            await Task.Delay(500);
            await Command.CardCommand.MoveToGrave(triggerInfo.targetCard);
        }
        // 部署
        public static async Task DeployCard(TriggerInfo triggerInfo)
        {
            //部署效果特殊处理，先执行部署行为再触发部署效果
            if (triggerInfo.targetCards.Any() &&AgainstInfo. SelectRegion != null)
            {
                await Command.CardCommand.DeployCard(triggerInfo.targetCard);
            }
            await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Deploy]);
        }
        public static async Task SummonCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Summon]);
        // 放逐
        public static async Task BanishCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Banish]);
        public static async Task DisCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Discard]);
        public static async Task DeadCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Dead]);

    }
    public class StateSystem
    {
        public static async Task SealCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Seal]);
        public static async Task CloseCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Close]);
        public static async Task ScoutCard(TriggerInfo triggerInfo) => await CardEffectStackControl.TriggerBroadcast(triggerInfo[TriggerType.Scout]);
    }
    public class FieldSystem
    {
        //直接改变，不触发机制
        public static async Task Increase(TriggerInfo triggerInfo, CardField cardField)
        {
            foreach (var targetCard in triggerInfo.targetCards)
            {

                if (targetCard[cardField] != 0)
                {
                    targetCard[cardField]++;
                }
            }
        }
        //临时方案
        public static async Task Change(TriggerInfo triggerInfo)
        {
            foreach (var targetCard in triggerInfo.targetCards)
            {
                await CardEffectStackControl.TriggerBroadcast(triggerInfo[targetCard][TriggerType.FieldChange]);
            }
        }
        public static int GetField(Card card, CardField cardField) => card[cardField];
        public static int GetTwoSideField(Card card, CardField cardField) => (card.LeftCard == null || card. LeftCard[CardState.Seal] ? 0 : card.LeftCard[cardField]) + (card.RightCard == null || card.RightCard[CardState.Seal] ? 0 : card.RightCard[cardField]);
    }
    /// <summary>
    /// 选择单位、区域、场景属性的相关机制
    /// </summary>
    public class SelectSystem
    {
        public static async Task SelectProperty() => await Command.StateCommand.WaitForSelectProperty();
        public static async Task SelectLocation(Card triggerCard, BattleRegion region, Territory territory) => await Command.StateCommand.WaitForSelectLocation(triggerCard, region, territory);
        public static async Task SelectBoardCard(Card triggerCard, List<Card> cards, CardBoardMode Mode = CardBoardMode.Select, int num = 1)
        {
            if (Mode == CardBoardMode.Select)
            {
                Mode = AgainstInfo.isMyTurn ? CardBoardMode.Select : CardBoardMode.ShowOnly;
            }
            await Command.StateCommand.WaitForSelectBoardCard(triggerCard,cards, Mode, num);
        }
        public static async Task SelectBoardCard(Card triggerCard, List<int> cardIDs) => await Command.StateCommand.WaitForSelectBoardCard(triggerCard,cardIDs);

        public static async Task SelectUnite(Card triggerCard, List<Card> filterCards, int num, bool isAuto = false) => await Command.StateCommand.WaitForSelecUnit(triggerCard, filterCards, num, isAuto);

        public static async Task SelectRegion(Card triggerCard, GameRegion regionType = GameRegion.Battle, Territory territory = Territory.All) => await Command.StateCommand.WaitForSelectRegion(triggerCard,regionType, territory);

    }
    //由系统触发的状态机制
    public class ProcessSystem
    {
        public static async Task WhenTurnStart() => await CardEffectStackControl.TriggerBroadcast(new TriggerInfo(null).SetTargetCard(AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.TurnStart]);
        public static async Task WhenTurnEnd() => await CardEffectStackControl.TriggerBroadcast(new TriggerInfo(null).SetTargetCard(AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.TurnEnd]);
        public static async Task WhenRoundStart() => await CardEffectStackControl.TriggerBroadcast(new TriggerInfo(null).SetTargetCard(AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.RoundStart]);
        public static async Task WhenRoundEnd() => await CardEffectStackControl.TriggerBroadcast(new TriggerInfo(null).SetTargetCard(AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.RoundEnd]);
    }
}