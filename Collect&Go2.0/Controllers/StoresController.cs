using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Collect_Go2._0.Controllers
{
    [Authorize]
    public class StoresController : Controller
    {
        private readonly IStoreDAL _storeDal;

        public StoresController(IStoreDAL storeDal)
        {
            _storeDal = storeDal;
        }

        public async Task<IActionResult> Index()
        {
            List<Store> stores = await Store.GetAllAsync(_storeDal);

            return View(stores);
        }
    }
}
