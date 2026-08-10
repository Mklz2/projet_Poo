using Collect_Go2._0.DAL;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collect_Go2._0.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private ProductsRepository _repository;

        public ProductsController()
        {
            _repository = new ProductsRepository();
        }

        public IActionResult Index(int? categoryId)
        {
            List<Products> products;

            if (categoryId.HasValue)
            {
                products = _repository.GetProductsByCategory(categoryId.Value);
            }
            else
            {
                products = _repository.GetAllProducts();
            }

            List<Category> categories = _repository.GetAllCategories();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            return View(products);
        }
    }
}