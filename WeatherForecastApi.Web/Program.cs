using Serilog;
using WeatherForecastApi.Application;
using WeatherForecastApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    //.WriteTo.Console()
    //.MinimumLevel.Debug()
    .CreateBootstrapLogger();

builder.Host.UseSerilog((hostContext, loggerConfiguration) =>
            _ = loggerConfiguration.ReadFrom.Configuration(builder.Configuration));

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Application services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Weather}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
