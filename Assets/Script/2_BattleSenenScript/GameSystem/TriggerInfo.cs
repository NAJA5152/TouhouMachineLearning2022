using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
//using UnityEngine;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.Info
{
    public class TriggerInfo
    {
        public TriggerTime triggerTime;
        public TriggerType triggerType;
        public Card triggerCard;
        public List<Card> targetCards;
        //public bool autoSelect = false;
        //public int selectNum = 0;
        /// <summary>
        /// 判断多个卡牌目标是否同时触发效果
        /// </summary>
        public bool triggerMeanWhile = false;
        [JsonIgnore]
        public Card targetCard => targetCards[0];
        public int point;
        public Location location = new Location(-1, -1);
        public BulletModel bulletModel { get; set; }
        [JsonIgnore]
        public TriggerInfo this[TriggerTime triggerTime] => Clone(triggerTime: triggerTime);
        [JsonIgnore]
        public TriggerInfo this[TriggerType triggerType] => Clone(triggerType: triggerType);
        [JsonIgnore]
        public TriggerInfo this[Card targetCard] => Clone(targetCards: new List<Card> { targetCard });

        private TriggerInfo Clone(TriggerTime? triggerTime = null, TriggerType? triggerType = null, List<Card> targetCards = null)
        {
            TriggerInfo triggerInfo = new TriggerInfo(triggerCard);
            triggerInfo.triggerTime = triggerTime ?? this.triggerTime;
            triggerInfo.triggerType = triggerType ?? this.triggerType;
            triggerInfo.targetCards = targetCards ?? this.targetCards;
            triggerInfo.triggerMeanWhile = triggerMeanWhile;
            triggerInfo.triggerCard = triggerCard;
            triggerInfo.bulletModel = bulletModel;
            triggerInfo.location = location;
            triggerInfo.point = point;
            //triggerInfo.param = param;
            //TriggerInfo triggerInfo = this.ToJson().ToObject<TriggerInfo>();
            //triggerInfo.triggerTime = triggerTime ?? this.triggerTime;
            //triggerInfo.triggerType = triggerType ?? this.triggerType;
            //triggerInfo.targetCards = targetCards ?? this.targetCards;
            return triggerInfo;
        }
        public TriggerInfo() { }
        /// <summary>
        /// 设置触发者（某卡牌或者系统(null)）
        /// </summary>
        public TriggerInfo(Card triggerCard) => this.triggerCard = triggerCard;

        /// <summary>
        /// 设置触发对象(单个)
        /// </summary>
        public TriggerInfo SetTargetCard(Card targetCard)
        {
            this.targetCards = new List<Card>() { targetCard }; ;
            return this;
        }
        /// <summary>
        /// 设置触发对象(多个)
        /// </summary>
        public TriggerInfo SetTargetCard(List<Card> targetCards)
        {
            this.targetCards = targetCards;
            return this;
        }
        /// <summary>
        /// 设置部署区域（靠所属，区域和次序定位，次序为正代表从左往右，最左侧位置为0，为负代表从右往左，最右侧为-1）
        /// </summary>
        public TriggerInfo SetLocation(Orientation orientation, GameRegion regionType, int rank)
        {
            int x = AgainstInfo .cardSet[regionType][orientation].singleRowInfos.First().RowRank;
            int y = rank;// >= 0 ? Math.Min(rank, CardSet.globalCardList[x].Count) : Math.Max(0, CardSet.globalCardList[x].Count + rank + 1);
            location = new Location(x, y);
            
            return this;
        }
        /// <summary>
        /// 设置触发点数信息
        /// </summary>
        public TriggerInfo SetPoint(int point)
        {
            this.point = point;
            return this;
        }
        ///// <summary>
        ///// 设置自动选择模式
        ///// </summary>
        //public TriggerInfo SetAutoSelectMode()
        //{
        //    this.autoSelect = true;
        //    return this;
        //}
        //public TriggerInfo SetSelectNum(int num)
        //{
        //    this.selectNum = num;
        //    return this;
        //}
        public TriggerInfo SetBullet(BulletModel bulletModel)
        {
            this.bulletModel = bulletModel;
            return this;
        }
        /// <summary>
        /// 以同时的方式触发弹幕和卡牌效果
        /// </summary>
        /// <returns></returns>
        public TriggerInfo SetMeanWhile()
        {
            this.triggerMeanWhile = true;
            return this;
        }
    }
}