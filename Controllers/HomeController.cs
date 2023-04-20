using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Packt.Shared;
using System.Diagnostics;

namespace Northwind.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext db;

        public HomeController(ILogger<HomeController> logger,
            NorthwindContext context)
        {
            _logger = logger;
            db = context;
        }

        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("I am in the Index method of the HomeController.");

            HomeIndexViewModel model = new
            (
                VisitorCount: (new Random()).Next(1, 1001),
                Categories: await db.Categories.ToListAsync(),
                Products: await db.Products.ToListAsync()
            );

            return View(model);
        }

        [Route("private")]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ModelBinding()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            HomeModelBindingViewModel model = new(
                thing,
                !ModelState.IsValid,
                ModelState.Values
                          .SelectMany(state => state.Errors)
                          .Select(error => error.ErrorMessage));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetail (int? id)
        {
            if(!id.HasValue)
            {
                return BadRequest("You must pass a product ID in the path parameter");
            }

            Product? model = await db.Products.SingleOrDefaultAsync(p => p.ProductId == id);

            if(model == null)
            {
                return NotFound($"ProductId {id} not found.");
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}