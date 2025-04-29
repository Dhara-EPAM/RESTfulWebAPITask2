using Microsoft.AspNetCore.Mvc;
using RESTfulWebAPITask2.Model;
using RESTfulWebAPITask2.Services;

namespace RESTfulWebAPITask2.Controllers
{
    [Route("api/v2/cart")]
    [ApiController]
    public class CartV2Controller : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        public CartV2Controller(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        // GET api/v2/cart
        [HttpGet("{cartId}")]
        public IActionResult GetCartItems(string cartId)
        {
            var cart = _cartItemService.GetCartItems(cartId);
            if (cart == null)
                return NotFound(new { Message = "Cart not found" });

            if (cart.CartItems == null)
                return NotFound(new { Message = "Item not found" });

            return Ok(cart.CartItems);
        }


        // POST api/v2/cart
        [HttpPost("{cartId}")]
        public IActionResult AddCartItem(string cartId, CartItem cartItem)
        {
            if (cartItem == null)
                return BadRequest("Item details are required");

            var cart = _cartItemService.AddCartItem(cartId, cartItem);

            return Ok(new { Message = "Item has been added successfully", Cart = cart });
        }

        // DELETE api/v2/cart/cart1/2
        [HttpDelete("{cartId}/{id}")]
        public IActionResult DeleteCartItem(string cartId, int id)
        {
            //Get cart & it's item
            var cart = _cartItemService.GetCartItems(cartId);

            if (cart == null)
                return NotFound(new { Message = "Cart not found" });

            var item = cart.CartItems.FirstOrDefault(i => i.Id == id);
            if (item == null)
                return NotFound(new { Message = "Item not found" });

            //Call delete service
            _cartItemService.DeleteCartItem(item);

            return Ok(new { Message = "Item deleted successfully" });
        }


        // GET api/v1/cart
        [HttpGet("")]
        public IActionResult GetAllCartswithItems()
        {
            var cart = _cartItemService.GetAllCartswithItems();

            if (cart == null)
                return NotFound(new { Message = "Cart not found" });

            return Ok(cart);
        }
    }
}
