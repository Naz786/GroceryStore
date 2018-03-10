using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Web;

namespace ValueVille.Models
{
    public class CategoryViewModel : SEOUrlCreation
    {
        public int Id { get; set; }
        [Required, Display(Name = "Category Name")]
        public string Name { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }
        public string OutputImage { get; set; }

        public static byte[] ConvertToByte(CategoryViewModel model)
        {
            if (model.Image != null)
            {
                byte[] imageByte = null;
                BinaryReader rdr = new BinaryReader(model.Image.InputStream);
                imageByte = rdr.ReadBytes((int)model.Image.ContentLength);

                return imageByte;
            }

            return null;
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

        // ViewModel => Model | Implicit type Operator
        public static implicit operator Category(CategoryViewModel viewModel)
        {
            var model = new Category
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Image = ConvertToByte(viewModel)
            };

            return model;
        }

        // Model => ViewModel | Implicit type Operator
        public static implicit operator CategoryViewModel(Category model)
        {
            var viewModel = new CategoryViewModel()
            {
                Id = model.Id,
                Name = model.Name,
                OutputImage = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(model.Image))
            };

            return viewModel;
        }
    }

    public class ProductViewModel : SEOUrlCreation
    {
        public int Id { get; set; }
        [Required, Display(Name = "Product Name")]
        public string Name { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Image { get; set; }
        public string OutputImage { get; set; }
        public Byte[] ByteImage { get; set; }
        [Required]
        public Decimal Price { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public bool ShowOnIndex { get; set; }
        public string CategoryName { get; set; }

        public static byte[] ConvertToByte(ProductViewModel model)
        {
            if (model.Image != null)
            {
                byte[] imageByte = null;
                BinaryReader rdr = new BinaryReader(model.Image.InputStream);
                imageByte = rdr.ReadBytes((int)model.Image.ContentLength);

                return imageByte;
            }

            return null;
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

        // ViewModel => Model | Implicit type Operator
        public static implicit operator Product(ProductViewModel viewModel)
        {
            var model = new Product
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Image = ConvertToByte(viewModel),
                Price = viewModel.Price,
                ShowOnIndex = viewModel.ShowOnIndex
            };

            return model;
        }

        // Model => ViewModel | Implicit type Operator
        public static implicit operator ProductViewModel(Product model)
        {
            var viewModel = new ProductViewModel()
            {
                Id = model.Id,
                Name = model.Name,
                ByteImage = model.Image,
                Price = model.Price,
                ShowOnIndex = model.ShowOnIndex,
                OutputImage = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(model.Image))
            };

            return viewModel;
        }
    }

    public class HomePageViewModel
    {
        public IEnumerable<CategoryViewModel> Categories;
        public IEnumerable<ProductViewModel> Products;
        public int CartCount;
    }

    public class ProductIndexViewModel
    {
        public IList<ProductViewModel> Products;
        public int? CategoryId;
        public int CartCount;
    }

    public class ProductDetailsViewModel : SEOUrlCreation
    {
        public ProductViewModel Product;
        public int? CategoryId;
        public string CategoryName;
    }

    public class ProductDeleteViewModel : SEOUrlCreation
    {
        public ProductViewModel Product;
        public int? CategoryId;
        public string CategoryName;
    }

    public class AddCSVDataViewModel
    {
        [Display(Name = "CSV File")]
        [Required]
        public HttpPostedFileBase CSVFile { get; set; }
    }

    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public string CartId { get; set; }

        public int CartCount;
        public decimal ShippingCost { get; set; }
    }

    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
    }

    public class ShoppingCartAddViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int CartItemQuantity { get; set; }
        public int AddId { get; set; }

    }

    public class CompleteViewModel
    {
        public int CartCount { get; set; }
        public int OrderId { get; set; }
    }

    public class AddressAndPaymentViewModel
    {
        [Required]
        public Order OrderDetails { get; set; }
        public int CartCount { get; set; }
        public string CartId { get; set; }

        public bool IsAnyNullOrEmpty(Order order)
        {
            foreach (PropertyInfo pi in order.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(order);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class AccountOrdersViewModel
    {
        public List<Order> Orders;
        public List<OrderDetail> OrderDetails;
        public int CartCount;
        public Order Order;
    }

    public class AdminOrdersViewModel
    {
        public List<Order> OrdersInProgress { get; set; }
        public List<Order> OrdersDespatched { get; set; }
        public List<Status> OrderStatuses { get; set; }
        public string Message { get; set; }
    }

    public class Status
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }

    public class UpdatedOrder
    {
        public int id { get; set; }
        public string newStatus { get; set; }
    }
}