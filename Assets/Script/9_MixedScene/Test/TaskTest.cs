using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TaskTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await Test();
    }
    async Task Test()
    {
        await Test1();
        async Task Test1()
        {
            Debug.Log("Hello, World!");
            List<string> list = null;
            Debug.Log(list.Count);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
