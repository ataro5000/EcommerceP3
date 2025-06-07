using System.Diagnostics;
using ComputerBuilderMvcApp.Models; // If Cart is a service
using Microsoft.AspNetCore.Http; // For IHttpContextAccessor
using Newtonsoft.Json; // For JsonConvert

Debug.WriteLine(">>>> Program.cs execution started <<<<");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache(); // For session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// This line registers your Cart to be created/retrieved by SessionCart.GetCart for each request.
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // IMPORTANT: This must be called before UseEndpoints or MapControllerRoute

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public static class SessionCart
{
    private const string CartSessionKey = "Cart";

    public static Cart GetCart(IServiceProvider services)
    {
        ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
        Cart? cart = null;

        if (session == null)
        {
            Debug.WriteLine("[SessionCart.GetCart] ISession is NULL. Returning new Cart.");
            return new Cart(); 
        }

        string? cartJson = session.GetString(CartSessionKey);
        Debug.WriteLine($"[SessionCart.GetCart] Retrieved cartJson from session: '{cartJson ?? "NULL or EMPTY"}'");

        if (!string.IsNullOrEmpty(cartJson))
        {
            try
            {
                cart = JsonConvert.DeserializeObject<Cart>(cartJson);
                if (cart == null)
                {
                    Debug.WriteLine("[SessionCart.GetCart] Deserialized cart is NULL. cartJson might be invalid or represent null.");
                }
                else
                {
                    Debug.WriteLine($"[SessionCart.GetCart] Successfully deserialized cart. Item count: {cart.Items.Count}");
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[SessionCart.GetCart] JSON Deserialization Error: {ex.Message}. Returning new Cart.");
                cart = new Cart(); 
            }
        }
        
        if (cart == null) 
        {
            Debug.WriteLine("[SessionCart.GetCart] Cart is null after attempting to load from session. Creating new Cart.");
            cart = new Cart();
            // MODIFICATION: Immediately save the new (empty) cart to session
            if (session != null) 
            {
                 try
                 {
                    string newCartJson = JsonConvert.SerializeObject(cart);
                    session.SetString(CartSessionKey, newCartJson); 
                    Debug.WriteLine($"[SessionCart.GetCart] Saved newly created empty cart to session. JSON: '{newCartJson}'");
                 }
                 catch (JsonException ex)
                 {
                    Debug.WriteLine($"[SessionCart.GetCart] JSON Serialization Error when saving new cart: {ex.Message}.");
                 }
            }
        }
        return cart;
    }


    public static void SaveCart(ISession session, Cart cart)
    {
        if (session == null)
        {
            Debug.WriteLine("[SessionCart.SaveCart] ISession is NULL. Cannot save cart.");
            return;
        }

        if (cart == null)
        {
            Debug.WriteLine("[SessionCart.SaveCart] Cart object is NULL. Cannot serialize and save.");
            // Optionally, remove the key if the cart is null
            // session.Remove(CartSessionKey);
            return;
        }

        try
        {
            string cartJsonToSave = JsonConvert.SerializeObject(cart);
            session.SetString(CartSessionKey, cartJsonToSave);
            Debug.WriteLine($"[SessionCart.SaveCart] Saved cart to session. Item count: {cart.Items.Count}, JSON: '{cartJsonToSave}'");
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"[SessionCart.SaveCart] JSON Serialization Error: {ex.Message}. Cart NOT saved.");
        }
    }
}