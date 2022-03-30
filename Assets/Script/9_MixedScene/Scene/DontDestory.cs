using UnityEngine;
namespace TouhouMachineLearningSummary.Other
{
    public class DontDestory : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(this);
        private void OnApplicationQuit() => Manager.TaskLoopManager.cancel.Cancel();
    }
}