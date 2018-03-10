using System.Linq;
using System.Web.Mvc;
using ValueVille.Models;

namespace ValueVille.Controllers
{
    public class ShoppingCartController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                CartId = cart.GetCartId(this.HttpContext),
                ShippingCost = cart.GetShippingCost()

            };

            if (viewModel.CartCount > 0)
            {
                viewModel.CartTotal += cart.GetShippingCost();
            }

            return View(viewModel);
        }
        
        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            var addedProduct = db.Products.Single(p => p.Id == id);
            var productName = addedProduct.Name;

            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);

            int itemCount = cart.AddItemToCart(addedProduct);

            var results = new ShoppingCartAddViewModel
            {
                Message = Server.HtmlEncode(productName) +
                    " has been added to your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                CartItemQuantity = itemCount,
                AddId = id
            };

            return Json(results);
        }

        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            var productName = db.Carts.Single(c => c.RecordId == id).Product.Name;

            int itemCount = cart.RemoveItemFromCart(id);

            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(productName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);

            var results = new ShoppingCartRemoveViewModel
            {
                CartCount = cart.GetCount()
            };

            return Json(results);
        }

        public ActionResult EmptyCart()
        {
            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);

            cart.EmptyCart();

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);

            return View(product);
        }

         
    }
}