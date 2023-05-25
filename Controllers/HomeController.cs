using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OzSapkaTShirt.Data;
using OzSapkaTShirt.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace OzSapkaTShirt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _context;
        Order? order;
        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string? id=null)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                order = _context.Orders.Where(o => o.UserId == userId && o.Status == 0)
                .Include(o => o.OrderProducts).FirstOrDefault();
                if (order != null)
                    ViewData["TotalQuantity"] = order.OrderProducts.Sum(o => o.Quantity);
                else
                    ViewData["TotalQuantity"] = null;
            }
            else
                ViewData["TotalQuantity"] = null;

            return View(_context.Products.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}