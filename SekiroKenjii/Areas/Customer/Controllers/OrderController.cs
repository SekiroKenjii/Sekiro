﻿using System;
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

namespace SekiroKenjii.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                Orders = await _db.Orders.Include(o => o.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()
            };

            return View(orderDetailsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderDetailsViewModel> orderList = new List<OrderDetailsViewModel>();

            List<Order> OrderList = await _db.Orders.Include(o => o.ApplicationUser).Where(a => a.UserId == claim.Value).ToListAsync();
            foreach (Order item in OrderList)
            {
                OrderDetailsViewModel oDVM = new OrderDetailsViewModel
                {
                    Orders = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderList.Add(oDVM);
            }

            if (claim != null)
            {
                var cnt = 0;
                var lstCnt = _db.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList();
                foreach (var c in lstCnt)
                {
                    cnt += c.Count;
                }
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }

            return View(orderList);
        }

        public async Task<IActionResult> CancelOnWeb(int OrderId)
        {            
            Order order = await _db.Orders.FindAsync(OrderId);

            List<OrderDetails> orderDetails = await _db.OrderDetails.Where(o => o.OrderId == OrderId).ToListAsync();

            order.Status = SD.StatusCancelOnWeb;
            foreach(var pro in orderDetails)
            {
                var product = _db.Products.Where(p => p.Id == pro.ProductId).FirstOrDefault();
                product.UnitsInStock += pro.Count;
                product.UnitsOnOder -= pro.Count;
            }
            order.PaymentStatus = SD.PaymentStatusRejected;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(OrderHistory));
        }
    }
}
