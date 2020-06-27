using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SekiroKenjii.Data;
using SekiroKenjii.Models;
using SekiroKenjii.Models.ViewModel;
using SekiroKenjii.Utility;

namespace SekiroKenjii.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser + "," + SD.CustomerOfficer + "," + SD.PackingUser + "," + SD.Shipper)]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<OrderDetailsViewModel> orderList = new List<OrderDetailsViewModel>();

            if (User.IsInRole(SD.CustomerOfficer))
            {
                List<Order> OrderList = await _db.Orders.Include(o => o.ApplicationUser).Where(o => o.Status == SD.StatusSubmitted).ToListAsync();
                foreach (Order item in OrderList)
                {
                    OrderDetailsViewModel oDVM = new OrderDetailsViewModel
                    {
                        Orders = item,
                        OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                    };
                    orderList.Add(oDVM);
                }

                return View(orderList);
            }
            if (User.IsInRole(SD.PackingUser))
            {
                List<Order> OrderList = await _db.Orders.Include(o => o.ApplicationUser).Where(o => o.Status == SD.StatusApproved || o.Status == SD.StatusReadyPack || o.Status == SD.StatusInPackingProcess).ToListAsync();
                foreach (Order item in OrderList)
                {
                    OrderDetailsViewModel oDVM = new OrderDetailsViewModel
                    {
                        Orders = item,
                        OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                    };
                    orderList.Add(oDVM);
                }

                return View(orderList);
            }
            if (User.IsInRole(SD.Shipper))
            {
                List<Order> OrderList = await _db.Orders.Include(o => o.ApplicationUser).Where(o => o.Status == SD.StatusReadyShip || o.Status == SD.StatusShipping).ToListAsync();
                foreach (Order item in OrderList)
                {
                    OrderDetailsViewModel oDVM = new OrderDetailsViewModel
                    {
                        Orders = item,
                        OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                    };
                    orderList.Add(oDVM);
                }

                return View(orderList);
            }
            if (User.IsInRole(SD.ManagerUser))
            {
                List<Order> OrderList = await _db.Orders.Include(o => o.ApplicationUser).ToListAsync();
                foreach (Order item in OrderList)
                {
                    OrderDetailsViewModel oDVM = new OrderDetailsViewModel
                    {
                        Orders = item,
                        OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                    };
                    orderList.Add(oDVM);
                }

                return View(orderList);
            }

            return View();
        }

        public async Task<IActionResult> OrderApproved(int OrderId)
        {
            Order order = await _db.Orders.FindAsync(OrderId);
            order.Status = SD.StatusApproved;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> OrderReadyPack(int OrderId)
        {
            Order order = await _db.Orders.FindAsync(OrderId);
            order.Status = SD.StatusReadyPack;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderPack(int OrderId)
        {
            Order order = await _db.Orders.FindAsync(OrderId);
            order.Status = SD.StatusInPackingProcess;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> FinishPack(int OrderId)
        {
            Order order = await _db.Orders.FindAsync(OrderId);
            order.Status = SD.StatusReadyShip;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderShip(int OrderId)
        {
            Order order = await _db.Orders.FindAsync(OrderId);
            order.Status = SD.StatusShipping;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderComplete(int OrderId)
        {
            Order order = await _db.Orders.FindAsync(OrderId);
            order.Status = SD.StatusCompleted;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderFail(int OrderId)
        {
            Order order = await _db.Orders.FindAsync(OrderId);
            order.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
