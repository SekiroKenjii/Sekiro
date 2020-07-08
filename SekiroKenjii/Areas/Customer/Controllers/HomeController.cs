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
using Microsoft.Extensions.Localization;

namespace SekiroKenjii.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

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
                var cnt = 0;
                var lstCnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList();
                foreach(var c in lstCnt)
                {
                    cnt += c.Count;
                }
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

            ShoppingCartVM = new ShoppingCartViewModel()
            {
                ShoppingCart = new Models.ShoppingCart(),
                Products = new Models.Product()
            };
            ShoppingCartVM.ShoppingCart = cartObj;
            ShoppingCartVM.Products = productFromDB;

            return View(ShoppingCartVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> Details()
        {
            ShoppingCartVM.ShoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCartVM.ShoppingCart.ApplicationUserId = claim.Value;

                var UnitStock = await _db.Products.Where(p => p.Id == ShoppingCartVM.ShoppingCart.ProductId).FirstOrDefaultAsync();

                ShoppingCart cartFromDb = await _db.ShoppingCarts.Where(c => c.ApplicationUserId == ShoppingCartVM.ShoppingCart.ApplicationUserId && 
                                                                        c.ProductId == ShoppingCartVM.ShoppingCart.ProductId).FirstOrDefaultAsync();

                if (cartFromDb == null)
                {
                    await _db.ShoppingCarts.AddAsync(ShoppingCartVM.ShoppingCart);
                }
                else
                {
                    cartFromDb.Count += ShoppingCartVM.ShoppingCart.Count;
                }

                UnitStock.UnitsInStock -= ShoppingCartVM.ShoppingCart.Count; 

                await _db.SaveChangesAsync();

                var count = 0;
                var lstCount = _db.ShoppingCarts.Where(c => c.ApplicationUserId == ShoppingCartVM.ShoppingCart.ApplicationUserId).ToList();
                foreach(var c in lstCount)
                {
                    count += c.Count;
                }
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
                
                return Redirect(Request.Headers["Referer"].ToString());

            }
            else
            {
                var productFromDB = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Tag).Where(p => p.Id == ShoppingCartVM.ShoppingCart.ProductId).FirstOrDefaultAsync();
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
                var cnt = 0;
                var lstCnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList();
                foreach(var c in lstCnt)
                {
                    cnt += c.Count;
                }
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }

            StringBuilder param = new StringBuilder();
            if (SearchString != null)
            {
                param.Append(SearchString);
            }

            IndexVM.Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).OrderBy(p=>p.Category.Name).ThenBy(p=>p.Supplier.Name).ToListAsync();

            if (SearchString != null)
            {
                foreach (var item in IndexVM.Products)
                {
                    string stringName = item.Category.Name.ToLower() + " " + item.SortName.ToLower();
                    if(stringName.Contains(SearchString.ToLower()))
                    {
                        IndexVM.Products = IndexVM.Products.Where(p => (p.Category.Name.ToLower() + " " + p.SortName.ToLower()).Contains(SearchString.ToLower())).ToList();
                    }
                    //if (SearchString.Replace(" ", string.Empty).ToLower().Contains(item.SortName.Replace(" ", string.Empty).ToLower()))
                    //{
                    //    IndexVM.Products = IndexVM.Products.Where(p => p.SortName.Replace(" ", string.Empty).ToLower().Contains(SearchString.Replace(" ", string.Empty).ToLower())).ToList();
                    //}
                    //if (item.Category.Name.Replace(" ", string.Empty).ToLower().Contains(SearchString.Replace(" ", string.Empty).ToLower()))
                    //{
                    //    IndexVM.Products = IndexVM.Products.Where(p => p.Category.Name.Replace(" ", string.Empty).ToLower().Contains(SearchString.Replace(" ", string.Empty).ToLower())).ToList();
                    //}
                    //if (item.Supplier.Name.Replace(" ", string.Empty).ToLower().Contains(SearchString.Replace(" ", string.Empty).ToLower()))
                    //{
                    //    IndexVM.Products = IndexVM.Products.Where(p => p.Supplier.Name.Replace(" ", string.Empty).ToLower().Contains(SearchString.Replace(" ", string.Empty).ToLower())).ToList();
                    //}
                }
            }

            return View(IndexVM);
        }
    }
}
