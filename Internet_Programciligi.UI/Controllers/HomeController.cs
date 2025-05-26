using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Internet_Programciligi.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Services.ApiService _apiService;

        public HomeController(
            ILogger<HomeController> logger, 
            IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor,
            Services.ApiService apiService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            SetCommonViewBagProperties();
            return View();
        }

        [Route("Login")]
        public IActionResult Login()
        {
            // Eğer zaten giriş yapmışsa anasayfaya yönlendir
            if (HttpContext.Session.GetString("JWTToken") != null)
            {
                return RedirectToAction("Index");
            }
            
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }

        [Route("Register")]
        public IActionResult Register()
        {
            // Eğer zaten giriş yapmışsa anasayfaya yönlendir
            if (HttpContext.Session.GetString("JWTToken") != null)
            {
                return RedirectToAction("Index");
            }
            
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }
        
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWTToken");
            return RedirectToAction("Index");
        }
        
        [Route("Courses")]
        public IActionResult Courses()
        {
            SetCommonViewBagProperties();
            var ApiBaseURL = _configuration["ApiBaseURL"];
            ViewBag.ApiBaseURL = ApiBaseURL;
            return View();
        }
        
        [Route("Teachers")]
        public IActionResult Teachers()
        {
            SetCommonViewBagProperties();
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