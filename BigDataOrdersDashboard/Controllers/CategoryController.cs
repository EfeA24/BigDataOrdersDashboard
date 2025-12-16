using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BigDataDbContext _context;

        public CategoryController(BigDataDbContext context)
        {
            _context = context;
        }

        public IActionResult CategoryList()
        {
            return View();
        }
    }
}
