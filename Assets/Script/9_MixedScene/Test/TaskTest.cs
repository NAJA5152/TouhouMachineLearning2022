using System;
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
            throw new Exception();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
