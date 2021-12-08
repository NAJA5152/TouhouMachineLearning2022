using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
//控制菜单中的摄像机镜头
namespace TouhouMachineLearningSummary.Manager
{
    public class CameraViewManager : MonoBehaviour
    {
        static CameraViewManager manager;

        public Transform sceneViewPosition;
        public Transform bookViewPosition;
        public Transform pageViewPosition;
        // Start is called before the first frame update
        void Awake() => manager = this;
        public static async Task MoveToSceneViewPositionAsync(bool isImmediately = false)
        {
            if (isImmediately)
            {
                Camera.main.transform.position = manager.sceneViewPosition.position;
                Camera.main.transform.eulerAngles = manager.sceneViewPosition.eulerAngles;
            }
            else
            {
                await CustomThread.TimerAsync(1, (time) =>
                {
                    Camera.main.transform.position = Vector3.Lerp(manager.transform.position, manager.sceneViewPosition.position, time);
                    Camera.main.transform.eulerAngles = Vector3.Lerp(manager.transform.eulerAngles, manager.sceneViewPosition.eulerAngles, time);
                });
            }
           
        }
        public static async Task MoveToBookViewAsync(bool isImmediately = false)
        {
            await CustomThread.TimerAsync(isImmediately ? 0 : 1, (time) =>
            {
                Camera.main.transform.position = Vector3.Lerp(manager.transform.position, manager.bookViewPosition.position, time);
                Camera.main.transform.eulerAngles = Vector3.Lerp(manager.transform.eulerAngles, manager.bookViewPosition.eulerAngles, time);
            });
        }

        public static async Task MoveToPageViewAsync(bool isImmediately = false)
        {
            await CustomThread.TimerAsync(isImmediately ? 0 : 1, (time) =>
            {
                Camera.main.transform.position = Vector3.Lerp(manager.transform.position, manager.pageViewPosition.position, time);
                Camera.main.transform.eulerAngles = Vector3.Lerp(manager.transform.eulerAngles, manager.pageViewPosition.eulerAngles, time);
            });
        }
    }
}

