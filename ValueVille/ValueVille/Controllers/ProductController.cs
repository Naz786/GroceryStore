using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using ValueVille.Models;

namespace ValueVille
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        public async Task<ActionResult> Index(int? id, string categoryName)
        {

            var products = (await db.Categories.Where(c => c.Id == id)
                                            .SelectMany(p => p.Products.Select(x => new ProductViewModel { Id = x.Id, Name = x.Name, ByteImage = x.Image, Price = x.Price, ShowOnIndex = x.ShowOnIndex}))
                                            .ToListAsync());

            if (products == null)
            {
                return HttpNotFound();
            }

            return View();
        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id, string productName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var categoryID = db.Categories.Single(c => c.Products.Any(p => p.Id == id)).Id;
            var categoryName = db.Categories.Single(c => c.Products.Any(p => p.Id == id)).Name;

            ProductViewModel model = await db.Products.FindAsync(id);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(new ProductDetailsViewModel { Product = model, CategoryId = categoryID, CategoryName = categoryName });
        }

        // GET: Product/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            return View(new ProductViewModel() { CategoryId = id.Value });
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Image,Price,CategoryId,ShowOnIndex")] ProductViewModel viewModel)
        {

            if (ModelState.IsValid && viewModel.Image != null)
            {
                var category = await db.Categories.FindAsync(viewModel.CategoryId);

                if (category == null)
                {
                    return HttpNotFound();
                }

                category.Products.Add(viewModel);

                await db.SaveChangesAsync();
                return RedirectToAction("Index/" + viewModel.CategoryId);
            }

            return View(viewModel);
        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int? id, string productName)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductViewModel product = await db.Products.FindAsync(id);

            var categoryID = db.Categories.Single(c => c.Products.Any(p => p.Id == id)).Id;
            var categoryName = db.Categories.Single(c => c.Products.Any(p => p.Id == id)).Name;

            if (product == null)
            {
                return HttpNotFound();
            }
            return View(new ProductViewModel { Id = product.Id, Name = product.Name, ByteImage = product.ByteImage,
                                                            Price = product.Price, ShowOnIndex = product.ShowOnIndex, CategoryId = categoryID, CategoryName = categoryName });
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Image,Price")] int? id, ProductViewModel model)
        {

            var oldImage = db.Products.Where(p => p.Id == model.Id).Select(x => x.Image).FirstOrDefault();

            if (ModelState.IsValid)
            {
                Product modifiedProduct = model;

                if (modifiedProduct.Image == null)
                {
                    modifiedProduct.Image = oldImage;
                }
                db.Entry(modifiedProduct).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index/" + model.CategoryId);
            }
            return View(model);
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int? id, string productName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var categoryID = db.Categories.Single(c => c.Products.Any(p => p.Id == id)).Id;
            var categoryName = db.Categories.Single(c => c.Products.Any(p => p.Id == id)).Name;

            ProductViewModel model = await db.Products.FindAsync(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(new ProductDeleteViewModel { Product = model, CategoryId = categoryID, CategoryName = categoryName });
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);

            var categoryID = db.Categories.Single(c => c.Products.Any(p => p.Id == id)).Id;

            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index/" + categoryID);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
