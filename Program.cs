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

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin() // Warning: only use AllowAnyOrigin in development.
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "main",
    pattern: "{*url}",  // This should catch everything including the root
    defaults: new { controller = "Main", action = "Index"});

app.Run();