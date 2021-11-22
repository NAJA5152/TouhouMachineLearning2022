using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hutao : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float value =Mathf.Clamp01(Mathf.Abs(transform.eulerAngles.z>180? transform.eulerAngles.z-360: transform.eulerAngles.z) / 25);
        transform.GetComponent<Renderer>().material.SetFloat("_value", value);
    }
}
