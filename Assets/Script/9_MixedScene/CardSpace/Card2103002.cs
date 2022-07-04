using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:储能电容器
    /// 卡牌能力:容量：（8） 我方回合结束：移除两侧单位能量值，加到自身身上，能量值超出容量后触发超载效果 超载：对全场所有单位造成1点伤害并摧毁自身
    /// </summary>
    public class Card2103002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (triggerInfo) =>
              {

                  int energyPoint = TwoSideCard.Sum(card => card[CardField.Energy]);
                  await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Energy, energyPoint));
                  await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, TwoSideCard).SetTargetField(CardField.Energy, 0));
                  if (this[CardField.Energy] > 8)
                  {
                      await GameSystem.UiSystem.ShowTips(this, "超载", new Color(1, 0, 0));
                      await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle].CardList).SetPoint(1).SetMeanWhile());
                      await GameSystem.PointSystem.Destory(new TriggerInfoModel(this, this));
                  }
              }, Condition.Default, Condition.OnMyTurn)
              .AbilityAppend();
        }
    }
}