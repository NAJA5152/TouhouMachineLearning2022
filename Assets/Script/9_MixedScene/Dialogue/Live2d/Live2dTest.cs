using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;

public class Live2dTest : MonoBehaviour
{
    public CubismModel _model;
    // Start is called before the first frame update
    void Start()
    {
        _model = this.FindCubismModel();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _model.ForceUpdateNow();
    }
}
