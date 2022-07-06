using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.Model
{
    //class Event { }
    public class Event
    {
        public TriggerTime triggerTime;
        public TriggerType triggerType;
        public Card triggerCard;
        public List<Card> targetCards;
        public CardState targetState;
        public CardField targetFiled;
        public int targetCardId;
        /// <summary>
        /// 判断多个卡牌目标是否同时触发效果
        /// </summary>
        public bool triggerMeanWhile = false;
        [JsonIgnore]
        public Card targetCard => targetCards.FirstOrDefault();
        public int point;
        public Location location = new Location(-1, -1);

        public BulletModel bulletModel { get; set; }
        [JsonIgnore]
        public Event this[TriggerTime triggerTime] => Clone(triggerTime: triggerTime);
        [JsonIgnore]
        public Event this[TriggerType triggerType] => Clone(triggerType: triggerType);
        [JsonIgnore]
        public Event this[Card targetCard] => Clone(targetCards: new List<Card> { targetCard });

        private Event Clone(TriggerTime? triggerTime = null, TriggerType? triggerType = null, List<Card> targetCards = null)
        {
            Event e = new Event(triggerCard, targetCards);
            e.triggerTime = triggerTime ?? this.triggerTime;
            e.triggerType = triggerType ?? this.triggerType;
            e.targetCards = targetCards ?? this.targetCards;
            e.triggerMeanWhile = triggerMeanWhile;
            e.triggerCard = triggerCard;
            e.bulletModel = bulletModel;
            e.location = location;
            e.point = point;
            e.targetState = targetState;
            e.targetFiled = targetFiled;
            e.targetCardId = targetCardId;
            return e;
        }
        //反序列化时使用
        public Event() { }
        /// <summary>
        /// 创建一个卡牌触发信息模板，并设置触发者（某卡牌,若是由系统触发则填null）、触发对象(单个)
        /// </summary>
        public Event(Card triggerCard, Card targetCard)
        {
            this.triggerCard = triggerCard;
            this.targetCards = new List<Card>();
            if (targetCard != null)
            {
                targetCards.Add(targetCard);
            }
        }
        /// <summary>
        /// 创建一个卡牌触发信息模板，并设置触发者（某卡牌,若是由系统触发则填null）、触发对象(多个)
        /// </summary>
        public Event(Card triggerCard, List<Card> targetCards)
        {
            this.triggerCard = triggerCard;
            this.targetCards = targetCards;
        }
        /// <summary>
        /// 设置部署区域（靠所属，区域和次序定位，次序为正代表从左往右，最左侧位置为0，为负代表从右往左，最右侧为-1）
        /// </summary>
        public Event SetLocation(Orientation orientation, GameRegion regionType, int rank)
        {
            int x = GameSystem.InfoSystem.AgainstCardSet[regionType][orientation].RowManagers.First().RowRank;
            int y = rank;
            location = new Location(x, y);
            return this;
        }
        /// <summary>
        /// 设置触发点数信息
        /// </summary>
        public Event SetPoint(int point)
        {
            this.point = point;
            return this;
        }
        public Event SetBullet(BulletModel bulletModel)
        {
            this.bulletModel = bulletModel;
            return this;
        }
        /// <summary>
        /// 以同时的方式触发弹幕和卡牌效果,触发后再进行结算
        /// </summary>
        /// <returns></returns>
        public Event SetMeanWhile()
        {
            this.triggerMeanWhile = true;
            return this;
        }
        /// <summary>
        /// 设置目标状态
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public Event SetTargetState(CardState targetState)
        {
            this.targetState = targetState;
            return this;
        }
        /// <summary>
        ///设置目标字段和目标值
        /// </summary>
        public Event SetTargetField(CardField targetField, int ponit)
        {
            this.targetFiled = targetField;
            this.point = ponit;
            return this;
        }
        /// <summary>
        /// 设置目标ID
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public Event SetTargetCardId(int cardId)
        {
            this.targetCardId = cardId;
            return this;
        }
    }
}