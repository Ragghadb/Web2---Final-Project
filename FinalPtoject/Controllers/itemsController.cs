using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalPtoject.Data;
using FinalPtoject.Models;
using Microsoft.Data.SqlClient;

namespace FinalPtoject.Controllers
{
    public class itemsController : Controller
    {
        private readonly FinalPtojectContext _context;

        public itemsController(FinalPtojectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> statis()
        {
            {
                string sql = "";

                var builder = WebApplication.CreateBuilder();
                string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
                SqlConnection conn = new SqlConnection(conStr);

                SqlCommand comm;
                conn.Open();
                sql = "SELECT COUNT( Id)  FROM items where category =1";
                comm = new SqlCommand(sql,conn);
                ViewData["d1"] = (int)comm.ExecuteScalar();

                sql = "SELECT COUNT( Id)  FROM items where category =2";
                comm = new SqlCommand(sql, conn);
                ViewData["d2"] = (int)comm.ExecuteScalar();
                return View();
            }
        }




        // GET: items
        public async Task<IActionResult> Index()
        {
            ViewData["role"] = HttpContext.Session.GetString("Role");

            return _context.items != null ?
                  View(await _context.items.ToListAsync()) :
                 Problem("Entity set 'FinalPtojectContext.items'  is null.");
        }

        // GET: items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }
        public async Task<IActionResult> image_slider()
        {
            return _context.items != null ?
                        View(await _context.items.ToListAsync()) :
                        Problem("Entity set 'FinalPtojectContext.items'  is null.");
        }
        public async Task<IActionResult> list()
        {
            return _context.items != null ?
                 View(await _context.items.OrderBy(m => m.category).ToListAsync()) :
                Problem("Entity set 'FinalPtojectContext.items'  is null.");
        }
     

        // GET: items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file,[Bind("Id,name,descr,price,quantity,discount,category")] items items)
        
            {
                if (file != null)
                {
                    string filename = file.FileName;
                    //  string  ext = Path.GetExtension(file.FileName);
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    items.imagefilename = filename;
                }

                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }

        // GET: items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);



        }

        // POST: items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile file, int id, [Bind("Id,name,descr,price,quantity,discount,category,imagefilename")] items items)
        {
     
            if (id != items.Id)
            { return NotFound(); }

            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                items.imagefilename = filename;
            }
            _context.Update(items);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            return View(items);
        }

        // GET: items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.items == null)
            {
                return Problem("Entity set 'FinalPtojectContext.items'  is null.");
            }
            var items = await _context.items.FindAsync(id);
            if (items != null)
            {
                _context.items.Remove(items);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool itemsExists(int id)
        {
          return (_context.items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
