using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class shan : MonoBehaviour
{
    Vector3 targetPos;

    public bool IsShow { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    async void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPos = transform.position + Vector3.forward * 5;
            IsShow = true;
            await Task.Delay(1000);
            IsShow = false;

        }
        if (IsShow)
        {
            var a = GetComponent<Renderer>().material.GetFloat("_Float");
            var target = Mathf.Lerp(a, 3, Time.deltaTime * 5);
            GetComponent<Renderer>().material.SetFloat("_Float", target);

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2);
        }
        else
        {
            var a = GetComponent<Renderer>().material.GetFloat("_Float");
            var target = Mathf.Lerp(a, 0, Time.deltaTime * 5);
            GetComponent<Renderer>().material.SetFloat("_Float", target);
        }


    }
}
