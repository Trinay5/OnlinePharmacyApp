using Microsoft.AspNetCore.Mvc;
using OnlinePharmacyAppMVC.DTO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class RegisterController : Controller
{
    private readonly HttpClient _client;

    public RegisterController()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7269/api/")
        };
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new RegisterUserDTO());
    }

    [HttpPost]
    public async Task<IActionResult> Index(RegisterUserDTO model)
    {
        // Map to original DTO to send to API
        var userDTO = new UserDTO
        {
            userName = model.userName,
            email = model.email,
            password = model.password,
            phoneNumber = model.phoneNumber,
            address = model.address,
            isAdmin = false // force regular user
        };

        var response = await _client.PostAsJsonAsync("User", userDTO);
        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Registration successful!";
            return RedirectToAction("Login", "Home");
        }
        else
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Registration failed: {response.StatusCode} - {errorMsg}";
            return View(model);
        }
    }
}
