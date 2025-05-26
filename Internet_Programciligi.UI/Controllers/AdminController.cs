using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Internet_Programciligi.UI.Services;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Internet_Programciligi.UI.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiService _apiService;

        public AdminController(
            ILogger<AdminController> logger, 
            IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor,
            ApiService apiService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _apiService = apiService;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            // Admin kontrolü
            if (!IsAdmin())
            {
                return RedirectToAction("Courses", "Home");
            }

            SetCommonViewBagProperties();
            return View("Home");
        }

        [Route("Home")]
        public async Task<IActionResult> Home()
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            // Admin kontrolü
            if (!IsAdmin())
            {
                return RedirectToAction("Courses", "Home");
            }

            SetCommonViewBagProperties();
            ViewBag.Token = token;

            try
            {
                // Dashboard verilerini API'den çekelim
                var userCountTask = _apiService.GetAsync<object>(ViewBag.ApiBaseURL + "/User/users", true);
                var courseCountTask = _apiService.GetAsync<object>(ViewBag.ApiBaseURL + "/Course", true);
                var categoryCountTask = _apiService.GetAsync<object>(ViewBag.ApiBaseURL + "/Category", true);

                await Task.WhenAll(userCountTask, courseCountTask, categoryCountTask);

                var users = await userCountTask;
                var courses = await courseCountTask;
                var categories = await categoryCountTask;

                // JsonElement'e dönüştürerek eleman sayısını alalım
                var usersArray = JsonDocument.Parse(JsonSerializer.Serialize(users)).RootElement;
                var coursesArray = JsonDocument.Parse(JsonSerializer.Serialize(courses)).RootElement;
                var categoriesArray = JsonDocument.Parse(JsonSerializer.Serialize(categories)).RootElement;

                ViewBag.UserCount = usersArray.ValueKind == JsonValueKind.Array ? usersArray.GetArrayLength() : 0;
                ViewBag.CourseCount = coursesArray.ValueKind == JsonValueKind.Array ? coursesArray.GetArrayLength() : 0;
                ViewBag.CategoryCount = categoriesArray.ValueKind == JsonValueKind.Array ? categoriesArray.GetArrayLength() : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API verilerini alırken hata oluştu");
                ViewBag.UserCount = 0;
                ViewBag.CourseCount = 0;
                ViewBag.CategoryCount = 0;
            }
            
            return View();
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            return RedirectToAction("Login", "Home");
        }

        [Route("Courses")]
        public async Task<IActionResult> Courses()
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            // Admin kontrolü
            if (!IsAdmin())
            {
                return RedirectToAction("Courses", "Home");
            }

            SetCommonViewBagProperties();
            ViewBag.Token = token;

            return View();
        }

        [Route("Categories")]
        public async Task<IActionResult> Categories()
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            // Admin kontrolü
            if (!IsAdmin())
            {
                return RedirectToAction("Courses", "Home");
            }

            SetCommonViewBagProperties();
            ViewBag.Token = token;

            return View();
        }

        [Route("Users")]
        public async Task<IActionResult> Users()
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            // Admin kontrolü
            if (!IsAdmin())
            {
                return RedirectToAction("Courses", "Home");
            }

            SetCommonViewBagProperties();
            ViewBag.Token = token;

            return View();
        }

        [Route("Lessons")]
        public async Task<IActionResult> Lessons()
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            // Admin kontrolü
            if (!IsAdmin())
            {
                return RedirectToAction("Courses", "Home");
            }

            SetCommonViewBagProperties();
            ViewBag.Token = token;

            return View();
        }
        
        [Route("TeacherProfiles")]
        public async Task<IActionResult> TeacherProfiles()
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            // Admin kontrolü
            if (!IsAdmin())
            {
                return RedirectToAction("Courses", "Home");
            }

            SetCommonViewBagProperties();
            ViewBag.Token = token;

            return View();
        }

        private bool IsAdmin()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
                return false;
                
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            
            if (jsonToken == null) 
                return false;
                
            var roleClaims = jsonToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            return roleClaims.Any(c => c.Value == "Admin");
        }
        
        private void SetCommonViewBagProperties()
        {
            ViewBag.IsAdmin = IsAdmin();
            
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                
                if (jsonToken != null)
                {
                    var userNameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    if (userNameClaim != null)
                    {
                        ViewBag.UserName = userNameClaim.Value;
                    }
                }
            }
            
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
        }
    }
} 