using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Server;
using Server.Data;
using Newtonsoft.Json.Serialization;
MongoDbCommand.Init();
Console.WriteLine("���ݿ��ѳ�ʼ��");

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
Console.WriteLine("�������Ӧ����");
Console.WriteLine("�����������");
Task.Run(() =>
{
    while (true)
    {
        Console.ReadLine();
        RoomCommand.Rooms.FirstOrDefault()?.Summary.UploadAgentSummary();
        Console.WriteLine("�ϴ����");
    }
});
PlanManager.Creat("����ƥ��", HoldListManager.Match,0,0,0,0,0,0);
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
    Console.WriteLine("ƥ��ɹ�������" + HoldListManager.Match());
    Console.WriteLine("ƥ���ʱ" + (startTime - DateTime.Now));
}
app.Run();

