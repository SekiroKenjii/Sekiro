﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SekiroKenjii.Data;
using SekiroKenjii.Models;
using SekiroKenjii.Models.ViewModel;
using SekiroKenjii.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace SekiroKenjii.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductViewModel ProductsVM { get; set; }

        public ProductController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductsVM = new ProductViewModel()
            {
                Products = new Models.Product(),
                Categories = _db.Categories,
                Suppliers = _db.Suppliers,
                Tags = _db.Tags
            };
        }
        
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p=>p.Tag).OrderBy(p=>p.Category.Name).ThenBy(p=>p.SortName).ToListAsync();
            return View(products);
        }

        //GET Create
        public IActionResult Create()
        {
            return View(ProductsVM);
        }
        //POST Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }
            _db.Products.Add(ProductsVM.Products);
            await _db.SaveChangesAsync();

            //Image being saved

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productsFromDb = await _db.Products.FindAsync(ProductsVM.Products.Id);

            if (files.Count != 0)
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ProductImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.Image = @"\" + SD.ProductImageFolder + @"\" + ProductsVM.Products.Id + extension;
            }
            else
            {
                //when user doesnt upload image
                var uploads = Path.Combine(webRootPath, SD.ProductImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ProductImageFolder + @"\" + ProductsVM.Products.Id + ".jpg");
                productsFromDb.Image = @"\" + SD.ProductImageFolder + @"\" + ProductsVM.Products.Id + ".jpg";
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
            ProductsVM.Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Tag).SingleOrDefaultAsync(p => p.Id == id);
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }
        //POST Create
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }
            
            //Image being saved

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productsFromDb = await _db.Products.FindAsync(ProductsVM.Products.Id);

            if (files.Count > 0)
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ProductImageFolder);
                var extension_new = Path.GetExtension(files[0].FileName);

                var imagePath = Path.Combine(webRootPath, productsFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                //we will upload the new file
                 
                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.Image = @"\" + SD.ProductImageFolder + @"\" + ProductsVM.Products.Id + extension_new;
            }

            productsFromDb.Name = ProductsVM.Products.Name;
            productsFromDb.SortName = ProductsVM.Products.SortName;
            productsFromDb.CategoryId = ProductsVM.Products.CategoryId;
            productsFromDb.SupplierId = ProductsVM.Products.SupplierId;
            productsFromDb.TagId = ProductsVM.Products.TagId;
            productsFromDb.Price = ProductsVM.Products.Price;
            productsFromDb.UnitsOnOder = ProductsVM.Products.UnitsOnOder;
            productsFromDb.UnitsInStock = ProductsVM.Products.UnitsInStock;
            productsFromDb.Discontinued = ProductsVM.Products.Discontinued;
            productsFromDb.Description = ProductsVM.Products.Description;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductsVM.Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p=>p.Tag).SingleOrDefaultAsync(p => p.Id == id);
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }

        //GET delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductsVM.Products = await _db.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Tag).FirstOrDefaultAsync(p => p.Id == id);
            if (ProductsVM.Products == null)
            {
                return NotFound();
            }
            return View(ProductsVM);
        }
        //POST delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Product products = await _db.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }
            else
            {
                var upload = Path.Combine(webRootPath, SD.ProductImageFolder);
                var extension = Path.GetExtension(products.Image);

                if (System.IO.File.Exists(Path.Combine(upload, products.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(upload, products.Id + extension));
                }
                _db.Products.Remove(products);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}