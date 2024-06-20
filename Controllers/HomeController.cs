using Microsoft.AspNetCore.Mvc;
using Papeleria_MVC.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Papeleria_MVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7222/api/";
        // Configuración de serialización de JSON para que las propiedades de los objetos se conviertan a minúsculas
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
            _client.BaseAddress = new Uri(_baseUrl);
        }

        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Login");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
