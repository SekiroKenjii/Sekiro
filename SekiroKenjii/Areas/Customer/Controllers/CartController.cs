using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SekiroKenjii.Data;
using SekiroKenjii.Models;
using SekiroKenjii.Models.ViewModel;
using SekiroKenjii.Utility;
using Stripe;

namespace SekiroKenjii.Areas.Customer.Controllers
{
    
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsCart detailsCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            detailsCart = new OrderDetailsCart()
            {
                Orders = new Models.Order()
            };

            detailsCart.Orders.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCarts.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                detailsCart.lstCarts = cart.ToList();
            }

            foreach(var list in detailsCart.lstCarts)
            {
                list.Product = await _db.Products.FirstOrDefaultAsync(p => p.Id == list.ProductId);
                detailsCart.Orders.OrderTotal = detailsCart.Orders.OrderTotal + (list.Product.Price * list.Count);
            }
            if (detailsCart.Orders.OrderTotal.ToString().Length > 8)
            {
                detailsCart.Orders.OrderTotal = Math.Round(detailsCart.Orders.OrderTotal);
            }
            detailsCart.Orders.OrderTotalOriginal = detailsCart.Orders.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.Orders.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupons.Where(c => c.Name.ToLower() == detailsCart.Orders.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailsCart.Orders.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.Orders.OrderTotalOriginal);
            }

            return View(detailsCart);
        }

        public async Task<IActionResult> Summary()
        {
            detailsCart = new OrderDetailsCart()
            {
                Orders = new Models.Order()
            };

            detailsCart.Orders.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = await _db.ApplicationUsers.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();
            var cart = _db.ShoppingCarts.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                detailsCart.lstCarts = cart.ToList();
            }

            foreach (var list in detailsCart.lstCarts)
            {
                list.Product = await _db.Products.FirstOrDefaultAsync(p => p.Id == list.ProductId);
                detailsCart.Orders.OrderTotal = detailsCart.Orders.OrderTotal + (list.Product.Price * list.Count);
                
            }
            if (detailsCart.Orders.OrderTotal.ToString().Length > 8)
            {
                detailsCart.Orders.OrderTotal = Math.Round(detailsCart.Orders.OrderTotal);
            }
            detailsCart.Orders.OrderTotalOriginal = detailsCart.Orders.OrderTotal;
            detailsCart.Orders.OrderName = applicationUser.FullName;
            detailsCart.Orders.PhoneNumber = applicationUser.PhoneNumber;
            detailsCart.Orders.OrderDate = DateTime.Now;
            detailsCart.Orders.ShipAddress = applicationUser.Address;
            detailsCart.Orders.ShipCity = applicationUser.City;
            detailsCart.Orders.ShipCountry = applicationUser.Country;
            detailsCart.Orders.ShipEmail = applicationUser.Email;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.Orders.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupons.Where(c => c.Name.ToLower() == detailsCart.Orders.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailsCart.Orders.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.Orders.OrderTotalOriginal);
            }

            return View(detailsCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailsCart.lstCarts = await _db.ShoppingCarts.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            detailsCart.Orders.PaymentStatus = SD.PaymentStatusPending;
            detailsCart.Orders.OrderDate = DateTime.Now;
            detailsCart.Orders.UserId = claim.Value;
            detailsCart.Orders.Status = SD.PaymentStatusPending;

            List<OrderDetails> lstOrderDetails = new List<OrderDetails>();

            _db.Orders.Add(detailsCart.Orders);
            await _db.SaveChangesAsync();

            detailsCart.Orders.OrderTotal = 0;

            foreach (var item in detailsCart.lstCarts)
            {
                item.Product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                OrderDetails orderDetails = new OrderDetails
                {
                    ProductId = item.ProductId,
                    OrderId = detailsCart.Orders.Id,                   
                    Name = item.Product.SortName,
                    Price = item.Product.Price,
                    Count = item.Count
                };
                detailsCart.Orders.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);
            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.Orders.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupons.Where(c => c.Name.ToLower() == detailsCart.Orders.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailsCart.Orders.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.Orders.OrderTotalOriginal);
            }
            else
            {
                detailsCart.Orders.OrderTotal = detailsCart.Orders.OrderTotalOriginal;
            }
            detailsCart.Orders.CouponCodeDiscount = detailsCart.Orders.OrderTotalOriginal - detailsCart.Orders.OrderTotal;

            _db.ShoppingCarts.RemoveRange(detailsCart.lstCarts);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            await _db.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(detailsCart.Orders.OrderTotal * 100),
                Currency = "usd",
                Description = "Order ID : " + detailsCart.Orders.Id,
                Source = stripeToken

            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                detailsCart.Orders.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                detailsCart.Orders.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower() == "succeeded")
            {
                detailsCart.Orders.PaymentStatus = SD.PaymentStatusApproved;
                detailsCart.Orders.Status = SD.StatusSubmitted;
            }
            else
            {
                detailsCart.Orders.PaymentStatus = SD.PaymentStatusRejected;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
            //return RedirectToAction("Confirm", "Order", new { id=detailsCart.Orders.Id });
        }

        public IActionResult AddCoupon()
        {
            if (detailsCart.Orders.CouponCode == null)
            {
                detailsCart.Orders.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, detailsCart.Orders.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cart.Count == 1)
            {
                _db.ShoppingCarts.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(c => c.Id == cartId);

            _db.ShoppingCarts.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);


            return RedirectToAction(nameof(Index));
        }
    }
}
