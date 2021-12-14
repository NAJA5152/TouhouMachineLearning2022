using System.Linq.Expressions;
namespace Server
{
    class TimerTask
    {
        public static void Creat(Action action,int day,int hour,int minutes)
        {
            Timer timer = new Timer(new TimerCallback(o => action()));
            timer.Change(DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0))
        }
        public static void Creat(Action action, int )
        {
            Timer timer = new Timer(new TimerCallback(o => action()));
            timer.Change(DateTime.Today.AddDays(0).AddHours(0).AddMinutes(0))
        }
    }
}