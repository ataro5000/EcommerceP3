
using System.Diagnostics;
using ComputerBuilderMvcApp.Models;
using Newtonsoft.Json;

/// Provides static methods to manage the shopping cart stored in the HTTP session.
public static class SessionCart
{
    private const string CartSessionKey = "Cart"; // Key used to store the cart in the session.


    /// Retrieves the current user's cart from the session.
    /// If no cart exists in the session, a new cart is created, saved to the session, and returned.

    /// <param name="services">The service provider to access session services.</param>
    /// <returns>The user's shopping cart.</returns>
    public static Cart GetCart(IServiceProvider services)
    {
        ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
        Cart? cart = null;

        if (session == null)
        {
            return new Cart(); 
        }

        string? cartJson = session.GetString(CartSessionKey);

        if (!string.IsNullOrEmpty(cartJson))
        {
            try
            {
                cart = JsonConvert.DeserializeObject<Cart>(cartJson);

            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"[SessionCart.GetCart] JSON Deserialization Error: {ex.Message}. Returning new Cart.");
                cart = new Cart(); 
            }
        }
        
        // If the cart is still null (e.g., not in session or deserialization failed), create a new one.
        if (cart == null) 
        {
            cart = new Cart();
            // Immediately save the new (empty) cart to session to ensure it's available for subsequent requests.
            if (session != null) 
            {
                try
                {
                    string newCartJson = JsonConvert.SerializeObject(cart);
                    session.SetString(CartSessionKey, newCartJson); 
                }
                catch (JsonException ex)
                {
                    Debug.WriteLine($"[SessionCart.GetCart] JSON Serialization Error when saving new cart: {ex.Message}.");
                }
            }
        }
        return cart;
    }


    /// Saves the provided cart object to the HTTP session.
    /// The HTTP session to save the cart to.
    /// The cart object to save.
    public static void SaveCart(ISession session, Cart cart)
    {
        if (session == null)
        {
            return;
        }

        if (cart == null)
        {
            return;
        }

        try
        {
            string cartJsonToSave = JsonConvert.SerializeObject(cart);
            session.SetString(CartSessionKey, cartJsonToSave);
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"[SessionCart.SaveCart] JSON Serialization Error: {ex.Message}. Cart NOT saved.");
        }
    }
}
