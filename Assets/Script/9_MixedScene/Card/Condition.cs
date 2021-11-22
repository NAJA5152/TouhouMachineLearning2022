namespace TouhouMachineLearningSummary.GameEnum
{
    public enum Condition
    {
        /// <summary>
        /// 默认效果触发条件，等价位于战场，自身未死亡，且未被封印
        /// </summary>
        Default,
        OnBattle,
        /// <summary>
        /// 我方回合
        /// </summary>
        OnMyTurn,
        OnOpTurn,
        Seal,
        NotSeal,
        Dead,
        NotDead
    }
}
