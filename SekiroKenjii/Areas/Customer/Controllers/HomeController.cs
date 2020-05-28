using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SekiroKenjii.Models;
using SekiroKenjii.Data;
using SekiroKenjii.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace SekiroKenjii.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(string SearchString = null)
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).ToListAsync(),
                Categories = await _db.Categories.ToListAsync(),
                Suppliers = await _db.Suppliers.ToListAsync(),
                Tags = await _db.Tags.ToListAsync(),
                Coupons = await _db.Coupons.Where(c=>c.IsActive==true).ToListAsync()
            };
            StringBuilder param = new StringBuilder();
            param.Append("&Search");
            if(SearchString != null)
            {
                param.Append(SearchString);
            }
            IndexVM.Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).ToListAsync();
            if (SearchString != null)
            {
                foreach (var item in IndexVM.Products)
                {
                    if (item.SortName.ToLower().Contains(SearchString.ToLower()))
                    {
                        IndexVM.Products = IndexVM.Products.Where(p => p.Name.ToLower().Contains(SearchString.ToLower())).ToList();
                    }
                    if (item.Category.Name.ToLower().Contains(SearchString.ToLower()))
                    {
                        IndexVM.Products = IndexVM.Products.Where(p => p.Category.Name.ToLower().Contains(SearchString.ToLower())).ToList();
                    }
                    if (item.Supplier.Name.ToLower().Contains(SearchString.ToLower()))
                    {
                        IndexVM.Products = IndexVM.Products.Where(p => p.Supplier.Name.ToLower().Contains(SearchString.ToLower())).ToList();
                    }
                }
            }            
            return View(IndexVM);
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
