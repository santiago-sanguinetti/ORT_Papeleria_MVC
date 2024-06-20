using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papeleria_MVC.Models;
using System.Text;
using System.Text.Json;

namespace Papeleria_MVC.Controllers
{

    public class LoginController : Controller
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7222/api/";
        // Configuración de serialización de JSON para que las propiedades de los objetos se conviertan a minúsculas
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public LoginController(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri(_baseUrl);
        }
        // GET: LoginController
        public ActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login");
            }

            var response = _client.GetAsync("Login").Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var login = JsonSerializer.Deserialize<List<LoginModel>>(content, _jsonOptions);
                return View(login);
            }
            return View();
        }

        // GET: LoginController/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: LoginController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginModel);
                var body = new StringContent(json, Encoding.UTF8, "application/json");
                var response = _client.PostAsync("Login", body).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var token = JsonSerializer.Deserialize<LoginToken>(content, _jsonOptions);
                    if (token == null)
                    {
                        ViewBag.ErrorMessage = "No se recibió un token de acceso.";
                        return View();
                    }
                    HttpContext.Session.SetString("Token", token.token);
                    HttpContext.Session.SetString("Role", token.role);
                    HttpContext.Session.SetString("Email", token.email);
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.token}");
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.ErrorMessage = "Credenciales inválidas.";
                return View();
            }

            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        public ActionResult Logout()
        {
            if (HttpContext.Session.GetString("Token") == null)
            {
                return RedirectToAction("Login");
            }
            try
            {
                HttpContext.Session.Clear();
                _client.DefaultRequestHeaders.Remove("Authorization");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
            
        }

    }
}
