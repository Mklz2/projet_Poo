using Collect_Go2._0.DAL;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collect_Go2._0.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private ProductRepository _repository;

        public ProductController()
        {
            _repository = new ProductRepository();
        }

        public IActionResult Index(int? categoryId)
        {
            List<Product> product;

            if (categoryId.HasValue)
            {
                product = _repository.GetProductByCategory(categoryId.Value);
            }
            else
            {
                product = _repository.GetAllProduct();
            }

            List<Category> categories = _repository.GetAllCategories();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            return View(product);
        }
    }
}