using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:护盾生成器
    /// 卡牌能力:部署：移除左侧单位能量，随后选择我方场上一个非金单位对其产生等能量值的护盾
    /// </summary>
    public class Card2103003 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
              .AbilityAdd(async (e) =>
              {
                  int num = LeftCard[CardField.Energy];
                  await GameSystem.FieldSystem.SetField(new Event(this, LeftCard).SetTargetField(CardField.Energy, 0));

                  await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver,CardRank.Copper].CardList, 1);
                  await GameSystem.FieldSystem.ChangeField(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetField(CardField.Shield, num));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}