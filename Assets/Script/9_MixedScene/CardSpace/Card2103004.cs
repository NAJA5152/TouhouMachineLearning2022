using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:充能水炮
    /// 卡牌能力:部署：移除左侧单位能量，随后选择对方场上一个非金单位对其产生等能量值的伤害
    /// </summary>
    public class Card2103004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                    await GameSystem.TransferSystem.DeployCard(new Event(this, this));
                })
                .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
                .AbilityAdd(async (e) =>
                {
                    int num = LeftCard[CardField.Energy];
                    await GameSystem.FieldSystem.SetField(new Event(this, LeftCard).SetTargetField(CardField.Energy, 0));

                    await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList, 1);
                    await GameSystem.PointSystem.Hurt(new Event(this, GameSystem.InfoSystem.SelectUnits).SetPoint(num));
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}