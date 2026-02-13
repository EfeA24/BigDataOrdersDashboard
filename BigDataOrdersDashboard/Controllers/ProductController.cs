using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly BigDataDbContext _context;

        public ProductController(BigDataDbContext context)
        {
            _context = context;
        }

        public IActionResult ProductList(int page = 1)
        {
            //var products = _context.Products.ToList();
            //return View(products);

            int pageSize = 10; // Sayfa başına gösterilecek ürün sayısı
            var values = _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.ProductId)
                .Skip((page - 1) * pageSize)//sayfa numrasanından 1 eksik sayı kadar ürünü atlar ör: page=1 ise 0 ürün atlar, page=2 ise 10 ürün atlar
                .Take(pageSize) //sayfa başına gösterilecek ürün sayısı kadar ürünü alır
                .ToList();

            int totalCount = _context.Products.Count(); // Toplam ürün sayısını alır
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize); // Toplam sayfa sayısını hesaplar
            ViewBag.CurrentPage = page; // Mevcut sayfa numarasını ViewBag'e ekler

            return View(values);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            var categoryList = _context.Categories
                .Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.CategoryId.ToString()
                })
                .ToList();

            ViewBag.CategoryList = categoryList;
            return View(new Product());
        }


        [HttpPost]
        public IActionResult CreateProduct(Product Product)
        {
            _context.Products.Add(Product);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }

        public IActionResult DeleteProduct(int id)
        {
            var value = _context.Products.Find(id);
            _context.Products.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }

        public IActionResult UpdateProduct(int id)
        {
            var value = _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.ProductId == id);

            if (value == null) return NotFound();

            ViewBag.CategoryList = _context.Categories
                .Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.CategoryId.ToString(),
                    Selected = (x.CategoryId == value.CategoryId) // seçili kategori
                })
                .ToList();

            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product Product)
        {
            _context.Products.Update(Product);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }
    }
}
