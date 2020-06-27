using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SekiroKenjii.Data;
using SekiroKenjii.Models;
using SekiroKenjii.Models.ViewModel;
using SekiroKenjii.Utility;

namespace SekiroKenjii.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser + "," + SD.CustomerOfficer + "," + SD.PackingUser + "," + SD.Shipper)]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            OrderViewModel OrderVM = new OrderViewModel();

            List<Order> OrderList = await _db.Orders.Include(o => o.ApplicationUser).ToListAsync();

            var user = await _userManager.Users.ToListAsync();
            OrderVM.Users = user;

            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach(var data in OrderList)
            {
                var orderName = "";
                var orderTotal = 0.0;
                foreach(var lst in OrderVM.Users)
                {
                    if (data.UserId == lst.Id)
                    {
                        orderName = data.OrderName;
                        orderTotal = data.OrderTotal;
                        dataPoints.Add(new DataPoint(orderName, orderTotal));
                    }
                }
            }
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            OrderVM.Orders = OrderList;
            OrderVM.DataPoints = dataPoints;


            return View(OrderVM);
        }
    }
}
