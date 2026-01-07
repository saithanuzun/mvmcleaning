using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Application;
using mvmclean.backend.Infrastructure;
using mvmclean.backend.Infrastructure.Persistence;
using mvmclean.backend.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add SPA services for React app
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/dist";
});

builder.Services.AddAuthentication()
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(60);
    })
    .AddCookie("ContractorCookie", options =>
    {
        options.LoginPath = "/Contractor/Login";
        options.AccessDeniedPath = "/Contractor/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(60);
    });

builder.Services.AddInfrastructureRegistration();
builder.Services.AddApplicationRegistration();


var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MVMdbContext>();
    try
    {
        // Apply any pending migrations
        await dbContext.Database.MigrateAsync();
        
        // Seed initial data
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}




app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error/Status/{0}");


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();


app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure SPA to serve React app under /shop route
app.Map("/shop", shop =>
{
    shop.UseSpa(spa =>
    {
        spa.Options.SourcePath = "ClientApp";

        if (app.Environment.IsDevelopment())
        {
            spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
        }
    });
});

app.Run();