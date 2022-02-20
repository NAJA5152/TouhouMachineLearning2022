using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;

public class testward : MonoBehaviour
{
    [ShowInInspector]
    public Dictionary<Color, int> colorMap= new Dictionary<Color, int>();
    public Texture2D texture;
    float[,] matrix;
    GameObject[,] models;
    public GameObject model;
    int w => texture.width;
    int h => texture.height;

    public float distanceX;
    public float distanceY;
    public float bias;
    // Start is called before the first frame update
    void Start()
    {
        matrix = new float[w, h];
        models = new GameObject[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Color color = texture.GetPixel(i, j);
                var heigh = FakeHigh(color);
                matrix[i, j] = heigh;
                models[i, j] = Instantiate(model);
                //Debug.Log("new");
                //models[i, j].transform.position = new Vector3(i, heigh, j);
                models[i,j].transform.position = new Vector3(i * distanceX + (j % 2 == 0 ? bias : 0), heigh, j * distanceY);
                models[i, j].GetComponent<Renderer>().material.color = color;
            }
        }
    }
    [Button]
    public void Test()
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Color color = texture.GetPixel(i, j);
                var heigh = FakeHigh(color);
                matrix[i, j] = heigh;
                //models[i, j].transform.position = new Vector3(i, heigh, j );
                models[i, j].transform.position = new Vector3(i * distanceX + (j % 2 == 0 ? bias : 0), heigh, j * distanceY);
                models[i, j].GetComponent<Renderer>().material.color = color;
            }
        }
    }
    async void Update()
    {
        //for (int i = 0; i < w - 1; i++)
        //{
        //    for (int j = 0; j < h - 1; j++)
        //    {
        //        Debug.DrawLine(new Vector3(i, matrix[i, j], j), new Vector3(i + 1, matrix[i + 1, j], j), Color.white);
        //        Debug.DrawLine(new Vector3(i, matrix[i, j], j), new Vector3(i, matrix[i, j + 1], j + 1), Color.white);
        //    }
        //}
        //if (Input.GetMouseButtonDown(0))
        //{
        //    transform.GetChild(0).gameObject.SetActive(true);
        //    await CustomThread.TimerAsync(0.5f, runAction: (process) => //在0.4秒内不断移动并降低透明度
        //    {
        //        GetComponent<Renderer>().material.SetFloat("_value", process);
        //    });
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    transform.GetChild(0).gameObject.SetActive(false);
        //    _ = CustomThread.TimerAsync(0.5f, runAction: (process) => //在0.4秒内不断移动并降低透明度
        //    {
        //        GetComponent<Renderer>().material.SetFloat("_value", 1 - process);
        //    });
        //}
    }
    public float FakeHigh(Color color)
    {
        return colorMap.Any()? colorMap.ToList().Sum(map =>
         {
             var distance = Vector3.Distance(ToVector3(map.Key), ToVector3(color));
             float weight = (Mathf.Sqrt(3) - distance) / Mathf.Sqrt(3);
             return map.Value * weight;
         }):0;
    }
    public Vector3 ToVector3(Color color) => new Vector3(color.r, color.g, color.b);
    // Update is called once per frame

}
