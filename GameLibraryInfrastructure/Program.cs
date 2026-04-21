using GameLibraryInfrastructure;
using GameLibraryInfrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GameLibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<IdentityContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("IdentityConnection")
));

builder.Services.AddIdentity<User, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<IdentityContext>();

// import/export files
builder.Services.AddScoped<GameLibraryInfrastructure.Services.IDataPortServiceFactory<GameLibraryDomain.Model.Game>, 
    GameLibraryInfrastructure.Services.GameDataPortServiceFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    string[] roleNames = { "SuperAdmin", "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    var adminEmail = "superadmin@gaminglib.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = "Boss",
            Email = adminEmail,
            Createdat = DateTime.UtcNow
        };

        var createAdmin = await userManager.CreateAsync(adminUser, "SuperPassword123!");
        if (createAdmin.Succeeded)
        {
            await userManager.AddToRolesAsync(adminUser, new[] { "SuperAdmin", "Admin" });
        }
    }
}

app.Run();
