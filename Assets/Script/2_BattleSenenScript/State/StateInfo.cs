using System.Threading;
namespace TouhouMachineLearningSummary.Manager
{
    class TakeLoopManager
    {
        public static CancellationTokenSource cancel;
        public static void Init()
        {
            //if (cancel == null)
            //{
                cancel = new CancellationTokenSource();
            //}
        }
    }
}
