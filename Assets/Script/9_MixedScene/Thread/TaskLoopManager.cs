using System.Threading;
namespace TouhouMachineLearningSummary.Manager
{
    class TaskLoopManager
    {
        public static CancellationTokenSource cancel;
        public static void Init() => cancel = new CancellationTokenSource();
        public static void TriggerThrow() => cancel.Cancel();
        public static void Throw() => cancel.Token.ThrowIfCancellationRequested();
    }
}
