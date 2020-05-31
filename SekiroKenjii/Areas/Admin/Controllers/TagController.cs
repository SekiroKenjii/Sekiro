using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SekiroKenjii.Data;
using SekiroKenjii.Models;
using SekiroKenjii.Utility;

namespace SekiroKenjii.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TagController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Tags.OrderBy(c => c.Name).ToListAsync());
        }

        //GET create
        public IActionResult Create()
        {
            return View();
        }
        //POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _db.Tags.Add(tag);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        //GET Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = await _db.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        //POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _db.Update(tag);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = await _db.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var tag = await _db.Tags.FindAsync(id);
            if (tag == null)
            {
                return View();
            }
            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = await _db.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
    }
}
