using UnityEngine;

public class BuletTest : MonoBehaviour
{
    public GameObject bulletModel;
    public GameObject startPos;
    public float range;
    public float force;
    // Start is called before the first frame update
    void Start()
    {
        //Task.Run(() =>
        //{
        //    MainThread.Run(async () =>
        //    {
        //        //60-120
        //        for (int i = 0; i < 15; i++)
        //        {
        //            for (int j = 60; j < 120; j += 5)
        //            {
        //                Vector3 bulletPos = range * new Vector3(Mathf.Cos(j / 180f * Mathf.PI), 0, Mathf.Sin(j / 180f * Mathf.PI));
        //                GameObject newBullet = Instantiate(bulletModel, startPos.transform.position + bulletPos, Quaternion.Euler(bulletPos - startPos.transform.position));
        //                newBullet.transform.forward = bulletPos - startPos.transform.position; ;
        //                newBullet.GetComponent<Renderer>().material.SetColor("_Color", Color.red * 10);
        //                newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * force, ForceMode.Acceleration);
        //                await Task.Delay(100);
        //            }
        //            await Task.Delay(1000);
        //            for (int j = 65; j < 115; j += 5)
        //            {
        //                Vector3 bulletPos = range * new Vector3(Mathf.Cos(j / 180f * Mathf.PI), 0, Mathf.Sin(j / 180f * Mathf.PI));
        //                GameObject newBullet = Instantiate(bulletModel, startPos.transform.position + bulletPos, Quaternion.Euler(bulletPos - startPos.transform.position));
        //                newBullet.transform.forward = bulletPos - startPos.transform.position; ;
        //                newBullet.GetComponent<Renderer>().material.SetColor("_Color", Color.blue*10);
        //                newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * force, ForceMode.Acceleration);
        //                await Task.Delay(100);
        //            }
        //            await Task.Delay(2000);

        //        }


        //    });
        //});
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
