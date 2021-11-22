using UnityEngine;
namespace TouhouMachineLearningSummary.Other
{
    public class DontDestory : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(this);
    }
}