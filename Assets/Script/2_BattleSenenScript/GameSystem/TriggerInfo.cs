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
        /// 以同时的方式触发弹幕和卡牌效果
        /// </summary>
        /// <returns></returns>
        public TriggerInfoModel SetMeanWhile()
        {
            this.triggerMeanWhile = true;
            return this;
        }
    }
}