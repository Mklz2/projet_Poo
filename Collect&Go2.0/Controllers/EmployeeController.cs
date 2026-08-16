using Collect_Go2._0.Interfaces;
using Collect_Go2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Collect_Go2._0.Controllers
{
    [Authorize(Roles = "orderpicker,cashier")]
    public class EmployeeController : Controller
    {
        private readonly IOrderDAL _orderDal;

        public EmployeeController(IOrderDAL orderDal)
        {
            _orderDal = orderDal;
        }

        private int CurrentStoreId => int.Parse(User.FindFirstValue("StoreId")!);

        // Préparateur : commandes de son magasin, pour LE LENDEMAIN, à préparer
        // Caissier : commandes de son magasin, pour AUJOURD'HUI, prêtes à être remises
        public async Task<IActionResult> Dashboard()
        {
            if (User.IsInRole("orderpicker"))
            {
                DateTime tomorrow = DateTime.Today.AddDays(1);
                List<Order> orders = await Order.GetByStoreAndStatusAsync(CurrentStoreId, OrderStatus.Placed, tomorrow, _orderDal);
                return View("PickerDashboard", orders);
            }

            DateTime today = DateTime.Today;
            List<Order> readyOrders = await Order.GetByStoreAndStatusAsync(CurrentStoreId, OrderStatus.Prepared, today, _orderDal);
            return View("CashierDashboard", readyOrders);
        }

        [HttpPost]
        [Authorize(Roles = "orderpicker")]
        public async Task<IActionResult> MarkPrepared(int orderId, int numberOfBoxes)
        {
            Order? order = await Order.GetByIdAsync(orderId, _orderDal);

            if (order == null || order.Store?.StoreId != CurrentStoreId)
            {
                return NotFound();
            }

            await order.MarkPreparedAsync(_orderDal, numberOfBoxes);

            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "cashier")]
        public async Task<IActionResult> Invoice(int orderId)
        {
            Order? order = await Order.GetByIdAsync(orderId, _orderDal);

            if (order == null || order.Store?.StoreId != CurrentStoreId)
            {
                return NotFound();
            }

            return View(order);
        }

        // AJAX GET /Employee/PreviewTotal — recalcul du montant en direct, toujours calculé par Order, jamais par la vue
        [Authorize(Roles = "cashier")]
        public async Task<IActionResult> PreviewTotal(int orderId, int returnedBoxes)
        {
            Order? order = await Order.GetByIdAsync(orderId, _orderDal);

            if (order == null || order.Store?.StoreId != CurrentStoreId)
            {
                return NotFound();
            }

            return Json(new { total = order.GetFinalTotal(returnedBoxes) });
        }

        [HttpPost]
        [Authorize(Roles = "cashier")]
        public async Task<IActionResult> Approve(int orderId, int returnedBoxes)
        {
            Order? order = await Order.GetByIdAsync(orderId, _orderDal);

            if (order == null || order.Store?.StoreId != CurrentStoreId)
            {
                return NotFound();
            }

            await order.ApproveOrderAsync(_orderDal, returnedBoxes);

            return RedirectToAction("Dashboard");
        }
    }
}
