using AutoMapper;
using Internet_Programciligi.DTOs;
using Internet_Programciligi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace Internet_Programciligi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                _logger.LogInformation($"Kayıt isteği: {registerDTO.UserName}, {registerDTO.Email}");
                
                if (ModelState.IsValid)
                {
                    // Kullanıcı adı veya email kontrolü
                    var existingUser = await _userManager.FindByNameAsync(registerDTO.UserName);
                    if (existingUser != null)
                    {
                        _logger.LogWarning($"Kullanıcı adı {registerDTO.UserName} zaten kullanılıyor.");
                        return BadRequest(new { message = "Bu kullanıcı adı zaten kullanılıyor." });
                    }

                    existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
                    if (existingUser != null)
                    {
                        _logger.LogWarning($"Email {registerDTO.Email} zaten kullanılıyor.");
                        return BadRequest(new { message = "Bu email adresi zaten kullanılıyor." });
                    }

                    var user = _mapper.Map<AppUser>(registerDTO);
                    var result = await _userManager.CreateAsync(user, registerDTO.Password);

                    if (result.Succeeded)
                    {
                        // Normal kullanıcı rolü atama
                        await _userManager.AddToRoleAsync(user, "User");
                        _logger.LogInformation($"Kullanıcı başarıyla oluşturuldu: {user.UserName}");
                        return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
                    }
                    
                    _logger.LogWarning($"Kullanıcı oluşturma hatası: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });
                }
                
                _logger.LogWarning($"Geçersiz model: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                return BadRequest(new { message = "Geçersiz kayıt bilgileri. Lütfen tüm alanları doldurun." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kayıt işlemi sırasında hata oluştu");
                return StatusCode(500, new { message = "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin." });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                _logger.LogInformation($"Giriş isteği: {loginDTO.Email}");
                
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(loginDTO.Email);
                    if (user == null)
                    {
                        _logger.LogWarning($"Kullanıcı bulunamadı: {loginDTO.Email}");
                        return BadRequest(new { message = "Hatalı e-posta veya şifre." });
                    }

                    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                    if (result.Succeeded)
                    {
                        // JWT token oluşturma
                        var tokenDTO = await GenerateToken(user);
                        _logger.LogInformation($"Kullanıcı giriş yaptı: {user.Email}");
                        return Ok(tokenDTO);
                    }
                    
                    _logger.LogWarning($"Geçersiz şifre: {loginDTO.Email}");
                    return BadRequest(new { message = "Hatalı e-posta veya şifre." });
                }
                
                _logger.LogWarning($"Geçersiz model: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                return BadRequest(new { message = "Geçersiz giriş bilgileri." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş işlemi sırasında hata oluştu");
                return StatusCode(500, new { message = "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                userDTOs.Add(userDTO);
            }

            return Ok(userDTOs);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            
            return Ok(userDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] string newRole)
        {
            try
            {
                _logger.LogInformation($"Rol değiştirme isteği - UserId: {userId}, NewRole: {newRole}");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"Kullanıcı bulunamadı - UserId: {userId}");
                    return NotFound(new { message = "Kullanıcı bulunamadı." });
                }

                // Rolün varlığını kontrol et
                var roleExists = await _roleManager.RoleExistsAsync(newRole);
                if (!roleExists)
                {
                    _logger.LogWarning($"Rol bulunamadı - Role: {newRole}");
                    return BadRequest(new { message = $"'{newRole}' rolü sistemde bulunamadı." });
                }

                // Mevcut rolleri temizle
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        _logger.LogError($"Mevcut roller silinemedi - Errors: {string.Join(", ", removeResult.Errors)}");
                        return BadRequest(new { message = "Mevcut roller kaldırılırken bir hata oluştu." });
                    }
                }

                // Yeni rolü ata
                var addResult = await _userManager.AddToRoleAsync(user, newRole);
                if (addResult.Succeeded)
                {
                    _logger.LogInformation($"Rol başarıyla değiştirildi - UserId: {userId}, NewRole: {newRole}");
                    return Ok(new { message = "Kullanıcı rolü başarıyla güncellendi." });
                }

                _logger.LogError($"Rol atanamadı - Errors: {string.Join(", ", addResult.Errors)}");
                return BadRequest(new { message = "Rol güncellenirken bir hata oluştu: " + string.Join(", ", addResult.Errors.Select(e => e.Description)) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rol değiştirme işlemi sırasında hata oluştu");
                return StatusCode(500, new { message = "Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Kullanıcı bulunamadı." });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "Kullanıcı başarıyla silindi." });
            }

            return BadRequest(new { message = "Kullanıcı silinirken bir hata oluştu." });
        }

        // JWT Token oluşturma methodu
        private async Task<TokenDTO> GenerateToken(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiration"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }
    }
} 