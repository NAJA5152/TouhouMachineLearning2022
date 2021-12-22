using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Server;
using Server.Data;
using Newtonsoft.Json.Serialization;
MongoDbCommand.Init();
Console.WriteLine("���ݿ��ѳ�ʼ��");
HoldListManager.Init();
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
app.Urls.Add("http://localhost:495");
Console.WriteLine("�������Ӧ����");
Console.WriteLine("�����������");
Task.Run(() =>
{
    while (true)
    {
        Console.ReadLine();
        //RoomManager.Rooms.ForEach(room => Console.WriteLine(room.Summary.ToJson()));
        RoomManager.Rooms. FirstOrDefault()?.Summary.UploadAgentSummary();
        Console.WriteLine("�ϴ����");
    }
});
Timer timer = new Timer(new TimerCallback((o) => HoldListManager.Match()));
timer.Change(0, 5000);
app.Run();


//app.Urls.Add("https://localhost:496");
//app.Urls.Add("https://localhost:555");
//app.Urls.Add("https://localhost:514");
//app.Urls.Add("https://localhost:515");
//app.Urls.Add("https://localhost:888");