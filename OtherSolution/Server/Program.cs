using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Server;
using Server.Data;
using Newtonsoft.Json.Serialization;
MongoDbCommand.Init();
Console.WriteLine("数据库已初始化");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddAntDesign();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.MaximumReceiveMessageSize = null;
}).AddNewtonsoftJsonProtocol();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<TouHouHub>("/TouHouHub");
app.Urls.Add("https://localhost:555");
app.Urls.Add("http://localhost:495");
//app.Urls.Add("https://localhost:514");
//app.Urls.Add("https://localhost:515");
//app.Urls.Add("https://localhost:888");
Console.WriteLine("已载入回应中心");
Console.WriteLine("服务端已启动");
Task.Run(() =>
{
    while (true)
    {
        Console.ReadLine();
        RoomCommand.Rooms.FirstOrDefault()?.Summary.UploadAgentSummary();
        Console.WriteLine("上传完毕");
    }
});
PlanManager.Creat("进行匹配", HoldListManager.Match,0,0,0,0,0,0);
Timer timer = new Timer(new TimerCallback((o) => MatchAction()));
timer.Change(0, 1000);
while (true)
{
    Console.ReadLine();
    AddUser();
}
void MatchAction()
{
    AddUser();
    startTime = DateTime.Now;
    Console.WriteLine("匹配成功数量：" + HoldListManager.Match());
    Console.WriteLine("匹配耗时" + (startTime - DateTime.Now));
}
app.Run();

