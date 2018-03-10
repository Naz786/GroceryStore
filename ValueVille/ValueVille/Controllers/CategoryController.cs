using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using ValueVille.Models;


namespace ValueVille.Controllers
{
   
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Category
        public async Task<ActionResult> Index()
        {
            var categories = (await db.Categories.ToListAsync()).Select<Category, CategoryViewModel>(x => x);


            return View(categories);
        }

        // GET: Category/Details/5
        public async Task<ActionResult> Details(int? id, string categoryName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CategoryViewModel model = await db.Categories.FindAsync(id);

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Image")] CategoryViewModel viewModel)
        {
            //&& viewModel.Image != null
            if (ModelState.IsValid && viewModel.Image != null)
            {
                db.Categories.Add(viewModel);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            
            return View(viewModel);
        } 

       

        // GET: Category/Edit/5
        public async Task<ActionResult> Edit(int? id, string categoryName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //return db.Categories.FirstOrDefault(c => c.Id == id);
            CategoryViewModel model = await db.Categories.FindAsync(id);  

            if (model == null)
            {
                return HttpNotFound();
            }
            
            
            return View(model);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Image")] int? id, CategoryViewModel model)
        {
            
            var oldImage = db.Categories.Where(c => c.Id == model.Id).Select(c => c.Image).FirstOrDefault();

            if (ModelState.IsValid)
            {
                Category ModifiedCategory = model; 

                if (ModifiedCategory.Image == null)
                {
                    ModifiedCategory.Image = oldImage;
                }
                db.Entry(ModifiedCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index/" + id);
            }
            return View(model);
        }

        // GET: Category/Delete/5
        public async Task<ActionResult> Delete(int? id, string categoryName)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CategoryViewModel category = await db.Categories.FindAsync(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Category category = await db.Categories.FindAsync(id);
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
