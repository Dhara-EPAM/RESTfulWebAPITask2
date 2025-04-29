using RESTfulWebAPITask2.Model;

namespace RESTfulWebAPITask2.Services
{
    public interface ICartItemService
    {
        Cart GetCartItems(string cartId);
        Cart AddCartItem(string cartId, CartItem cartItem);
        void DeleteCartItem(CartItem cartItem);
        List<Cart> GetAllCartswithItems();
    }
}
