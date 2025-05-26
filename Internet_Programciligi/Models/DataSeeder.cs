using Microsoft.AspNetCore.Identity;

namespace Internet_Programciligi.Models
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
        {
            // Rolleri kontrol et ve oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new AppRole { Name = "Admin" });
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new AppRole { Name = "User" });
            }
        }

        public static async Task SeedAdminUserAsync(UserManager<AppUser> userManager)
        {
            // Admin kullanıcısını kontrol et ve oluştur
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else 
            {
                // Eğer admin kullanıcısı varsa, email adresini güncelle ve şifreyi sıfırla
                if (adminUser.Email != "admin@example.com")
                {
                    adminUser.Email = "admin@example.com";
                    await userManager.UpdateAsync(adminUser);
                }

                // Admin rolünü kontrol et
                var isInRole = await userManager.IsInRoleAsync(adminUser, "Admin");
                if (!isInRole)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Şifreyi sıfırla (gerektiğinde)
                var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
                await userManager.ResetPasswordAsync(adminUser, token, "Admin123!");
            }
        }
    }
} 