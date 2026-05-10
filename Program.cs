//using Microsoft.EntityFrameworkCore;
//using WaterPlantApp.Data;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//// Remove or comment out HTTPS redirection if you want plain HTTP access from other machines.
//// app.UseHttpsRedirection();

//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthorization();
//app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

//// Make the app listen on all interfaces on port 5000 (reachable as http://10.85.77.253:5000)
//app.Urls.Clear();
//app.Urls.Add("http://0.0.0.0:5000");

//app.Run();

using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WaterPlantApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Resolve connection string: Azure env vars take priority over appsettings
string? connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ??
    Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection") ??
    Environment.GetEnvironmentVariable("SQLCONNSTR_DefaultConnection") ??
    builder.Configuration.GetConnectionString("DefaultConnection");

var loggerFactory = LoggerFactory.Create(lb => lb.AddConsole());
var startupLogger = loggerFactory.CreateLogger("Startup");

if (string.IsNullOrWhiteSpace(connectionString))
{
    startupLogger.LogCritical("Connection string 'DefaultConnection' not found.");
}
else if (builder.Environment.IsProduction() &&
         connectionString.IndexOf("(localdb)", StringComparison.OrdinalIgnoreCase) >= 0)
{
    startupLogger.LogCritical("LocalDB detected in Production — not allowed on Azure.");
    throw new InvalidOperationException("LocalDB connection string not allowed in Production.");
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.EnableRetryOnFailure()));
}

builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply EF Core migrations only when APPLY_MIGRATIONS=true env var is set
var applyMigrations = string.Equals(
    Environment.GetEnvironmentVariable("APPLY_MIGRATIONS"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (applyMigrations)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        startupLogger.LogInformation("Database migrations applied at startup.");
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex, "Failed to apply migrations at startup.");
        throw;
    }
}

try
{
    app.Run();
}
catch (Exception ex)
{
    try
    {
        var path = Environment.GetEnvironmentVariable("HOME") is string home
            && !string.IsNullOrEmpty(home)
                ? Path.Combine(home, "LogFiles", "startup_errors.txt")
                : Path.Combine(Directory.GetCurrentDirectory(), "startup_errors.txt");

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.AppendAllText(path, $"[{DateTime.UtcNow:O}] {ex}\n\n");
    }
    catch { }
    throw;
}