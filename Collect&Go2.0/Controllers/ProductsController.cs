using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collect_Go2._0.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductDAL _productDal;
        private readonly ICategoryDAL _categoryDal;

        public ProductsController(IProductDAL productDal, ICategoryDAL categoryDal)
        {
            _productDal = productDal;
            _categoryDal = categoryDal;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            List<Product> products;

            if (categoryId.HasValue)
            {
                Category? category = await Category.GetByIdAsync(categoryId.Value, _categoryDal);

                if (category == null)
                {
                    return NotFound();
                }

                await category.LoadProductsAsync(_productDal);
                products = category.Products;
            }
            else
            {
                products = await Product.GetAllAsync(_productDal);
            }

            ViewBag.Categories = await Category.GetAllAsync(_categoryDal);
            ViewBag.SelectedCategory = categoryId;

            return View(products);
        }
    }
}
