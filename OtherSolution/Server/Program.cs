using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Server;
using Server.Data;
using Newtonsoft.Json.Serialization;
MongoDbCommand.Init();
Console.WriteLine("���ݿ��ѳ�ʼ��");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
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
app.Run();