using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
//控制菜单中的摄像机镜头
namespace TouhouMachineLearningSummary.Manager
{
    public class CameraViewManager : MonoBehaviour
    {
        static CameraViewManager manager;

        public Transform sceneViewPoint;
        public Transform bookViewPosition;
        public Transform pageViewPosition;
        // Start is called before the first frame update
        void Awake() => manager = this;
        void Start()
        {
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
        }

        /// <summary>
        /// 0 场景视角
        /// 1 书本视角
        /// 2 页面视角
        /// </summary>
        /// <param name="isImmediately"></param>
        /// <returns></returns>
        public static async Task MoveToViewAsync(int viewIndex, bool isImmediately = false)
        {
            Vector3 targetPos = Vector3.zero;
            Vector3 targetEuler = Vector3.zero;
            switch (viewIndex)
            {
                case 0:
                    {
                        targetPos = manager.sceneViewPoint.position;
                        targetEuler = manager.sceneViewPoint.eulerAngles;
                        break;
                    }
                case 1:
                    {
                        targetPos = manager.bookViewPosition.position;
                        targetEuler = manager.bookViewPosition.eulerAngles;
                        break;
                    }
                case 2:
                    {
                        targetPos = manager.pageViewPosition.position;
                        targetEuler = manager.pageViewPosition.eulerAngles;
                        break;
                    }
            }
            if (isImmediately)
            {
                Camera.main.transform.position = targetPos;
                Camera.main.transform.eulerAngles = targetEuler;
            }
            else
            {
                await CustomThread.TimerAsync(1, (time) =>
                {
                    Camera.main.transform.position = Vector3.Lerp(manager.transform.position, targetPos, time);
                    Camera.main.transform.eulerAngles = Vector3.Lerp(manager.transform.eulerAngles, targetEuler, time);
                });
            }
        }
    }
}