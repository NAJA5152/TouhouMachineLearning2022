using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.Model
{
    public class TriggerInfoModel
    {
        public TriggerTime triggerTime;
        public TriggerType triggerType;
        public Card triggerCard;
        public List<Card> targetCards;
        public CardState targetState;
        public CardField targetFiled;
        /// <summary>
        /// 判断多个卡牌目标是否同时触发效果
        /// </summary>
        public bool triggerMeanWhile = false;
        [JsonIgnore]
        public Card targetCard => targetCards.FirstOrDefault();
        public int point;
        public Location location = new Location(-1, -1);
        private int targetStateMode;
        private int targetFieldMode;

        public BulletModel bulletModel { get; set; }
        [JsonIgnore]
        public TriggerInfoModel this[TriggerTime triggerTime] => Clone(triggerTime: triggerTime);
        [JsonIgnore]
        public TriggerInfoModel this[TriggerType triggerType] => Clone(triggerType: triggerType);
        [JsonIgnore]
        public TriggerInfoModel this[Card targetCard] => Clone(targetCards: new List<Card> { targetCard });

        private TriggerInfoModel Clone(TriggerTime? triggerTime = null, TriggerType? triggerType = null, List<Card> targetCards = null)
        {
            TriggerInfoModel triggerInfo = new TriggerInfoModel(triggerCard);
            triggerInfo.triggerTime = triggerTime ?? this.triggerTime;
            triggerInfo.triggerType = triggerType ?? this.triggerType;
            triggerInfo.targetCards = targetCards ?? this.targetCards;
            triggerInfo.triggerMeanWhile = triggerMeanWhile;
            triggerInfo.triggerCard = triggerCard;
            triggerInfo.bulletModel = bulletModel;
            triggerInfo.location = location;
            triggerInfo.point = point;
            triggerInfo.targetState = targetState;
            triggerInfo.targetFiled = targetFiled;
            return triggerInfo;
        }
        public TriggerInfoModel() { }
        /// <summary>
        /// 创建一个卡牌触发信息模板，并设置触发者（某卡牌,若是由系统触发则填null）
        /// </summary>
        public TriggerInfoModel(Card triggerCard) => this.triggerCard = triggerCard;

        /// <summary>
        /// 设置触发对象(单个)
        /// </summary>
        public TriggerInfoModel SetTargetCard(Card targetCard)
        {
            this.targetCards = new List<Card>() { targetCard }; ;
            return this;
        }
        /// <summary>
        /// 设置触发对象(多个)
        /// </summary>
        public TriggerInfoModel SetTargetCard(List<Card> targetCards)
        {
            this.targetCards = targetCards;
            return this;
        }
        /// <summary>
        /// 设置部署区域（靠所属，区域和次序定位，次序为正代表从左往右，最左侧位置为0，为负代表从右往左，最右侧为-1）
        /// </summary>
        public TriggerInfoModel SetLocation(Orientation orientation, GameRegion regionType, int rank)
        {
            int x = GameSystem.InfoSystem.AgainstCardSet[regionType][orientation].SingleRowInfos.First().RowRank;
            int y = rank;
            location = new Location(x, y);

            return this;
        }
        /// <summary>
        /// 设置触发点数信息
        /// </summary>
        public TriggerInfoModel SetPoint(int point)
        {
            this.point = point;
            return this;
        }
        public TriggerInfoModel SetBullet(BulletModel bulletModel)
        {
            this.bulletModel = bulletModel;
            return this;
        }
        /// <summary>
        /// 以同时的方式触发弹幕和卡牌效果,触发后再进行结算
        /// </summary>
        /// <returns></returns>
        public TriggerInfoModel SetMeanWhile()
        {
            this.triggerMeanWhile = true;
            return this;
        }
        /// <summary>
        /// 设置目标状态
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public TriggerInfoModel SetTargetState(CardState targetState)
        {
            this.targetState = targetState;
            return this;
        }
        /// <summary>
        ///设置目标字段和目标值
        /// </summary>
        public TriggerInfoModel SetTargetField(CardField targetField,int ponit)
        {
            this.targetFiled = targetField;
            this.point = ponit;
            return this;
        }
        ///// <summary>
        ///// 设置状态响应方式
        ///// 0赋予，只有原先未有该状态才会清除并触发衍生效果
        ///// 1改变,取反特定状态的
        ///// 2清除,只有原先已有该状态才会清除并触发衍生效果
        ///// </summary>
        //public TriggerInfoModel SetTargetStateMode(int targetStateMode)
        //{
        //    this.targetStateMode = targetStateMode;
        //    return this;
        //}
        ///// <summary>
        ///// 设置状态或字段的附加方式
        ///// 0赋予 直接设置字段的值，无论原先有没有
        ///// 1减少 只对原先有该值的卡牌产生效果并触发衍生效果
        ///// 2增加 只对原先有该值的卡牌产生效果并触发衍生效果
        ///// </summary>
        //private TriggerInfoModel SetTargetFieldMode(int targetFieldMode)
        //{
        //    this.targetFieldMode = targetFieldMode;
        //    return this;
        //}
        
    }
}