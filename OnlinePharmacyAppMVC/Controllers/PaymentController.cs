using Microsoft.AspNetCore.Mvc;
using OnlinePharmacyAppMVC.DTO;

namespace OnlinePharmacyAppMVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _client;
        public PaymentController()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7269/api/")
            };
        }
        [HttpGet]
        public IActionResult Index(decimal amount)
        {
            ViewBag.AmountToPay = amount;
            return View();
        }

        //[HttpPost]
        //public IActionResult ProcessPayment(decimal amount, string paymentMethod)
        //{
        //    // TODO: Integrate with payment gateway or simulate success
        //    TempData["Message"] = $"Payment of ₹{amount} via {paymentMethod} was successful!";
        //    return RedirectToAction("Confirmation");
        //}

        public IActionResult Confirmation()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(decimal totalAmount)
        {
            var userIdString = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                TempData["Error"] = "User session expired. Please login again.";
                return RedirectToAction("Login", "Home");
            }

            //decimal totalAmount = 0;
            if (TempData["FinalTotal"] != null)
            {
                decimal.TryParse(TempData["FinalTotal"].ToString(), out totalAmount);
                TempData.Keep("FinalTotal"); // Optional: reuse if needed on redirect
            }

            var order = new OrderDTO
            {
                userId = userId,
                totalAmount = totalAmount,
                status = "Placed",
                orderDate = DateOnly.FromDateTime(DateTime.Now),

            };

            var response = await _client.PostAsJsonAsync("Order", order);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = " ";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "There was a problem placing your order.";
            return RedirectToAction("Index");
        }
    }
}
