using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:板烧琪露诺
    /// </summary>
    public class Card2000001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   //await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle].CardList, 2);
                   //for (int i = 0; i < 5; i++)
                   //{
                   //    await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetLocation(Orientation.My, GameRegion.Water, 0));
                   //    await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetLocation(Orientation.My, GameRegion.Fire, 0));
                   //    await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetLocation(Orientation.My, GameRegion.Wind, 0));
                   //    await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetLocation(Orientation.My, GameRegion.Soil, 0));
                   //}
                   int targetCount = AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][GameEnum.CardTag.Fairy].count;
                   Debug.Log("场上妖精数量为" + targetCount);
                   for (int i = 0; i < targetCount; i++)
                   {
                       if (BasePoint > 1)
                       {
                           await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.Op][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList, 1, isAuto: true);
                           await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, AgainstInfo.SelectUnits).SetPoint(1));
                           await GameSystem.PointSystem.Weak(new TriggerInfoModel(this, this).SetPoint(1));
                       }
                   }
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}