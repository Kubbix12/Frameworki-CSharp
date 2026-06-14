using Microsoft.EntityFrameworkCore;
using UserManager.Components;
using UserManager.Data;
using UserManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("Default")
                       ?? "Data Source=usermanager.db";

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
