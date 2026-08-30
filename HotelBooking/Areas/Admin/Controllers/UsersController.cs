using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;
using HotelBooking.Models.Entities;
using HotelBooking.Models.ViewModels;

namespace HotelBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(string? search, string? role)
        {
            var users = await _userManager.Users.ToListAsync();
            var userListVM = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "Customer";

                if (!string.IsNullOrEmpty(role) && userRole != role)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var s = search.ToLower();
                    if (!user.FullName.ToLower().Contains(s) && !user.Email!.ToLower().Contains(s))
                    {
                        continue;
                    }
                }

                userListVM.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Role = userRole,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    TotalBookingsCount = await _context.Bookings.CountAsync(b => b.UserId == user.Id)
                });
            }

            ViewData["CurrentSearch"] = search;
            ViewData["CurrentRole"] = role;

            return View(userListVM);
        }

        // POST: /Admin/Users/ToggleLock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = user.IsActive 
                ? $"Đã mở khóa tài khoản {user.Email}." 
                : $"Đã tạm khóa tài khoản {user.Email}.";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Users/ChangeRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string id, string newRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (await _roleManager.RoleExistsAsync(newRole))
            {
                await _userManager.AddToRoleAsync(user, newRole);
                TempData["SuccessMessage"] = $"Đã chuyển vai trò của {user.FullName} thành \"{newRole}\".";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
