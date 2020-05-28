using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SekiroKenjii.Data;
using SekiroKenjii.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using SekiroKenjii.Models.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.IO;
using SekiroKenjii.Utility;

namespace SekiroKenjii.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public SupplierViewModel SupplierVM { get; set; }

        public SupplierController(ApplicationDbContext db, IHostingEnvironment hostEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostEnvironment;
            SupplierVM = new SupplierViewModel()
            {
                Suppliers = new Models.Supplier()
            };    
        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.Suppliers.OrderBy(s=>s.Name).ToListAsync());
        }

        //GET Create
        public IActionResult Create()
        {
            return View(SupplierVM);
        }
        //POST Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(SupplierVM);
            }
            _db.Suppliers.Add(SupplierVM.Suppliers);
            await _db.SaveChangesAsync();

            //Image being saved

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var supplierFromDb = await _db.Suppliers.FindAsync(SupplierVM.Suppliers.Id);

            if (files.Count != 0)
            {
                //Image has been uploaded
                var upload = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(upload, SupplierVM.Suppliers.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                supplierFromDb.BrandImage = @"\" + SD.ImageFolder + @"\" + SupplierVM.Suppliers.Id + extension;
            }
            else
            {
                //When user doesnt upload image
                var upload = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultSupplierImage);
                System.IO.File.Copy(upload, webRootPath + @"\" + SD.ImageFolder + @"\" + SupplierVM.Suppliers.Id + ".jpg");
                supplierFromDb.BrandImage = @"\" + SD.ImageFolder + @"\" + SupplierVM.Suppliers.Id + ".jpg";
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SupplierVM.Suppliers = await _db.Suppliers.SingleOrDefaultAsync(s=>s.Id==id);
            if (SupplierVM.Suppliers == null)
            {
                return NotFound();
            }
            return View(SupplierVM);
        }
        //POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var supplierFromDB = _db.Suppliers.Where(p => p.Id == SupplierVM.Suppliers.Id).FirstOrDefault();

                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var upload = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(supplierFromDB.BrandImage);

                    if (System.IO.File.Exists(Path.Combine(upload, SupplierVM.Suppliers.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(upload, SupplierVM.Suppliers.Id + extension_old));
                    }
                    using (var filestream = new FileStream(Path.Combine(upload, SupplierVM.Suppliers.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    SupplierVM.Suppliers.BrandImage = @"\" + SD.ImageFolder + @"\" + SupplierVM.Suppliers.Id + extension_new;
                }
                if (SupplierVM.Suppliers.BrandImage != null)
                {
                    supplierFromDB.BrandImage = SupplierVM.Suppliers.BrandImage;
                }

                supplierFromDB.Name = SupplierVM.Suppliers.Name;
                supplierFromDB.Address = SupplierVM.Suppliers.Address;
                supplierFromDB.City = SupplierVM.Suppliers.City;
                supplierFromDB.Country = SupplierVM.Suppliers.Country;
                supplierFromDB.Region = SupplierVM.Suppliers.Region;
                supplierFromDB.Phone = SupplierVM.Suppliers.Phone;
                supplierFromDB.HomePage = SupplierVM.Suppliers.Phone;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(SupplierVM);
        }

        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SupplierVM.Suppliers = await _db.Suppliers.SingleOrDefaultAsync(s=>s.Id == id);
            if (SupplierVM.Suppliers == null)
            {
                return NotFound();
            }
            return View(SupplierVM);
        }
        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Supplier supplier = await _db.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }
            else
            {
                var upload = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(supplier.BrandImage);

                if (System.IO.File.Exists(Path.Combine(upload, supplier.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(upload, supplier.Id + extension));
                }
                _db.Suppliers.Remove(supplier);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }

        //GET Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SupplierVM.Suppliers = await _db.Suppliers.SingleOrDefaultAsync(s=>s.Id == id);
            if (SupplierVM.Suppliers == null)
            {
                return NotFound();
            }
            return View(SupplierVM);
        }
    }
}