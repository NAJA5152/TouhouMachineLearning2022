using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;

public class testward : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    async void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            await CustomThread.TimerAsync(0.5f, runAction: (process) => //在0.4秒内不断移动并降低透明度
            {
                GetComponent<Renderer>().material.SetFloat("_value",  process);
            });
        }
        if (Input.GetMouseButtonDown(1))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            _ = CustomThread.TimerAsync(0.5f, runAction: (process) => //在0.4秒内不断移动并降低透明度
            {
                GetComponent<Renderer>().material.SetFloat("_value", 1 - process);
            });
        }
    }
}
