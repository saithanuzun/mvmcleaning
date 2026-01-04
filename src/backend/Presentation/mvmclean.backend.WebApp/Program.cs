using Microsoft.EntityFrameworkCore;
using mvmclean.backend.Application;
using mvmclean.backend.Infrastructure;
using mvmclean.backend.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}




app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error/Status/{0}");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();