// This file is the main entry point for the ASP.NET Core application.
// It configures services, defines the HTTP request pipeline, and sets up routing.
// It also includes a static helper class `SessionCart` for managing the shopping cart in the session.
using System.Diagnostics;

Debug.WriteLine(">>>> Program.cs execution started <<<<");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Configure session services.
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sets session timeout.
    options.Cookie.HttpOnly = true; // Makes the session cookie inaccessible to client-side scripts.
    options.Cookie.IsEssential = true; // Marks the session cookie as essential for GDPR compliance.
});

builder.Services.AddScoped(SessionCart.GetCart);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Uses a generic error handler page in production.

    app.UseHsts(); // Adds HTTP Strict Transport Security Protocol (HSTS) headers.
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS.
app.UseStaticFiles(); // Enables serving static files (e.g., CSS, JavaScript, images).

app.UseRouting(); // Adds routing middleware to the pipeline.

app.UseSession(); // Enables session state. This must be called before UseAuthorization and MapControllerRoute.

app.UseAuthorization(); // Adds authorization middleware.

// Configures the default controller route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Starts the application.


