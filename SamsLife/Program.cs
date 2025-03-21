// filepath: /workspaces/llm-demo/SamsLife/Program.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SamsLife.Data;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddServerSideBlazor();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();
//builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

//builder.Services.AddSingleton<WeatherForecastService>();
//builder.Services.AddScoped<MyIdService>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            // Always return to /analyze after sign-in
            context.ProtocolMessage.RedirectUri = "https://samslife2.azurewebsites.net/analyze";
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures cookies are only sent over HTTPS
    options.Cookie.HttpOnly = true; // Prevents client-side scripts from accessing the cookie
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevents the cookie from being sent with cross-site requests
});

// builder.Services.AddAntiforgery(options =>
// {
//     options.Cookie.Name = ".AspNetCore.Antiforgery"; // Name of the antiforgery cookie
//     options.HeaderName = "X-XSRF-TOKEN"; // Header name for antiforgery token
//});
builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers(); 
app.MapRazorPages(); 
app.MapBlazorHub(); 
app.MapFallbackToPage("/_Host");

app.UseAuthentication();
app.UseAuthorization();  
app.UseSession();


app.Run();