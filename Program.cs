using altsite.Models;
using altsite.MongoControllers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


var dbSetting = builder.Configuration.GetSection("MongoDBSettings");
IMongoClient client = new MongoClient(dbSetting.GetValue<string>("connectionString"));
var db = client.GetDatabase("AltSite");
IMongoCollection<Site> collection = db.GetCollection<Site>("Sites");

builder.Services.AddSingleton<IMongoCollection<Site>>(collection);
builder.Services.AddSingleton<SiteMongoController>(new SiteMongoController(collection));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "main",
    pattern: "{url}",
    defaults: new { controller = "Main", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");

app.Run();