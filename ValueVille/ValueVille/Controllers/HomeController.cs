using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ValueVille.Models;

namespace ValueVille.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private async Task SendMail(Contact contact)
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mail = new MailMessage();
            //Setting From , To and CC
            mail.From = new MailAddress("info@valueville.co.uk", "ValueVille");
            mail.To.Add(new MailAddress(contact.EmailAddress));
            mail.Bcc.Add(new MailAddress("info@smokin-donut.com"));
            mail.Bcc.Add(new MailAddress("info@valueville.co.uk"));
            mail.Subject = "Thanks For Signing Up";
            ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            string messageBody = Resource.registration_confirmation_email.Replace("{0}", contact.FirstName);
            AlternateView alternate = AlternateView.CreateAlternateViewFromString(messageBody, mimeType);
            mail.AlternateViews.Add(alternate);
            await smtpClient.SendMailAsync(mail);
        }

        public async Task<ActionResult> Index(string message = null)
        {
            ViewBag.Message = message;
            var categories = (await db.Categories.ToListAsync()).Select<Category, CategoryViewModel>(x => x);

            var specialProducts = (await db.Products.Where(s => s.ShowOnIndex == true).ToListAsync()).Select<Product, ProductViewModel>(x => x);

            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            var cartCount = cart.GetCount();

            return View(new HomePageViewModel { Categories = categories, Products = specialProducts, CartCount = cartCount});
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult CreateContact()
        {
            return View("LaunchPromotionSignUp");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateContact([Bind(Include = "Id,FirstName,LastName,EmailAddress")] Contact contact)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await db.Contacts.Where(c => c.EmailAddress == contact.EmailAddress).FirstOrDefaultAsync() != null)
                        ModelState.AddModelError(string.Empty, "You can only register your email address once.");
                    else
                    {
                        db.Contacts.Add(contact);
                        await db.SaveChangesAsync();
                        await SendMail(contact);
                        return RedirectToAction("Index", new { message = "Thanks for signing up with ValueVille " + contact.FirstName + ". We'll be in touch soon." });
                    }
                }
            }
            catch( Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                ModelState.AddModelError(string.Empty, "An error occurred updating the database. Please try a different email address");
            }
            return View("LaunchPromotionSignUp");
        }

        [AllowAnonymous]
        public async Task<ActionResult> Products(int? id)
        {
            //ViewBag.Message = "Your application description page.";
            var model = (await db.Categories.Where(c => c.Id == id)
                                            .SelectMany(p => p.Products.Select(x => new ProductViewModel() { Id = x.Id, Name = x.Name, ByteImage = x.Image, Price = x.Price }))
                                            .ToListAsync());

            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            var cartCount = cart.GetCount();

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(new ProductIndexViewModel { Products = model, CategoryId = id, CartCount = cartCount });
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Orders()
        {
            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            var cartCount = cart.GetCount();

            var orders = db.Orders.Where(o => o.Email == User.Identity.Name).ToList();

            return View(new AccountOrdersViewModel { Orders = orders, CartCount = cartCount });
        }

        [Authorize]
        [HttpGet]
        public ActionResult OrderDetails(int id)
        {
            var cart = ShoppingCart.GetShoppingCart(this.HttpContext);
            var cartCount = cart.GetCount();
            var orderDetails = db.OrderDetails.Where(o => o.OrderId == id).ToList();
            var order = db.Orders.Where(o => o.OrderId == id).Single();
                 
            return View(new AccountOrdersViewModel { OrderDetails = orderDetails, CartCount = cartCount, Order = order });
        }
    }
}