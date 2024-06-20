using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Papeleria_MVC.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Papeleria_MVC.Controllers
{
    public class StockMovementController : Controller
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7222/api/";
        // Configuración de serialización de JSON para que las propiedades de los objetos se conviertan a minúsculas
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public StockMovementController(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri(_baseUrl);
        }

        // GET: StockMovementController
        public ActionResult Index()
        {
            var token = HttpContext.Session.GetString("Token");
            var request = new HttpRequestMessage(HttpMethod.Get, "StockMovement");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = _client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var stockMovements = JsonSerializer.Deserialize<List<StockMovementViewModel>>(content, _jsonOptions);
                if (!stockMovements.Any() || stockMovements == null)
                {
                    ViewBag.Message = "No hay movimientos de stock.";
                    return View(new List<StockMovementViewModel>());
                }
                return View(stockMovements);
            }
            return View();
        }

        // GET: StockMovementController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = "No se proporcionó un identificador de movimiento de stock.";
                return View();
            }
            try
            {
                var response = _client.GetAsync($"StockMovement/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var stockMovement = JsonSerializer.Deserialize<StockMovementViewModel>(content, _jsonOptions);
                    return View(stockMovement);
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }

        }

        // GET: StockMovementController/Create
        public async Task<ActionResult> Create()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var request = new HttpRequestMessage(HttpMethod.Get, "StockMovement");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var maxItemQuantityResponse = await _client.GetAsync($"Params/MaxItemQuantity");
                if (!maxItemQuantityResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "Error al obtener la cantidad máxima de artículos. Intente más tarde.";
                    TempData["MaxItemQuantity"] = 0;
                    return View();
                }
                var contentMaxItemQuantity = await maxItemQuantityResponse.Content.ReadAsStringAsync();
                int intMaxItemQuantity = int.Parse(contentMaxItemQuantity);
                TempData["MaxItemQuantity"] = intMaxItemQuantity;

                var itemsResponse = await _client.GetAsync("Items");
                if (!itemsResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "Error al obtener los artículos. Intente más tarde.";
                    return View();
                }
                var contentItems = await itemsResponse.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<ItemsViewModel>>(contentItems, _jsonOptions);

                var movementTypesResponse = await _client.GetAsync("MovementTypes");
                if (!movementTypesResponse.IsSuccessStatusCode)
                {
                    ViewBag.ErrorMessage = "Error al obtener los tipos de movimiento. Intente más tarde.";
                    return View();
                }
                var contentMovementTypes = await movementTypesResponse.Content.ReadAsStringAsync();
                var movementTypes = JsonSerializer.Deserialize<List<MovementTypesViewModel>>(contentMovementTypes, _jsonOptions);

                var createStockMovementViewModel = new CreateStockMovementViewModel
                {
                    Items = new SelectList(items ?? new List<ItemsViewModel>(), "Id", "Name"),
                    MovementTypes = new SelectList(movementTypes ?? new List<MovementTypesViewModel>(), "Id", "Name")
                };

                return View(createStockMovementViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }

        // POST: StockMovementController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateStockMovementViewModel createStockMovementViewModel)
        {
            if (createStockMovementViewModel.ItemId <= 0 || createStockMovementViewModel.ItemQuantity <= 0 || createStockMovementViewModel.MovementTypeId <= 0)
            {
                TempData["ErrorMessage"] = "Error al crear el movimiento de stock. Verifique los datos.";
                return RedirectToAction("Create");
            }
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var request = new HttpRequestMessage(HttpMethod.Get, "StockMovement");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = System.Text.Json.JsonSerializer.Serialize(createStockMovementViewModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("StockMovement", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Movimiento de stock creado exitosamente.";
                    return RedirectToAction("Create");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["ErrorMessage"] = "Error al crear el movimiento de stock. Verifique los datos.";
                    return RedirectToAction("Create");
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    TempData["ErrorMessage"] = "Error al crear el movimiento de stock. Intente más tarde.";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al crear el movimiento de stock.";
                    return RedirectToAction("Create");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Create");
            }
        }

        // GET StockMovement/GetByArticleAndMovementType
        public async Task<ActionResult> GetByArticleAndMovementType()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var request = new HttpRequestMessage(HttpMethod.Get, "StockMovement");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var itemsResponse = await _client.GetAsync("Items");
                if (!itemsResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error al obtener los artículos. Intente más tarde.";
                    return RedirectToAction("Index");
                }
                var contentItems = await itemsResponse.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<ItemsViewModel>>(contentItems, _jsonOptions);

                var movementTypesResponse = await _client.GetAsync("MovementTypes");
                if (!movementTypesResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error al obtener los tipos de movimiento. Intente más tarde.";
                    return RedirectToAction("Index");
                }
                var contentMovementTypes = await movementTypesResponse.Content.ReadAsStringAsync();
                var movementTypes = JsonSerializer.Deserialize<List<MovementTypesViewModel>>(contentMovementTypes, _jsonOptions);

                var getByArticleAndMovementTypeModel = new GetByArticleAndMovementTypeModel
                {
                    Items = new SelectList(items ?? new List<ItemsViewModel>(), "Id", "Name"),
                    MovementTypes = new SelectList(movementTypes ?? new List<MovementTypesViewModel>(), "Id", "Name")
                };

                return View(getByArticleAndMovementTypeModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetByArticleAndMovementTypeIndex(int itemId, int movementTypeId, int page = 1, int pageSize = 10)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var request = new HttpRequestMessage(HttpMethod.Get, "StockMovement");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = $"StockMovement/GetByArticleAndMovementType/{itemId}/{movementTypeId}/page/{page}/pageSize/{pageSize}";
                HttpResponseMessage response = await _client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var stockMovements = JsonSerializer.Deserialize<List<ItemByMovementTypeModel>>(content, _jsonOptions);

                    return View(stockMovements);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["ErrorMessage"] = "Error al obtener el movimiento de stock. Verifique los datos.";
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    TempData["ErrorMessage"] = "Error al obtener el movimiento de stock. Intente más tarde.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al obtener el movimiento de stock.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }



        
        // Generar get para poder ingresar dos fechas y obtener los movimientos de stock en ese rango
        public ActionResult GetItemsByDateRange()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetItemsByDateRangeIndex(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 10)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var request = new HttpRequestMessage(HttpMethod.Get, "StockMovement");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = $"StockMovement/GetItemsByDateRange/{startDate.ToString("yyyy-MM-dd")}/{endDate.ToString("yyyy-MM-dd")}/page/{page}/pageSize/{pageSize}";
                HttpResponseMessage response = await _client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var stockMovements = JsonSerializer.Deserialize<List<ItemsViewModel>>(content, _jsonOptions);

                    return View(stockMovements);
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    TempData["ErrorMessage"] = "Error al obtener los movimientos de stock. Verifique los datos.";
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    TempData["ErrorMessage"] = "Error al obtener los movimientos de stock. Intente más tarde.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al obtener los movimientos de stock.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        // GET: StockMovementController/GetSummaryByYearAndMovementType
        public async Task<ActionResult> GetSummaryByYearAndMovementType()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var request = new HttpRequestMessage(HttpMethod.Get, "StockMovement/GetSummaryByYearAndMovementType");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.GetAsync("StockMovement/GetSummaryByYearAndMovementType");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var summary = JsonSerializer.Deserialize<List<SummaryViewModel>>(content, _jsonOptions);
                    return View(summary);
                }
                else
                {
                    ViewBag.ErrorMessage = "Error al obtener el resumen de movimientos de stock por año y tipo de movimiento.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }



        
    }
}
