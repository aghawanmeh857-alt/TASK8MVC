using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TASK8MVC.Data;

namespace TASK8MVC.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                userRoles[user.Id] = string.Join(", ", await _userManager.GetRolesAsync(user));
            }
            ViewBag.Users = users;
            ViewBag.UserRoles = userRoles;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.CurrentRoles = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string roleId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var role = await _roleManager.FindByIdAsync(roleId ?? "");
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (role == null || (role.Name != "User" && role.Name != "Admin" && role.Name != "SuperAdmin"))
                ModelState.AddModelError("", "Select a valid role.");
            if (user.Id == _userManager.GetUserId(User) && role?.Name != "SuperAdmin")
                ModelState.AddModelError("", "You cannot remove your own SuperAdmin role.");
            if (ModelState.IsValid && role != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    var result = removeResult;
                    if (removeResult.Succeeded)
                        result = await _userManager.AddToRoleAsync(user, role.Name!);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        transaction.Commit();
                        return RedirectToAction("Index");
                    }
                    foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.CurrentRoles = currentRoles;
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.Id == _userManager.GetUserId(User))
            {
                ModelState.AddModelError("", "You cannot delete your own account.");
                return View(user);
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded) return RedirectToAction("Index");
            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return View(user);
        }
    }
}