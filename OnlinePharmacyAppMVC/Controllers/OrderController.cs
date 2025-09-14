using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using OnlinePharmacyAppMVC.DTO;
using OnlinePharmacyAppMVC.Models;
using System.Text.Json;

namespace OnlinePharmacyAppMVC.Controllers
{

    public class OrderController : Controller
    {
        private readonly HttpClient _client;

        public OrderController()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7269/api/")
            };
        }
        public async Task<IActionResult> ViewOrder()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7269/api/");
            HttpResponseMessage msg = await client.GetAsync("Order");
            msg.EnsureSuccessStatusCode();
            string respstring = await msg.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<GetOrder>(respstring);
            return View(list);

        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            string userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");

            var response = await _client.GetAsync($"Order/UserOrders/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load orders.";
                return View(new GetOrder { data = new List<OrderDTO>() });
            }

            var result = await response.Content.ReadFromJsonAsync<GetOrder>();
            return View(result);
        }
        public async Task<IActionResult> ConfirmOrder()
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
                var cartJson = JsonSerializer.Serialize(cartItems);
                HttpContext.Session.SetString("Cart", cartJson);

                return View(cartItems);
            }

            return View(new List<CartModel>());
        }
        [HttpPost]
        public IActionResult DownloadPdfReport()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
                return NotFound("No cart data found.");

            var cart = JsonSerializer.Deserialize<List<CartModel>>(cartJson);

            using var stream = new MemoryStream();
            var doc = new iTextSharp.text.Document();
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
            doc.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            doc.Add(new Paragraph("Order Summary", titleFont));
            doc.Add(new Paragraph($"Generated on: {DateTime.Now}", FontFactory.GetFont(FontFactory.HELVETICA, 10)));
            doc.Add(new Paragraph(" ")); // Spacer

            // Table
            var table = new iTextSharp.text.pdf.PdfPTable(4);
            table.AddCell("Medicine Name");
            table.AddCell("Quantity");
            table.AddCell("Price (each)");
            table.AddCell("Amount");

            foreach (var item in cart)
            {
                table.AddCell(item.MedName);
                table.AddCell(item.StockQty.ToString());
                table.AddCell(item.Price.ToString("F2"));
                table.AddCell(item.Amount.ToString("F2"));
            }

            doc.Add(table);
            doc.Close();

            var pdfBytes = stream.ToArray();
            return File(pdfBytes, "application/pdf", "OrderSummary.pdf");
        }


    }


}
