using System.Threading;
namespace TouhouMachineLearningSummary.Manager
{
    class TaskLoopManager
    {
        public static CancellationTokenSource cancel;
        public static void Init() => cancel = new CancellationTokenSource();
        public static void TriggerThrow() => cancel.Cancel();
        public static void Throw()
        {
            try
            {
                cancel.Token.ThrowIfCancellationRequested();
            }
            catch (System.Exception)
            {
                UnityEngine.Debug.LogError("线程中断！");
                throw;
            }
        }
    }
}
