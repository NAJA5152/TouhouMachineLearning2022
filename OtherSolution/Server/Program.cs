using Server;
using Server.Data;
using System;
(float ing, float lat) targetPoint = (0, 0);
var points = new List<(float ing, float lat)>() { (0, 1) };
//points.ForEach(point => Console.WriteLine(point.ToJson() + "距离是" + Distance(ToXY(point), ToXY(targetPoint))));
points.ForEach(point => Console.WriteLine(point.ToJson() + "距离是" + Distance(point, targetPoint)));

//points.Where(point => Distance(ToXY(point), ToXY(targetPoint)) < 3);

(float x, float y) ToXY((float ing, float lat) point)
{
    float scale = (float)Math.Cos(point.lat / 90 * Math.PI);
  
    
    float x = point.ing * scale * 111.18957695999f;
    float y = point.lat * 111.18957695999f;
    return (x, y);
}
//float Distance((float x, float y) point, (float x, float y) target) => (float)Math.Sqrt(Math.Pow((point.x - target.x), 2) + Math.Pow((point.y - target.y), 2));

float Distance((float x, float y) point, (float x, float y) target)
{
    double dLat1InRad = point.y * (Math.PI / 180);
    double dLong1InRad = point.x * (Math.PI / 180);
    double dLat2InRad = target.y * (Math.PI / 180);
    double dLong2InRad = target.x * (Math.PI / 180);
    double dLongitude = dLong2InRad - dLong1InRad;
    double dLatitude = dLat2InRad - dLat1InRad;
    double a = Math.Pow(Math.Sin(dLatitude / 2), 2) + Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) * Math.Pow(Math.Sin(dLongitude / 2), 2);
    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    float dDistance = 6385.66f *(float) c;
    return dDistance;

}

MongoDbCommand.Init();
Console.WriteLine("数据库已初始化");
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
Console.WriteLine("已载入回应中心");
Console.WriteLine("服务端已启动");
Task.Run(() =>
{
    while (true)
    {
        Console.ReadLine();
        //RoomManager.Rooms.ForEach(room => Console.WriteLine(room.Summary.ToJson()));
        RoomManager.Rooms.FirstOrDefault()?.Summary.UploadAgentSummary();
        Console.WriteLine("上传完毕");
    }
});
Timer timer = new Timer(new TimerCallback((o) => HoldListManager.Match()));
timer.Change(0, 5000);
app.Run();
