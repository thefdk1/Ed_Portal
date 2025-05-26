using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Internet_Programciligi.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Internet_Programciligi.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { status = "success", message = "API çalışıyor", timestamp = DateTime.Now });
        }

        [HttpGet]
        public IActionResult Index()
        {
            var endpoints = new Dictionary<string, string>
            {
                // Kullanıcı endpoints
                {"POST /api/User/register", "Yeni kullanıcı kaydetme" },
                {"POST /api/User/login", "Kullanıcı girişi ve token alma" },
                {"GET /api/User/profile", "Kullanıcı profili görüntüleme (Auth gerekli)" },
                {"GET /api/User/users", "Tüm kullanıcıları listeleme (Admin için)" },

                // Kurs endpoints
                {"GET /api/Course", "Tüm kursları listeleme" },
                {"GET /api/Course/{id}", "Belirli bir kursu görüntüleme" },
                {"POST /api/Course", "Yeni kurs ekleme (Admin için)" },
                {"PUT /api/Course/{id}", "Kurs güncelleme (Admin için)" },
                {"DELETE /api/Course/{id}", "Kurs silme (Admin için)" },
                {"POST /api/Course/enroll/{courseId}", "Bir kursa kayıt olma (Auth gerekli)" },

                // Enrollment endpoints
                {"GET /api/Enrollment", "Tüm kayıtları listeleme (Admin için)" },
                {"GET /api/Enrollment/my-enrollments", "Kullanıcının kurs kayıtlarını listeleme (Auth gerekli)" },
                {"GET /api/Enrollment/{id}", "Belirli bir kaydı görüntüleme (Auth gerekli)" },
                {"PUT /api/Enrollment/{id}", "Kayıt güncelleme (Admin için)" },
                {"DELETE /api/Enrollment/{id}", "Kayıt silme (Auth/Admin için)" }
            };

            return Ok(new { 
                Application = "Eğitim Portalı API", 
                Description = "Bu API, kursları yönetmek ve kullanıcıların kurslara kayıt olmasını sağlamak için tasarlanmıştır.", 
                Endpoints = endpoints
            });
        }
    }
} 