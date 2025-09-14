using Microsoft.AspNetCore.Mvc;
using OnlinePharmacyAppMVC.DTO;
using OnlinePharmacyAppMVC.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace OnlinePharmacyAppMVC.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class CartController : Controller
    {
        private readonly HttpClient _client;

        public CartController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7269/api/");
        }
        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.UserId = userId;


            var response = await _client.GetAsync($"https://localhost:7269/api/Cart/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var cartItems = await response.Content.ReadFromJsonAsync<List<CartModel>>();
                return View(cartItems);
            }

            return View(new List<CartModel>());
        }
        public async Task<IActionResult> ApplyDiscount()
        {
            var userIdString = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Home");
            }

            // Fetch cart
            var cartResponse = await _client.GetAsync($"https://localhost:7269/api/Cart/{userId}");
            if (!cartResponse.IsSuccessStatusCode)
                return View("Error");

            var cartItems = await cartResponse.Content.ReadFromJsonAsync<List<CartModel>>();
            var subtotal = cartItems.Sum(i => i.Amount);

            // Fetch discount
            var discountResponse = await _client.GetAsync($"https://localhost:7269/api/Discount/apply?userId={userId}&subtotal={subtotal}");
            if (!discountResponse.IsSuccessStatusCode)
                return View("Error");

            var discount = await discountResponse.Content.ReadFromJsonAsync<DiscountResponseDTO>();

            var viewModel = new CartPageViewModel
            {
                CartItems = cartItems,
                DiscountAmount = discount.DiscountAmount,
                DiscountCode = discount.DiscountCode,
                IsPercentage = discount.IsPercentage
            };

            return View("DiscountSummary", viewModel);
        }


    }
}

        //[HttpGet]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public async Task<IActionResult> Index()
        //{
        //    string userIdStr = HttpContext.Session.GetString("userId");
        //    string isAdmin = HttpContext.Session.GetString("isAdmin");

        //    if (string.IsNullOrEmpty(userIdStr) || isAdmin == "True")
        //    {
        //        TempData["Error"] = "Access denied. Please log in with a customer account.";
        //        return RedirectToAction("Login", "Home");
        //    }

        //    if (!int.TryParse(userIdStr, out int userId))
        //    {
        //        TempData["Error"] = "Session error.";
        //        return RedirectToAction("Login", "Home");
        //    }

        //    // Fetch cart items
        //    var cartResponse = await _client.GetAsync($"Cart/{userId}");
        //    var cartItems = new List<CartModel>();

        //    if (cartResponse.IsSuccessStatusCode)
        //    {
        //        cartItems = await cartResponse.Content.ReadFromJsonAsync<List<CartModel>>();

        //        // Fetch medicine data for each item to get available stock
        //        foreach (var item in cartItems)
        //        {
        //            var medResponse = await _client.GetAsync($"Medicine/{item.MedicineId}");
        //            if (medResponse.IsSuccessStatusCode)
        //            {
        //                var medicine = await medResponse.Content.ReadFromJsonAsync<MedicineDTO>();
        //                item.AvailableQty = medicine?.stockQty ?? item.StockQty; // fallback to current cart qty
        //            }
        //            else
        //            {
        //                item.AvailableQty = item.StockQty;
        //            }
        //        }
        //    }

        //    // Fetch discount
        //    var discountResponse = await _client.GetAsync($"Discount/GetByUserId/{userId}");
        //    DiscountDTO discount = null;

        //    if (discountResponse.IsSuccessStatusCode)
        //    {
        //        discount = await discountResponse.Content.ReadFromJsonAsync<DiscountDTO>();
        //    }

        //    // Calculate totals
        //    decimal total = cartItems.Sum(i => i.Amount);
        //    decimal discountAmt = 0;

        //    if (discount != null)
        //    {
        //        discountAmt = discount.IsPercentage ? total * (discount.Value / 100) : discount.Value;
        //        ViewBag.DiscountCode = discount.DiscountCode;
        //        ViewBag.DiscountType = discount.DiscountType;
        //    }

        //    ViewBag.Total = total;
        //    ViewBag.DiscountAmount = discountAmt;
        //    ViewBag.FinalAmount = total - discountAmt;

        //    return View(cartItems);
        //}