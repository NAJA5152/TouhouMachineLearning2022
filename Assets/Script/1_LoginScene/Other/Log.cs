using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Log
{
    static bool isLog=false;
    public static void Show(string tag)
    {
        if (isLog)
        {
            UnityEngine.Debug.Log(tag + ":" + DateTime.Now);
        }
    }
}

