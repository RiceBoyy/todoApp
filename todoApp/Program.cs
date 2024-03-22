using Azure.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication;
using todoApp.Code;
using todoApp.Components;
using todoApp.Components.Account;
using todoApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<CPRServices>();
builder.Services.AddScoped<ToDoListServices>();
builder.Services.AddSingleton<HashingHandler>();
builder.Services.AddSingleton<SymetrisHandler>();
builder.Services.AddSingleton<AsymmetricHandler>();
builder.Services.AddSingleton<Roles>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var todoListConnectionString = builder.Configuration.GetConnectionString("TodoDbConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseSqlServer(todoListConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthorizedPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireRole("Admin");
    });

    options.AddPolicy("EditorUserPolicy", policy =>
    {
        policy.RequireRole("EditorUser");
    });
});

// Configure allowed password requirements
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
});

// GET file path myCertifi.pfx
string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
userFolder = Path.Combine(userFolder, ".aspnet");
userFolder = Path.Combine(userFolder, "https");
userFolder = Path.Combine(userFolder, "myCertifi.pfx");
builder.Configuration.GetSection("Kestrel:Endpoints:https:Certificate:Path").Value = userFolder;

//// GET password from Secret.json
string KestrelCerPassword = builder.Configuration.GetValue<string>("KestrelCerPassword");
builder.Configuration.GetSection("Kestrel:Endpoints:https:Certificate:Password").Value = KestrelCerPassword;

// ONLY Kestrel Allowed Access
builder.WebHost.UseKestrel((context, serverOption) =>
{
    serverOption.Configure(context.Configuration.GetSection("Kestrel")).Endpoint("HTTPS", ListenOptions =>
    {
        ListenOptions.HttpsOptions.SslProtocols = SslProtocols.Tls13;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
