using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
namespace TouhouMachineLearningSummary.Thread
{
    public class CustomThread : MonoBehaviour
    {
        //static Queue<Action> TargetAction = new Queue<Action>();
        //void Awake() => timer = gameObject;
        //public static void Init() => TargetAction.Clear();
        //[Obsolete("这写法全都过时啦！老东西")]
        //public static void Run(Action RunAction) => TargetAction.Enqueue(RunAction);
        /// <summary>
        /// 定时任务模块
        /// </summary>
        //static GameObject timer;
        public static async Task TimerAsync(float stopTime, Action<float> runAction = null)
        {
            float currentTime = 0;
            while (currentTime <= stopTime)
            {
                runAction(currentTime);
                currentTime += 0.1f;
                await Task.Delay(10);
            }
        }
        //void Update()
        //{
        //    if (TargetAction.Any())
        //    {
        //        TargetAction.Dequeue()?.Invoke();
        //    }
        //}
        /// <summary>
        /// 自定义延时函数，在训练模式下会变成无延时模式，加速训练
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Task Delay(int time) => Info.AgainstInfo.isTrainMode ? Task.Delay(0) : Task.Delay(time);

    }
}
