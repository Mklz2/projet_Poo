using Collect_Go2._0.Helpers;
using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Collect_Go2._0.Controllers
{
    [Authorize(Roles = "client")]
    public class OrderController : Controller
    {
        private const string SessionKey = "Cart";

        private readonly IStoreDAL _storeDal;
        private readonly ITimeSlotDAL _timeSlotDal;
        private readonly IOrderDAL _orderDal;

        public OrderController(IStoreDAL storeDal, ITimeSlotDAL timeSlotDal, IOrderDAL orderDal)
        {
            _storeDal = storeDal;
            _timeSlotDal = timeSlotDal;
            _orderDal = orderDal;
        }

        // GET /Order/Checkout — choix du magasin puis du créneau, seulement à la confirmation
        public async Task<IActionResult> Checkout()
        {
            Order cart = HttpContext.Session.GetObject<Order>(SessionKey) ?? new Order();

            if (cart.OrderItems.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Stores = await Store.GetAllAsync(_storeDal);

            return View(cart);
        }

        // AJAX GET /Order/GetDates?storeId=X — dates disponibles pour ce magasin
        public async Task<IActionResult> GetDates(int storeId)
        {
            List<TimeSlot> available = await TimeSlot.GetAvailableAsync(storeId, _timeSlotDal);

            var dates = available
                .Select(s => s.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .Select(d => new
                {
                    value = d.ToString("yyyy-MM-dd"),
                    label = d.ToString("dddd d MMMM", new CultureInfo("fr-FR"))
                });

            return Json(dates);
        }

        // AJAX GET /Order/GetTimeSlots?storeId=X&date=yyyy-MM-dd — créneaux disponibles ce jour-là
        public async Task<IActionResult> GetTimeSlots(int storeId, string date)
        {
            DateTime parsedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            List<TimeSlot> available = await TimeSlot.GetAvailableAsync(storeId, _timeSlotDal);

            var slots = available
                .Where(s => s.Date.Date == parsedDate.Date)
                .OrderBy(s => s.StartHour)
                .Select(s => new
                {
                    timeSlotId = s.TimeSlotId,
                    label = $"{s.StartHour:HH:mm} - {s.EndHour:HH:mm}",
                    placesLeft = s.PlacesLeft
                });

            return Json(slots);
        }

        // POST /Order/PlaceOrder — confirmation finale
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int storeId, int timeSlotId)
        {
            Order? cart = HttpContext.Session.GetObject<Order>(SessionKey);

            if (cart == null || cart.OrderItems.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            Store? store = await Store.GetByIdAsync(storeId, _storeDal);
            TimeSlot? slot = await TimeSlot.GetByIdAsync(timeSlotId, _timeSlotDal);

            if (store == null || slot == null)
            {
                ModelState.AddModelError(string.Empty, "Magasin ou créneau invalide.");
                ViewBag.Stores = await Store.GetAllAsync(_storeDal);
                return View("Checkout", cart);
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            string firstname = User.FindFirstValue(ClaimTypes.GivenName)!;
            string lastname = User.FindFirstValue(ClaimTypes.Surname)!;
            string email = User.FindFirstValue(ClaimTypes.Name)!;
            string phone = User.FindFirstValue(ClaimTypes.MobilePhone)!;

            cart.Client = new Client(userId, firstname, lastname, email, string.Empty, phone);
            cart.Store = store;
            cart.TimeSlot = slot;

            try
            {
                await cart.PlaceOrderAsync(_orderDal);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Stores = await Store.GetAllAsync(_storeDal);
                return View("Checkout", cart);
            }

            HttpContext.Session.Remove(SessionKey);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        // GET /Order/History — commandes du client connecté
        public async Task<IActionResult> History()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            List<Order> orders = await Order.GetByClientAsync(userId, _orderDal);

            return View(orders);
        }
    }
}
