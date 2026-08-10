using Collect_Go2._0.DAL;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Collect_Go2._0.Controllers
{
    [Authorize]
    public class StoresController : Controller
    {
        public IActionResult Index()
        {
            StoresRepository repository = new StoresRepository();

            List<Stores> stores = repository.GetAllStores();

            return View(stores);
        }
    }
}