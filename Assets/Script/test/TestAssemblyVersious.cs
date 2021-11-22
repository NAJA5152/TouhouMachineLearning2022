using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

public class TestAssemblyVersious : MonoBehaviour
{
    [Button]
    public void Use07()
    {
        CardAssemblyManager.SetCurrentAssembly("2021_10_07");
    }

    [Button]
    public void Use08()
    {
        CardAssemblyManager.SetCurrentAssembly("2021_10_08");
    }
}
