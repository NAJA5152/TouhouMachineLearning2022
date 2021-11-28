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

        // Update is called once per frame
        void Update()
        {
            //transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * 3);
            //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetTransform.eulerAngles, Time.deltaTime * 3);
        }
        public static async Task MoveToSceneViewPositionAsync(bool isImmediately = false)
        {
            await CustomThread.TimerAsync(isImmediately ? 0 : 1, (time) =>
                {
                    manager.transform.position = Vector3.Lerp(manager.transform.position, manager.sceneViewPosition.position, time);
                    manager.transform.eulerAngles = Vector3.Lerp(manager.transform.eulerAngles, manager.sceneViewPosition.eulerAngles, time);
                });
        }
        public static async Task MoveToBookViewAsync(bool isImmediately = false)
        {
            await CustomThread.TimerAsync(isImmediately ? 0 : 1, (time) =>
            {
                manager.transform.position = Vector3.Lerp(manager.transform.position, manager.bookViewPosition.position, time);
                manager.transform.eulerAngles = Vector3.Lerp(manager.transform.eulerAngles, manager.bookViewPosition.eulerAngles, time);
            });
        }

        public static async Task MoveToPageViewAsync(bool isImmediately = false)
        {
            await CustomThread.TimerAsync(isImmediately ? 0 : 1, (time) =>
            {
                manager.transform.position = Vector3.Lerp(manager.transform.position, manager.pageViewPosition.position, time);
                manager.transform.eulerAngles = Vector3.Lerp(manager.transform.eulerAngles, manager.pageViewPosition.eulerAngles, time);
            });
        }
    }
}

