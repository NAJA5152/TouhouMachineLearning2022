using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

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
        //设置
        public static async Task Set(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Set]);
        //增益
        public static async Task Gain(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Gain]);
        //伤害
        public static async Task Hurt(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Hurt]);
        //治愈
        public static async Task Cure(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Cure]);
        //重置
        public static async Task Reset(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Reset]);
        //强化
        public static async Task Strengthen(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Strengthen]);
        //削弱
        public static async Task Weak(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Weak]);
        //摧毁
        public static async Task Destory(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Destory]);
        //逆转
        public static async Task Reversal(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Reverse]);
        //增加
        public static async Task Increase(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Increase]);
        //减少
        public static async Task Decrease(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Decrease]);
    }
    /// <summary>
    /// 转移卡牌位置、所属区域的相关机制，共有战场区，手牌区，使用区，墓地区，牌组区 共五个区域
    /// </summary>
    public class TransferSystem
    {
        /// <summary>
        /// 卡牌直接出现在对应区域，仅限单位牌，不会触发联锁效果
        /// </summary>
        /// <param name="triggerInfo"></param>
        /// <returns></returns>
        public static async Task GenerateCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[CardCommand.GenerateCard(triggerInfo.targetCardId)][TriggerType.Generate]);
        /// <summary>
        /// 在战场区各个区域间移动卡牌(触发移动效果)
        /// </summary>
        public static async Task MoveCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Move]);

        /// <summary>
        /// 从卡组区移动至手牌区(不触发移动效果)
        /// </summary>
        public static async Task DrawCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Draw]);
        /// <summary>
        /// 回手，从战场区移动至手牌区（不触发移动效果)
        /// </summary>
        public static async Task RecycleCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Reverse]);

        /// <summary>
        /// 打出，从手牌区\战场区打出至使用区(不触发移动效果)
        /// </summary>
        public static async Task PlayCard(TriggerInfoModel triggerInfo, bool isAnsy = true)
        {
            if (triggerInfo.targetCard != null)
            {
                await Command.CardCommand.PlayCard(triggerInfo.targetCard, isAnsy);
                await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Play]);
            }
        }
        /// <summary>
        /// 复活，从墓地区移动至战场区(不触发移动效果)
        /// </summary>
        public static async Task ReviveCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Revive]);
        /// <summary>
        /// 部署，从使用区移动至战场区(不触发移动效果)
        /// </summary>
        public static async Task DeployCard(TriggerInfoModel triggerInfo)
        {
            //部署效果特殊处理，先执行部署行为再触发部署效果
            if (triggerInfo.targetCards.Any() && AgainstInfo.SelectRowRank != -1)
            {
                await Command.CardCommand.DeployCard(triggerInfo.targetCard);
            }
            await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Deploy]);
        }
        /// <summary>
        /// 召唤，从牌组区移动至战场区(不触发移动效果)
        /// </summary>
        public static async Task SummonCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Summon]);
       
        /// <summary>
        /// 弃牌，从手牌区移动至墓地(不触发移动效果)
        /// </summary>
        public static async Task DisCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Discard]);
        /// <summary>
        /// 死亡，从战场区移动至墓地(不触发移动效果)
        /// </summary>
        public static async Task DeadCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Dead]);

        /// <summary>
        /// 间隙，从对局区移除至场外(不触发移动效果)
        /// </summary>
        public static async Task BanishCard(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.Banish]);

        /// <summary>
        ///  直接移动至卡组(不触发连锁效果)
        /// </summary>
        public static async Task MoveToDeck(Card targetCard, int index=0,bool isRandom=true) => await Command.CardCommand.MoveToDeck(targetCard, index, isRandom);
        /// <summary>
        /// 直接移动至墓地(不触发连锁效果)
        /// </summary>
        public static async Task MoveToGrave(Card targetCard) => await Command.CardCommand.MoveToGrave(targetCard);
    }
    public class StateSystem
    {
        public static async Task SetState(TriggerInfoModel triggerInfo)
        {
            //筛选触发目标，对不包含该状态的卡牌才会激活状态
            //triggerInfo.targetCards = triggerInfo.targetCards.Where(card => card[triggerInfo.targetState]).ToList();
            await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.StateAdd]);
        }
        public static async Task ClearState(TriggerInfoModel triggerInfo)
        {
            //筛选触发目标，对包含该状态的卡牌才会清空状态
            triggerInfo.targetCards = triggerInfo.targetCards.Where(card => card[triggerInfo.targetState]).ToList();
            await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.StateClear]);
        }
        public static async Task ChangeState(TriggerInfoModel triggerInfo)
        {
            List<Card> stateActivateCardList = triggerInfo.targetCards.Where(card => card[triggerInfo.targetState]).ToList();
            List<Card> stateUnActivateCardList = triggerInfo.targetCards.Where(card => !card[triggerInfo.targetState]).ToList();
            //设置所有状态未激活的为激活状态
            triggerInfo.targetCards = stateUnActivateCardList;
            await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.StateAdd]);
            //设置所有状态激活的为未激活状态
            triggerInfo.targetCards = stateActivateCardList;
            await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.StateClear]);
        }
    }
    public class FieldSystem
    {
        /// <summary>
        /// 直接设置字段值,不触发增加或减少衍生效果
        /// </summary>
        public static async Task SetField(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.FieldSet]);
        /// <summary>
        /// 设置字段值改变量，若为正数则触发字段增加衍生效果，若为正数则触发字段减少衍生效果
        /// </summary>
        public static async Task ChangeField(TriggerInfoModel triggerInfo) => await GameSystemCommand.TriggerBroadcast(triggerInfo[TriggerType.FieldChange]);
    }
    /// <summary>
    /// 异变系统
    /// </summary>
    public class VariationSystem
    {
        /// <summary>
        /// 设置当前异变情况
        /// </summary>
        public static async Task SetVariation(VariationType variationType) => Info.AgainstInfo.VariationType = variationType;
        /// <summary>
        /// 清空当前异变
        /// </summary>
        public static async Task ClearVariation(VariationType variationType) => Info.AgainstInfo.VariationType = VariationType.None;
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
    /// <summary>
    /// 由系统触发的流程控制事件，不会由卡牌触发
    /// </summary>

    public class ProcessSystem
    {
        public static async Task WhenTurnStart() => await GameSystemCommand.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.TurnStart]);
        public static async Task WhenTurnEnd() => await GameSystemCommand.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList)[TriggerTime.When][TriggerType.TurnEnd]);
        public static async Task WhenRoundStart() => await GameSystemCommand.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.RoundStart]);
        public static async Task WhenRoundEnd() => await GameSystemCommand.TriggerBroadcast(new TriggerInfoModel(null, AgainstInfo.cardSet.CardList).SetMeanWhile()[TriggerTime.When][TriggerType.RoundEnd]);
    }
    /// <summary>
    /// 获取游戏内对战信息的高层api接口
    /// </summary>
    public class InfoSystem
    {
        /// <summary>
        /// 获取对战内所有卡牌集合,并可通过各种特征标签进行筛选
        /// </summary>
        public static CardSet AgainstCardSet => AgainstInfo.cardSet;
        /// <summary>
        /// 获取对战时通过选择操作选择的卡牌列表
        /// </summary>
        public static List<Card> SelectUnits => AgainstInfo.SelectUnits;
        /// <summary>
        /// 获取对战时通过选择操作选择的首个卡牌
        /// </summary>
        public static Card SelectUnit => AgainstInfo.SelectUnits.FirstOrDefault();
        /// <summary>
        /// 获取对战时通过选择的区域序号
        /// </summary>
        public static int SelectRegionRank => AgainstInfo.SelectRowRank;
        /// <summary>
        /// 获取对战时选择的区域对应的卡牌列表
        /// </summary>
        public static List<Card> SelectRegionCardList => AgainstInfo.SelectRowCardList;
        /// <summary>
        /// 获取对战时通过选择的区域序号
        /// </summary>
        public static int SelectLocation => AgainstInfo.SelectRank;
        /// <summary>
        /// 当选择面版中的卡牌皆为对战中出现的实体目标时
        /// 可从SelectBoardCards获得选择的单位实例对象
        ///  注意：
        /// 当面版卡牌皆为对战时不存在的虚构卡牌时
        /// 可从SelectBoardCardIds获得选择的单位的卡牌id列表
        /// </summary>
        public static List<Card> SelectBoardCards => AgainstInfo.SelectActualCards;
        /// <summary>
        /// 当面版卡牌皆为对战时不存在的虚构卡牌时
        /// 可从SelectBoardCardIds获得选择的单位的卡牌id列表
        ///  注意：
        /// 当选择面版中的卡牌皆为对战中出现的实体目标时
        /// 可从SelectBoardCards获得选择的单位实例对象
        /// </summary>
        public static List<int> SelectBoardCardIds => AgainstInfo.SelectVirualCardIds;
        /// <summary>
        /// 获得目标卡牌字段值
        /// </summary>
        public static int GetField(Card card, CardField cardField) => card[cardField];
        /// <summary>
        /// 获得目标两侧卡牌字段值之和
        /// </summary>
        public static int GetTwoSideField(Card card, CardField cardField) => (card.LeftCard == null || card.LeftCard[CardState.Seal] ? 0 : card.LeftCard[cardField]) + (card.RightCard == null || card.RightCard[CardState.Seal] ? 0 : card.RightCard[cardField]);
    }
    /// <summary>
    /// 会对UI层产生影响的立绘、属性选择、对话等
    /// </summary>
    public class UiSystem
    {
        /// <summary>
        /// 显示立绘(领袖)
        /// </summary>
        public static async Task ShowFigure(Card card) => await Manager.FigureManager.Instance.ShowFigureAsync(true, card.CardTranslateName);
        /// <summary>
        /// 在卡牌上显示文字
        /// </summary>
        public static async Task ShowTips(Card card,string Text,Color color) => await card.ThisCardManager.ShowTips(Text, color);
        /// <summary>
        ///  在卡牌上显示图标
        /// </summary>
        public static async Task ShowIcon(Card card, CardField cardField) => await card.ThisCardManager.ShowFieldIcon(cardField);
        /// <summary>
        ///  在卡牌上显示图标
        /// </summary>
        public static async Task ShowIcon(Card card, CardState cardState) => await card.ThisCardManager.ShowStateIcon(cardState);
        /// <summary>
        ///  在卡牌上显示破碎图标
        /// </summary>
        public static async Task ShowIconBreak(Card card, CardField cardField) => await card.ThisCardManager.ShowFieldIconBreak(cardField);
        /// <summary>
        ///  在卡牌上显示破碎图标
        /// </summary>
        public static async Task ShowIconBreak(Card card, CardState cardState) => await card.ThisCardManager.ShowStateIconBreak(cardState);

    }
}