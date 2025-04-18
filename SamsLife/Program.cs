// filepath: /workspaces/llm-demo/SamsLife/Program.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SamsLife.Data;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.HttpOverrides;

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
            // Check if the user is navigating to /analyze
            var path = context.Request.Path.Value ?? "";

            if (path.Equals("/analyze", StringComparison.OrdinalIgnoreCase))
            {
                // Only redirect to /Landing if user came from /analyze
                context.ProtocolMessage.RedirectUri =
                    "https://samslife2.azurewebsites.net/Landing";
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".AspNetCore.Antiforgery";
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Path = "/"; // <-- Explicitly set cookie path
    //options.Cookie.Domain = "samslife2.azurewebsites.net"; // <-- Add this line explicitly

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Path = "/"; // <-- Explicitly set cookie path
    //options.Cookie.Domain = "samslife2.azurewebsites.net"; // <-- Add this line explicitly
});

builder.Services.AddScoped<UrlHashHelper>();

builder.Services.AddScoped<Util>();
builder.Services.AddHttpClient<Util>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    KnownNetworks = { }, // <-- explicitly clear these
    KnownProxies = { }   // <-- explicitly clear known proxies

});

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseHttpsRedirection();
app.UseAuthorization();  
app.UseSession();

app.MapControllers(); 
app.MapRazorPages(); 
app.MapBlazorHub(); 
app.MapFallbackToPage("/_Host");

app.Run();