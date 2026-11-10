using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK8MVC.Data;
using TASK8MVC.Models;

namespace TASK8MVC.Controllers
{
    [Authorize]
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SupplyRequestController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var requests = _context.SupplyRequests
                .Include(r => r.RequestItems).ThenInclude(i => i.Product);

            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                return View(requests.ToList());
            }

            return View(requests.Where(r => r.UserId == user.Id).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<int> productIds, List<int> quantities)
        {
            if (!ModelState.IsValid || productIds == null || quantities == null || productIds.Count == 0 || productIds.Count != quantities.Count)
            {
                ModelState.AddModelError("", "Select at least one product and enter its quantity.");
                ViewBag.Products = _context.Products.ToList();
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var request = new SupplyRequest
            {
                RequestDate = DateTime.Now,
                Status = "Pending",
                UserId = user.Id,
                TotalPrice = 0,
                RequestItems = new List<RequestItem>()
            };

            for (int i = 0; i < productIds.Count; i++)
            {
                var product = _context.Products.Find(productIds[i]);
                if (product == null || quantities[i] <= 0 || quantities[i] > product.Quantity || productIds.Distinct().Count() != productIds.Count)
                {
                    ModelState.AddModelError("", "Choose each product once, with a positive quantity within the available stock.");
                    ViewBag.Products = _context.Products.ToList();
                    return View();
                }

                request.RequestItems.Add(new RequestItem
                {
                    ProductId = product.Id,
                    Quantity = quantities[i],
                    Price = product.Price
                });
                request.TotalPrice += product.Price * quantities[i];
            }

            _context.SupplyRequests.Add(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = _context.SupplyRequests.Include(r => r.RequestItems).ThenInclude(i => i.Product)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && request.UserId != user.Id)
                return Forbid();

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = _context.SupplyRequests.Include(r => r.RequestItems)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && request.UserId != user.Id)
                return Forbid();

            if (request.Status != "Pending") return Forbid();
            ViewBag.Products = _context.Products.ToList();
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<int> productIds, List<int> quantities)
        {
            var request = _context.SupplyRequests.Include(r => r.RequestItems)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && request.UserId != user.Id)
                return Forbid();

            if (!ModelState.IsValid || productIds == null || quantities == null || productIds.Count == 0 || productIds.Count != quantities.Count)
            {
                ModelState.AddModelError("", "Select at least one product and enter its quantity.");
                ViewBag.Products = _context.Products.ToList();
                return View(request);
            }

            if (request.Status != "Pending") return Forbid();

            var updatedItems = new List<RequestItem>();
            double totalPrice = 0;
            for (int i = 0; i < productIds.Count; i++)
            {
                var product = _context.Products.Find(productIds[i]);
                if (product == null || quantities[i] <= 0 || quantities[i] > product.Quantity || productIds.Distinct().Count() != productIds.Count)
                {
                    ModelState.AddModelError("", "Choose each product once, with a positive quantity within the available stock.");
                    ViewBag.Products = _context.Products.ToList();
                    return View(request);
                }

                updatedItems.Add(new RequestItem
                {
                    SupplyRequestId = request.Id,
                    ProductId = product.Id,
                    Quantity = quantities[i],
                    Price = product.Price
                });
                totalPrice += product.Price * quantities[i];
            }

            _context.RequestItems.RemoveRange(request.RequestItems);
            request.RequestItems = updatedItems;
            request.TotalPrice = totalPrice;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var request = _context.SupplyRequests.Include(r => r.RequestItems).ThenInclude(i => i.Product)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && request.UserId != user.Id)
                return Forbid();

            if (request.Status != "Pending") return Forbid();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = _context.SupplyRequests.Include(r => r.RequestItems)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin") && request.UserId != user.Id)
                return Forbid();

            if (request.Status != "Pending") return Forbid();
            _context.RequestItems.RemoveRange(request.RequestItems);
            _context.SupplyRequests.Remove(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id, string status)
        {
            if (status != "Approved" && status != "Rejected") return BadRequest();
            var request = _context.SupplyRequests.Find(id);
            if (request == null) return NotFound();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be reviewed.");
            request.Status = status;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = request.Id });
        }
    }
}