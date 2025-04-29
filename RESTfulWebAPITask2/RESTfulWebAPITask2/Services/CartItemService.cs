using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulWebAPITask2.Model;

namespace RESTfulWebAPITask2.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly CartDBContext _dbContext;
        public CartItemService(CartDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Cart GetCartItems(string cartId)
        {
            return _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CartId == cartId);
        }

        public Cart AddCartItem(string cartId, CartItem cartItem)
        {
            //get car & it's items
            var cart = _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.CartId == cartId);

            //if cart is new then create cart
            if (cart == null)
            {
                cart = new Cart { CartId = cartId };
                _dbContext.Carts.Add(cart);
            }

            //add items to the cart
            cartItem.CartId = cartId;
            _dbContext.CartItems.Add(cartItem);

            _dbContext.SaveChanges();

            return cart;
        }

        public void DeleteCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Remove(cartItem);
            _dbContext.SaveChanges();
        }

        public List<Cart> GetAllCartswithItems()
        {
            return _dbContext.Carts
               .Include(c => c.CartItems).ToList();
        }
    }
}
