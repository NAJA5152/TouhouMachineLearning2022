namespace TouhouMachineLearningSummary.GameEnum
{
    /// <summary>
    /// 触发的条件
    /// </summary>
    public enum Condition
    {
        /// <summary>
        /// 默认效果触发条件（位于战场，自身未死亡，且未被封印时触发）
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
