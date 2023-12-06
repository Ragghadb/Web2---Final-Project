using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FinalPtoject.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FinalPtojectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinalPtojectContext") ?? throw new InvalidOperationException("Connection string 'FinalPtojectContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(1); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=usersalls}/{action=login}");

app.Run();
