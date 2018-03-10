using System;
using System.Linq;
using System.Web.Mvc;
using ValueVille.Models;

namespace ValueVille.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Checkout/AddressAndPayment
        public ActionResult AddressAndPayment()
        {
            AddressAndPaymentViewModel model = new AddressAndPaymentViewModel();

            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            var email = User.Identity.Name;

            model.OrderDetails = new Order();
            model.OrderDetails.CartCount = cart.GetCount();
            model.OrderDetails.Total = cart.GetTotal();

            model.CartCount = cart.GetCount();
            model.CartId = cart.GetCartId(this.HttpContext);

            return View(model);
        }

        [HttpGet]
        public ActionResult BillingAddress()
        {
            AddressAndPaymentViewModel model = new AddressAndPaymentViewModel();

            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            var email = User.Identity.Name;
            model.OrderDetails = new Order();
            model.OrderDetails.CartCount = cart.GetCount();
            model.OrderDetails.Total = cart.GetTotal();

            model.CartCount = cart.GetCount();
            model.CartId = cart.GetCartId(this.HttpContext);

            return View(model);
        }

        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public ActionResult BillingAddress(AddressAndPaymentViewModel model)
        {

            try
            {
                var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
                model.OrderDetails.CartCount = cart.GetCount();

                model.OrderDetails.Email = User.Identity.Name;
                model.OrderDetails.OrderDate = DateTime.Now;
                model.OrderDetails.Status = "In Progress";
                //Save Order
                db.Orders.Add(model.OrderDetails);
                db.SaveChanges();
                //Process the order
                cart.CreateOrder(model.OrderDetails);

                return RedirectToAction("Complete",
                new { id = model.OrderDetails.OrderId, CartCount = 0 });

            }
            catch
            {
                // Invalid - redisplay with errors
                return View(model);
            }
        }

        //
        // GET: /Checkout/Complete
        public ActionResult Complete(int id, int CartCount)
        {
            // Validate customer owns this order
            bool isValid = db.Orders.Any(
                o => o.OrderId == id &&
                o.Email == User.Identity.Name);

            var model = new CompleteViewModel
            {
                OrderId = id,
                CartCount = CartCount
            };

            if (isValid)
            {
                return View(model);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult Complete_Order()
        {
            var model = new CompleteViewModel
            {
                CartCount = 0
            };
            return View(model);
        }


    }
}