using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourismMVC.Data;
using TourismMVC.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Get the Connection String from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=app.db";

// 2. Register ApplicationDbContext to use SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Register Identity (Login System)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Important for ApplicationUser!
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();