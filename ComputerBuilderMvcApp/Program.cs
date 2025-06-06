using ComputerBuilderMvcApp.Models; // If Cart is a service
using Microsoft.Extensions.DependencyInjection; // For AddSingleton, AddScoped, AddTransient

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the Cart service. You need to decide on its lifetime.
// If the cart should be per user session, you might need session state or other mechanisms.
// For a simple in-memory cart shared across all users (not recommended for production):
// builder.Services.AddSingleton<Cart>(); 
// For a cart that is created per request (might not be what you want for a shopping cart):
// builder.Services.AddScoped<Cart>(); 
// For a cart that is new every time it's requested:
// builder.Services.AddTransient<Cart>();

// If Cart needs to be session-specific, you'll need to configure session state:
builder.Services.AddDistributedMemoryCache(); // Required for session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Then register Cart as scoped or transient and manage its state via session.
// A common approach is to have a service that retrieves/stores the cart from/to the session.
// For now, let's assume a simple singleton for demonstration, replace with proper session management.
builder.Services.AddSingleton<Cart>();


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

app.UseAuthorization();

// If you added session state:
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();