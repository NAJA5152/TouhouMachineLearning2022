using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this,AgainstInfo.cardSet[Orientation.My][GameRegion.Battle].CardList,2);
                   for (int i = 0; i < 5; i++)
                   {
                       await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits).SetLocation(Orientation.My, GameRegion.Water, 0));
                       await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits).SetLocation(Orientation.My, GameRegion.Fire, 0));
                       await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits).SetLocation(Orientation.My, GameRegion.Wind, 0));
                       await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits).SetLocation(Orientation.My, GameRegion.Soil, 0));
                   }
                   //int targetCount = AgainstInfo.cardSet[Orientation.My][RegionTypes.Battle][CardTag.Fairy].count;
                   //Debug.Log("场上妖精数量为" + targetCount);
                   //for (int i = 0; i < targetCount; i++)
                   //{
                   //    if (basePoint > 1)
                   //    {
                   //        await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.Op][RegionTypes.Battle][CardRank.Silver, CardRank.Copper].CardList, 1, isAuto: true);
                   //        await GameSystem.PointSystem.Hurt(new TriggerInfo(this).SetTargetCard(AgainstInfo.selectUnits).SetPoint(1));
                   //        await GameSystem.PointSystem.Weak(new TriggerInfo(this).SetTargetCard(this).SetPoint(1));
                   //    }
                   //}
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}