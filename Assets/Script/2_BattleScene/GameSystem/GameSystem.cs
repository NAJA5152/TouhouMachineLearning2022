﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
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
        public static async Task Set(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Set]);
        //增益
        public static async Task Gain(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Gain]);
        //伤害
        public static async Task Hurt(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Hurt]);
        //治愈
        public static async Task Cure(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Cure]);
        //重置
        public static async Task Reset(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Reset]);
        //强化
        public static async Task Strengthen(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Strengthen]);
        //削弱
        public static async Task Weak(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Weak]);
        //摧毁
        public static async Task Destory(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Destory]);
        //逆转
        public static async Task Reversal(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Reverse]);
        //增加
        public static async Task Increase(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Increase]);
        //减少
        public static async Task Decrease(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Decrease]);
    }
    /// <summary>
    /// 转移卡牌位置、所属区域的相关机制，共有战场区，手牌区，使用区，墓地区，牌组区 共五个区域
    /// </summary>
    public class TransferSystem
    {
        /// <summary>
        /// 卡牌直接出现在对应区域，仅限单位牌，不会触发联锁效果
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static async Task GenerateCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[CardCommand.GenerateCard(e.targetCardId)][TriggerType.Generate]);
        /// <summary>
        /// 在战场区各个区域间移动卡牌(触发移动连锁效果)
        /// </summary>
        public static async Task MoveCard(Event e)
        {
            if (e.targetCard != null && !e.targetCard[CardState.Forbidden] && GameSystem.InfoSystem.AgainstCardSet[e.location.X].Count < 6)
            {
                await GameSystemCommand.TriggerBroadcast(e[TriggerType.Move]);
            }
        }
        /// <summary>
        /// 从卡组区移动至手牌区(不触发移动连锁效果)
        /// </summary>
        public static async Task DrawCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Draw]);
        /// <summary>
        /// 回手，从战场区移动至手牌区（不触发移动连锁效果)
        /// </summary>
        public static async Task RecycleCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Reverse]);

        /// <summary>
        /// 只支持单个牌被打出
        /// 打出，从手牌区\战场区打出至使用区(不触发移动连锁效果)
        /// </summary>
        public static async Task PlayCard(Event e)
        {
            if (e.targetCard != null)
            {
                //遍历打出所有目标卡牌
                foreach (var targetCard in e.targetCards)
                {
                    await Command.CardCommand.PlayCard(targetCard);
                    await GameSystemCommand.TriggerBroadcast(e[TriggerType.Play][targetCard]);
                }
            }
        }
        /// <summary>
        /// 复活，从墓地区移动至战场区(不触发移动连锁效果)
        /// </summary>
        public static async Task ReviveCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Revive]);
        /// <summary>
        /// 部署，从使用区移动至战场区(不触发移动连锁效果)
        /// 若为重新触发则不改变位置直接触发部署效果
        /// </summary>
        public static async Task DeployCard(Event e, bool ReTrigger = false)
        {
            //部署效果特殊处理，先执行部署行为再触发部署效果
            if (e.targetCards.Any() && AgainstInfo.SelectRowRank != -1)
            {
                await Command.CardCommand.DeployCard(e.targetCard, ReTrigger);
            }
            await GameSystemCommand.TriggerBroadcast(e[TriggerType.Deploy]);
        }
        /// <summary>
        /// 召唤，从牌组区移动至战场区(不触发移动连锁效果)
        /// </summary>
        public static async Task SummonCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Summon]);

        /// <summary>
        /// 弃牌，从手牌区移动至墓地(不触发移动连锁效果)
        /// </summary>
        public static async Task DisCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Discard]);
        /// <summary>
        /// 死亡，从战场区移动至墓地(不触发移动连锁效果)
        /// </summary>
        public static async Task DeadCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Dead]);

        /// <summary>
        /// 间隙，从对局区移除至场外(不触发移动连锁效果)
        /// </summary>
        public static async Task BanishCard(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.Banish]);

        /// <summary>
        ///  直接移动至卡组(不触发任何连锁效果)
        /// </summary>
        public static async Task MoveToDeck(Card targetCard, int index = 0, bool isRandom = true) => await Command.CardCommand.MoveToDeck(targetCard, index, isRandom);
        /// <summary>
        /// 直接移动至对方手牌(不触发任何连锁效果)
        /// </summary>
        public static async Task MoveToOpHand(Card targetCard) => await Command.CardCommand.MoveToOpHand(targetCard);
        /// <summary>
        /// 直接移动至墓地(不触发任何连锁效果)
        /// </summary>
        public static async Task MoveToGrave(Card targetCard) => await Command.CardCommand.MoveToGrave(targetCard);
    }
    /// <summary>
    /// 为卡牌附加，去除，反转各类状态的相关机制
    /// </summary>
    public class StateSystem
    {
        public static async Task SetState(Event e)
        {
            //筛选触发目标，对不包含该状态的卡牌才会激活状态
            //e.targetCards = e.targetCards.Where(card => card[e.targetState]).ToList();
            await GameSystemCommand.TriggerBroadcast(e[TriggerType.StateAdd]);
        }
        public static async Task ClearState(Event e)
        {
            //筛选触发目标，对包含该状态的卡牌才会清空状态
            e.targetCards = e.targetCards.Where(card => card[e.targetState]).ToList();
            await GameSystemCommand.TriggerBroadcast(e[TriggerType.StateClear]);
        }
        public static async Task ChangeState(Event e)
        {
            List<Card> stateActivateCardList = e.targetCards.Where(card => card[e.targetState]).ToList();
            List<Card> stateUnActivateCardList = e.targetCards.Where(card => !card[e.targetState]).ToList();
            //设置所有状态未激活的为激活状态
            e.targetCards = stateUnActivateCardList;
            await GameSystemCommand.TriggerBroadcast(e[TriggerType.StateAdd]);
            //设置所有状态激活的为未激活状态
            e.targetCards = stateActivateCardList;
            await GameSystemCommand.TriggerBroadcast(e[TriggerType.StateClear]);
        }
    }
    /// <summary>
    /// 为卡牌附加，去除，反转各类值的相关机制
    /// </summary>
    public class FieldSystem
    {
        /// <summary>
        /// 直接设置字段值,不触发增加或减少衍生效果
        /// </summary>
        public static async Task SetField(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.FieldSet]);
        /// <summary>
        /// 设置字段值改变量，若为正数则触发"字段增加"衍生效果，若为负数则触发"字段减少"衍生效果
        /// </summary>
        public static async Task ChangeField(Event e) => await GameSystemCommand.TriggerBroadcast(e[TriggerType.FieldChange]);
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

        public static async Task SelectUnit(Card triggerCard, List<Card> filterCards, int num, bool isAuto = false) => await Command.StateCommand.WaitForSelecUnit(triggerCard, filterCards, num, isAuto);

        public static async Task SelectRegion(Card triggerCard, Territory territory = Territory.All, GameRegion regionType = GameRegion.Battle) => await Command.StateCommand.WaitForSelectRegion(triggerCard, territory, regionType);

    }
    /// <summary>
    /// 由系统触发的流程控制事件，不会由卡牌触发
    /// </summary>
    public class ProcessSystem
    {
        public static async Task WhenTurnStart() => await GameSystemCommand.TriggerNotice(new Event(null, AgainstInfo.cardSet.CardList)[TriggerType.TurnStart]);
        public static async Task WhenTurnEnd() => await GameSystemCommand.TriggerNotice(new Event(null, AgainstInfo.cardSet.CardList)[TriggerType.TurnEnd]);
        public static async Task WhenRoundStart() => await GameSystemCommand.TriggerNotice(new Event(null, AgainstInfo.cardSet.CardList)[TriggerType.RoundStart]);
        public static async Task WhenRoundEnd() => await GameSystemCommand.TriggerNotice(new Event(null, AgainstInfo.cardSet.CardList)[TriggerType.RoundEnd]);
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
        public static Card SelectBoardCard => SelectBoardCards.FirstOrDefault();
        /// <summary>
        /// 当面版卡牌皆为对战时不存在的虚构卡牌时
        /// 可从SelectBoardCardIds获得选择的单位的卡牌id列表
        ///  注意：
        /// 当选择面版中的卡牌皆为对战中出现的实体目标时
        /// 可从SelectBoardCards获得选择的单位实例对象
        /// </summary>
        public static List<int> SelectBoardCardIds => AgainstInfo.SelectVirualCardIds;
        public static int SelectBoardCardId => SelectBoardCardIds.FirstOrDefault();
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
        public static async Task ShowFigure(Card card) => await Manager.FigureManager.Instance.ShowFigureAsync(true, card.TranslateName);
        /// <summary>
        /// 在卡牌上显示文字
        /// </summary>
        public static async Task ShowTips(Card card, string Text, Color color) => await card.ThisCardManager.ShowTips(Text, color);
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
        /// <summary>
        /// 在卡牌上显示对话框
        /// </summary>
        public static async Task ShowDialog(Card card, string Text) => await Task.Delay(0);
    }
}