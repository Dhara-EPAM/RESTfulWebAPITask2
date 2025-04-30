using Elfie.Serialization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RESTfulWebAPITask2.Controllers;
using RESTfulWebAPITask2.Model;
using RESTfulWebAPITask2.Services;

namespace UnitTests
{
    public class CartTests
    {
        private Mock<ICartItemService> _cartItemService;
        private CartV1Controller _cartController;

        [SetUp]
        public void Setup()
        {
            _cartItemService = new Mock<ICartItemService>();
            _cartController = new CartV1Controller(_cartItemService.Object);
        }

        [Test]
        public void GetCart_ReturnsOkResult_WithCart()
        {
            var cartId = "cart 1";
            var expectedCart = new Cart { CartId = cartId };
            _cartItemService.Setup(s => s.GetCartItems(cartId)).Returns(expectedCart);

            var result = _cartController.GetCartItems(cartId);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.AreEqual(expectedCart, okResult.Value);
        }

        [Test]
        public void AddCartItem_ReturnsCreatedResult()
        {
            string cartId = "testCart";
            var cartItem = new CartItem
            {
                Id = 1,
                Name = "Test Product",
                Price = 100.00m,
                Quantity = 2,
                CartId = "testCart"
            };

            var addCart = new Cart
            {
                CartId = cartId,
                CartItems = new List<CartItem> { cartItem }
            };

            _cartItemService.Setup(service => service.AddCartItem(cartId, cartItem)).Returns(addCart);

            var response = _cartController.AddCartItem(cartId, cartItem);

            // Assert
            var okResult = response as OkObjectResult;
            Assert.IsNotNull(okResult);

            var json = JsonConvert.SerializeObject(okResult.Value);
            var jObject = JObject.Parse(json);

            Assert.AreEqual(addCart.CartId, jObject["Cart"]["CartId"].ToString());
        }

        [Test]
        public void DeleteCartItem_ReturnsOk_WhenItemIsDeleted()
        {
            string cartId = "testCart";
            int itemId = 1;
            var itemToDelete = new CartItem { Id = itemId, Name = "Item1", CartId = cartId };
            var cart = new Cart
            {
                CartId = cartId,
                CartItems = new List<CartItem> { itemToDelete }
            };

            _cartItemService.Setup(service => service.GetCartItems(cartId)).Returns(cart);
            _cartItemService.Setup(service => service.DeleteCartItem(itemToDelete)).Verifiable();

            var response = _cartController.DeleteCartItem(cartId, itemId);

            // Assert
            var okResult = response as OkObjectResult;
            Assert.IsNotNull(okResult);

            var responseString = okResult.Value.ToString();
            Assert.IsTrue(responseString.Contains("Item deleted successfully"));
        }

    }
}