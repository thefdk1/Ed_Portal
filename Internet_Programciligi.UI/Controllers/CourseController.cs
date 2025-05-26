using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Internet_Programciligi.UI.Services;

namespace Internet_Programciligi.UI.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApiService _apiService;

        public CourseController(
            ILogger<CourseController> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ApiService apiService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _apiService = apiService;
        }

        [Route("Course/Details/{id}")]
        public IActionResult Details(int id)
        {
            SetCommonViewBagProperties();
            ViewBag.CourseId = id;
            
            // Kullanıcı giriş yapmışsa token'ı da gönder
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                ViewBag.Token = token;
            }

            return View();
        }

        [Route("Course/Enroll/{id}")]
        public IActionResult Enroll(int id)
        {
            // JWT token kontrolü
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Home");
            }

            SetCommonViewBagProperties();
            ViewBag.CourseId = id;
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