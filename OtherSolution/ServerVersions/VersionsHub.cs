using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;

public class VersionsHub : Hub
{
    static Process progress = null;
    public bool UpdateServer(byte[] datas)
    {
        try
        {
            CloseServer();
            File.WriteAllBytes("/root/gezi/Server/Server.dll", datas);
            Console.WriteLine("进行版本更新,当前服务器更新时间为" + new FileInfo("/root/gezi/Server/Server.dll").LastWriteTime);
            StartServer();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

    }
    public static void Init()
    {
        Console.WriteLine("启动版本控制器");
        progress = Process.Start("dotnet","/root/gezi/Server/Server.dll");
    }
    public void StartServer() => progress = Process.Start("dotnet", "Server/Server.dll");
    public void CloseServer() => progress.Close();
}