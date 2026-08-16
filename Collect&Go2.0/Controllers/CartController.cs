using Collect_Go2._0.Helpers;
using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collect_Go2._0.Controllers
{
    [Authorize(Roles = "client")]
    public class CartController : Controller
    {
        private const string SessionKey = "Cart";

        private readonly IProductDAL _productDal;

        public CartController(IProductDAL productDal)
        {
            _productDal = productDal;
        }

        public IActionResult Index()
        {
            Order cart = HttpContext.Session.GetObject<Order>(SessionKey) ?? new Order();

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            Product? product = await Product.GetByIdAsync(productId, _productDal);

            if (product == null)
            {
                return NotFound();
            }

            Order cart = HttpContext.Session.GetObject<Order>(SessionKey) ?? new Order();

            cart.AddItem(product, quantity);

            HttpContext.Session.SetObject(SessionKey, cart);

            return RedirectToAction("Index", "Products");
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            Order? cart = HttpContext.Session.GetObject<Order>(SessionKey);

            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObject(SessionKey, cart);
            }

            return RedirectToAction("Index");
        }
    }
}
