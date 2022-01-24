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
        public static async Task Gain(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Gain]);
        //伤害
        public static async Task Hurt(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Hurt]);
        //治愈
        public static async Task Cure(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Cure]);
        //重置
        public static async Task Reset(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Reset]);
        //强化
        public static async Task Strengthen(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Strengthen]);
        //削弱
        public static async Task Weak(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Weak]);
        //摧毁
        public static async Task Destory(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Destory]);
    }
    /// <summary>
    /// 转移卡牌位置、所属区域的相关机制
    /// </summary>
    public class TransSystem
    {
        public static async Task DrawCard(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Draw]);
        public static async Task PlayCard(TriggerInfoModel triggerInfo, bool isAnsy = true)
        {
            if (triggerInfo.targetCard != null)
            {
                await Command.CardCommand.PlayCard(triggerInfo.targetCard, isAnsy);
                await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Play]);
            }
        }
        // 回手
        public static async Task RecycleCard(TriggerInfoModel triggerInfo)
        {

        }
        // 复活
        public static async Task ReviveCard(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Revive]);
        // 移动卡牌
        public static async Task MoveCard(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Move]);
        // 移动至墓地(不触发移动效果)
        public static async Task MoveToGrave(TriggerInfoModel triggerInfo)
        {
            await Task.Delay(500);
            await Command.CardCommand.MoveToGrave(triggerInfo.targetCard);
        }
        // 部署
        public static async Task DeployCard(TriggerInfoModel triggerInfo)
        {
            //部署效果特殊处理，先执行部署行为再触发部署效果
            if (triggerInfo.targetCards.Any() && AgainstInfo.SelectRowRank != -1)
            {
                await Command.CardCommand.DeployCard(triggerInfo.targetCard);
            }
            await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Deploy]);
        }
        public static async Task SummonCard(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Summon]);
        // 放逐
        public static async Task BanishCard(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Banish]);
        public static async Task DisCard(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Discard]);
        public static async Task DeadCard(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.Dead]);

    }
    public class StateSystem
    {
        public static async Task SetState(TriggerInfoModel triggerInfo)
        {
            //筛选触发目标，对不包含该状态的卡牌才会激活状态
            triggerInfo.targetCards = triggerInfo.targetCards.Where(card => card[triggerInfo.targetState]).ToList();
            await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.StateAdd]);
        }
        public static async Task ClearState(TriggerInfoModel triggerInfo)
        {
            //筛选触发目标，对包含该状态的卡牌才会清空状态
            triggerInfo.targetCards = triggerInfo.targetCards.Where(card => !card[triggerInfo.targetState]).ToList();
            await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.StateClear]);
        }
        public static async Task ChangeState(TriggerInfoModel triggerInfo)
        {
            List<Card> stateActivateCardList = triggerInfo.targetCards.Where(card => card[triggerInfo.targetState]).ToList();
            List<Card> stateUnActivateCardList = triggerInfo.targetCards.Where(card => !card[triggerInfo.targetState]).ToList();
            //设置所有状态未激活的为激活状态
            triggerInfo.targetCards = stateUnActivateCardList;
            await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.StateAdd]);
            //设置所有状态激活的为未激活状态
            triggerInfo.targetCards = stateActivateCardList;
            await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.StateClear]);
        }
    }
    public class FieldSystem
    {
        /// <summary>
        /// 直接设置字段值,不触发增加或减少衍生效果
        /// </summary>
        public static async Task SetField(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.FieldSet]);
        /// <summary>
        /// 设置字段值改变量，若为正数则触发字段增加衍生效果，若为正数则触发字段减少衍生效果
        /// </summary>
        public static async Task ChangeField(TriggerInfoModel triggerInfo) => await CardAbilityControl.TriggerBroadcast(triggerInfo[TriggerType.FieldChange]);
    }
    /// <summary>
    /// 选择单位、区域、场景属性的相关机制
    /// </summary>
    public class SelectSystem
    {
        public static async Task SelectProperty() => await Command.StateCommand.WaitForSelectProperty();
        public static async Task SelectLocation(Card triggerCard, Territory territory, BattleRegion region) => await Command.StateCommand.WaitForSelectLocation(triggerCard, territory, region);
        public static async Task SelectBoardCard(Card triggerCard, List<Card> cards, CardBoardMode Mode = CardBoardMode.Select, int num = 1)
        {
            //在对方视角时选择时为只读无法改变
            if (Mode == CardBoardMode.Select)
            {
                Mode = AgainstInfo.IsMyTurn ? CardBoardMode.Select : CardBoardMode.ShowOnly;
            }
            await Command.StateCommand.WaitForSelectBoardCard(triggerCard, cards, Mode, num);
        }
        public static async Task SelectBoardCard(Card triggerCard, List<int> cardIDs) => await Command.StateCommand.WaitForSelectBoardCard(triggerCard, cardIDs);

        public static async Task SelectUnite(Card triggerCard, List<Card> filterCards, int num, bool isAuto = false) => await Command.StateCommand.WaitForSelecUnit(triggerCard, filterCards, num, isAuto);

        public static async Task SelectRegion(Card triggerCard, Territory territory = Territory.All, GameRegion regionType = GameRegion.Battle) => await Command.StateCommand.WaitForSelectRegion(triggerCard, territory, regionType);

    }
    //由系统触发的状态机制
    public class ProcessSystem
    {
        public static async Task WhenTurnStart() => await CardAbilityControl.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.TurnStart]);
        public static async Task WhenTurnEnd() => await CardAbilityControl.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.TurnEnd]);
        public static async Task WhenRoundStart() => await CardAbilityControl.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.RoundStart]);
        public static async Task WhenRoundEnd() => await CardAbilityControl.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.RoundEnd]);
    }
    /// <summary>
    /// 获取游戏内对战信息的高层api接口
    /// </summary>
    public class InfoSystem
    {
        /// <summary>
        /// 获取对战内所有卡牌集合并可通过属性标签进行筛选
        /// </summary>
        public static CardSet AgainstCardSet => AgainstInfo.cardSet;
        public static List<Card> SelectUnits => AgainstInfo.SelectUnits;
        public static int SelectRowRank => AgainstInfo.SelectRowRank;
        public static List<Card> SelectRowCardList => AgainstInfo.SelectRowCardList;
        public static int SelectLocation => AgainstInfo.SelectRank;
        public static int GetField(Card card, CardField cardField) => card[cardField];
        public static int GetTwoSideField(Card card, CardField cardField) => (card.LeftCard == null || card.LeftCard[CardState.Seal] ? 0 : card.LeftCard[cardField]) + (card.RightCard == null || card.RightCard[CardState.Seal] ? 0 : card.RightCard[cardField]);
    }
}