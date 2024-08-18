using _2024FinalYearProject.Data;
using _2024FinalYearProject.Data.Interfaces;
using _2024FinalYearProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


var connString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<AppDbContext>(opts =>
opts.UseSqlServer(connString, opts =>
{
    opts.EnableRetryOnFailure();
    opts.CommandTimeout(120);
    opts.UseCompatibilityLevel(110);
}));


builder.Services.AddIdentityCore<AppUser>().AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();


//Configure middleware
var app = builder.Build();

//Configure middleware
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// route for paging all products
app.MapControllerRoute(
    name: "allpaging",
    pattern: "{controller}/{action}/{id=all}/page{BookingPage}");

// route for sorting
app.MapControllerRoute(
    name: "sortingcategory",
    pattern: "{controller}/{action}/{id}/orderby{sortBy}");

// least specific route - 0 required segments 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//SeedIdentityData.EnsureIdentityPopulated(app);

app.Run();