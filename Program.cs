using Microsoft.EntityFrameworkCore;
using AnnuaireCESI.Data;
using AnnuaireCESI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AnnuaireDbContext>(
    options => options.UseNpgsql(
        "Host=localhost;Database=annuaire-dotnet;Username=postgres;Password=postgres;Port=5432"));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.Cookie.Name = "AgronnectAuthCookie";
    });

builder.Services.AddMvc();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


public class SmtpConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}