using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalPtoject.Data;
using FinalPtoject.Models;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.Data.SqlClient;

namespace FinalPtoject.Controllers
{
    public class ordersController : Controller
    {
        private readonly FinalPtojectContext _context;

        public ordersController(FinalPtojectContext context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
              return _context.order != null ? 
                          View(await _context.order.ToListAsync()) :
                          Problem("Entity set 'FinalPtojectContext.order'  is null.");
        }

        public async Task<IActionResult> report()
        {
            var orItems = await _context.orderdetail.FromSqlRaw("select usersall.id as Id, usersall.name as customername, sum (quantity * price) as total from itemsall, orders, usersall where  itemid = itemsall.Id, and custid = usersall.Id group by usersall.id, usersall.name ").ToListAsync();
            return View(orItems);
        }


        public async Task<IActionResult> Buy(int? id)
        {
            string ss = HttpContext.Session.GetString("role");

            if (ss == "customer")
            {
                var ord = await _context.order.FindAsync(id);
                return View(ord);
            }
            else
                return RedirectToAction("login", "Usersall");
        }

        [HttpPost]
        public async Task<IActionResult> Buy(int itemId, int quantity)
        {
            order order = new order();
            order.itemid = itemId;
            order.quantity = quantity;
            order.userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            order.buydate = DateTime.Today;
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalProjectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql;
            int qt = 0;
            sql = "select * from orders where (id ='" + order.itemid + "' )";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                qt = (int)reader["bookquantity"]; // store quantity
            }
            reader.Close();
            conn.Close();
            if (order.quantity > qt)
            {
                ViewData["message"] = "maxiumam order quantity sould be " + qt;
                var ord = await _context.order.FindAsync(itemId);
                return View(ord);
            }
            else
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                sql = "UPDATE items  SET quantity  = quantity   - '" + order.quantity + "'  where (id ='" + order.itemid + "' )";
                comm = new SqlCommand(sql, conn);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
                return RedirectToAction(nameof(Index));
            }
        }



        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.order == null)
            {
                return NotFound();
            }

            var order = await _context.order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

     



        // GET: orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,itemid,userid,quantity,buydate")] order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.order == null)
            {
                return NotFound();
            }

            var order = await _context.order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,itemid,userid,quantity,buydate")] order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!orderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.order == null)
            {
                return NotFound();
            }

            var order = await _context.order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.order == null)
            {
                return Problem("Entity set 'FinalPtojectContext.order'  is null.");
            }
            var order = await _context.order.FindAsync(id);
            if (order != null)
            {
                _context.order.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool orderExists(int id)
        {
          return (_context.order?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
