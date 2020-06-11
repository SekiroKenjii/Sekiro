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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SekiroKenjii.Utility;

namespace SekiroKenjii.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        //private int PageSize = 12;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).ToListAsync(),
                Categories = await _db.Categories.ToListAsync(),
                Suppliers = await _db.Suppliers.ToListAsync(),
                Tags = await _db.Tags.ToListAsync(),
                Coupons = await _db.Coupons.Where(c=>c.IsActive==true).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }

            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var productFromDB = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Tag).Where(p => p.Id == id).FirstOrDefaultAsync();
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDB,
                ProductId = productFromDB.Id
            };

            return View(cartObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCarts.Where(c => c.ApplicationUserId == CartObject.ApplicationUserId && 
                                                                        c.ProductId == CartObject.ProductId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.ShoppingCarts.AddAsync(CartObject);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + CartObject.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCarts.Where(c => c.ApplicationUserId == CartObject.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {
                var productFromDB = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Tag).Where(p => p.Id == CartObject.ProductId).FirstOrDefaultAsync();
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDB,
                    ProductId = productFromDB.Id
                };

                return View(cartObj);
            }
        }

        public async Task<IActionResult> Shop(string SearchString = null)
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).ToListAsync(),
                Categories = await _db.Categories.ToListAsync(),
                Suppliers = await _db.Suppliers.ToListAsync(),
                Tags = await _db.Tags.ToListAsync(),
                Coupons = await _db.Coupons.Where(c => c.IsActive == true).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }

            StringBuilder param = new StringBuilder();
            if (SearchString != null)
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
    }
}
