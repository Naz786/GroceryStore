using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ValueVille.Models
{

    public class Repository 
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static List<Product> ProductsList { get; set; }
        public static List<Category> CategoriesList { get; set; }

        public static void CSVToList(HttpPostedFileBase CSVFile)
        {

            if (CSVFile == null || CSVFile.ContentLength == 0)
            {
                throw new InvalidOperationException("No file has been selected for upload or the file is empty.");
            }

            // saves the file into a directory in the App_Data folder
            var fileName = Path.GetFileName(CSVFile.FileName);
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), fileName);
            CSVFile.SaveAs(path);

            // String array of all the lines - All records
            var records = File.ReadLines(path).Skip(1);

            // Creates Product objects from array of records
            // At present, images are assumed void
            // When CSV data contains images, then this code will need changing slightly.
            var CSVProducts = from line in records
                              let columns = line.Split(',')
                              select new Product
                              {
                                  Id = int.Parse(columns[0]),
                                  Barcode = columns[13],
                                  Name = columns[1],
                                  CategoryName = CapitaliseWords(columns[9]),
                                  Description = columns[2],
                                  Price = decimal.Parse(columns[4])
                              };

            ProductsList = CSVProducts.ToList();

            var CSVCategories = File.ReadLines(path).Skip(1)
                               .Select(l => l.Split(new[] { ',' })[9])
                               .Where(category => category.Length > 0)
                               .Distinct(StringComparer.InvariantCultureIgnoreCase)
                               .Select(s => new Category { Name = CapitaliseWords(s) });

            CategoriesList = CSVCategories.ToList();

        }

        public static string CapitaliseWords(string name)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            name = textInfo.ToTitleCase(name);
            return name;
        }

        public static byte[] ConvertToByte(string image)
        {
            if (image != null)
            {
                byte[] imageByte = null;
                string path = HttpContext.Current.Server.MapPath(image);
                imageByte = File.ReadAllBytes(path);

                return imageByte;
            }

            return null;
        }

        public static async Task SaveProducts()
        {
            var DefaultImage = "/Content/Images/add-image.jpg";
            
            foreach (var category in CategoriesList)
            {
                // Replaces null image fields with temporary image 
                if (category.Image == null)
                {
                    category.Image = ConvertToByte(DefaultImage);
                }
                foreach (var product in ProductsList)
                {
                    // Replaces null image fields with temporary image 
                    if (product.Image == null)
                    {
                        product.Image = ConvertToByte(DefaultImage);
                    }
                    // Checks if product belongs to a category by matching categoryName
                    if (product.CategoryName == category.Name)
                    {
                        // Only adds to table if not already existing in table
                        if (!db.Categories.Any(c => c.Products.Any(p => p.Name == product.Name)))
                        {
                            category.Products.Add(product);
                        }
                    }
                }
                // Only adds to table if not already existing in table
                if (!db.Categories.Any(c => c.Name == category.Name))
                {
                    db.Categories.Add(category);
                }
                
            }
            await db.SaveChangesAsync();
        }

    }


}

