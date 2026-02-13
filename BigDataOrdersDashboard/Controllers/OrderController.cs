using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Controllers
{
    public class OrderController : Controller
    {
        private readonly BigDataDbContext _context;
        public OrderController(BigDataDbContext context)
        {
            _context = context;
        }
        public IActionResult OrderList(int page = 1)
        {
            int pageSize = 10; // her sayfada 12 kayıt
            var values = _context.Orders
                                 .OrderBy(p => p.OrderId)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .Include(x => x.Product)
                                 .Include(y => y.Customer)
                                 .ToList();

            int totalCount = _context.Orders.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(values);
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            ViewBag.ProductList = _context.Products
                .Select(x => new SelectListItem
                {
                    Text = x.ProductName,
                    Value = x.ProductId.ToString()
                })
                .ToList();

            ViewBag.CustomerList = _context.Customers
                .Select(x => new SelectListItem
                {
                    Text = x.CustomerName + " " + x.CustomerSurname,
                    Value = x.CustomerId.ToString()
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            order.OrderDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("OrderList");
        }

        public IActionResult DeleteOrder(int id)
        {
            var value = _context.Orders.Find(id);
            _context.Orders.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("OrderList");
        }

        [HttpGet]
        public IActionResult UpdateOrder(int id)
        {
            var value = _context.Orders.Find(id);
            if (value == null)
            {
                return NotFound();
            }

            ViewBag.ProductList = _context.Products
                .Select(x => new SelectListItem
                {
                    Text = x.ProductName,
                    Value = x.ProductId.ToString(),
                    Selected = x.ProductId == value.ProductId
                })
                .ToList();

            ViewBag.CustomerList = _context.Customers
                .Select(x => new SelectListItem
                {
                    Text = x.CustomerName + " " + x.CustomerSurname,
                    Value = x.CustomerId.ToString(),
                    Selected = x.CustomerId == value.CustomerId
                })
                .ToList();

            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("OrderList");
        }
    }
}
