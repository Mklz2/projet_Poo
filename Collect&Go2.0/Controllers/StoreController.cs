using Collect_Go2._0.DAL;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Collect_Go2._0.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            StoreRepository repository = new StoreRepository();

            List<Store> stores = repository.GetAllStores();

            return View(stores);
        }
    }
}