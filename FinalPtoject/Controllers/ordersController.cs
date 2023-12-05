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
using System.Net;

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

        public async Task<IActionResult> Buy(int? Id)
        {
            var item = await _context.items.FindAsync(Id);
            string ss = HttpContext.Session.GetString("role");
            if (ss == "admin")
                return View(item);
            else
                return RedirectToAction("login", "Usersalls"); }


        [HttpPost]
        public async Task<IActionResult> Buy(int itemid, int quantity)
        {
            order order = new order();
            order.itemid = itemid;
            order.quantity = quantity;
            order.userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            order.buydate = DateTime.Today;
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql;
            int qt = 0;
            sql = "select * from items where (id ='" + order.itemid + "' )";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                qt = (int)reader["quantity"]; // store quantity
            }
            reader.Close();
            conn.Close();
            if (order.quantity > qt)
            {
                ViewData["message"] = "Maximum order quantity should be " + qt;
                var items = await _context.items.FindAsync(itemid);
                return View(items);
            }
            else
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                sql = "UPDATE items SET quantity = quantity - " + order.quantity + " where id ='" + order.itemid + "'";
                comm = new SqlCommand(sql, conn);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
                ViewData["message"] = "Thank you for your purchase!";
                var items = await _context.items.FindAsync(itemid);
                return RedirectToAction(nameof(Index));



            }
        }












        public async Task<IActionResult> order_detail(int? idd)
        {
            var orItems = await _context.order.FromSqlRaw
                ("select usersall.id, usersall.name as username, orders.buydate as BuyDate, items.price * orders.quantity as TotalPrice," +
                " orders.quantity as quantity from orders, usersall, items  where  userid =" +
                " '" + idd + "'  and usersall.Id = orders.userid and orders.itemid = items.id   ").ToListAsync();
            return View(orItems);
        }

        public async Task<IActionResult> report()
        {
            var orItems = await _context.items.FromSqlRaw
                ("select usersall.id as Id, usersall.name as customername, " +
                "sum(quantity * price) as total from itemsall," +
                " orders, usersall where  itemid = itemsall.Id, and custid =" +
                " usersall.Id group by usersall.id, usersall.name ").ToListAsync();
            return View(orItems);

        }

public async Task<IActionResult> myprunchase()
        {
            int userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            var orItems = await _context.orders.FromSqlRaw("select *  from orders where  userid = '" + userid + "'  ").ToListAsync();
            return View(orItems);
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
        public async Task<IActionResult> Create(int? id)
        {
            var items = await _context.items.FindAsync(id);
            return View(items);
        }

        // POST: orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int itemid, int quantity)
        {
            if (!ModelState.IsValid)
            {
                // If not valid, return the view with error messages
                return View(await _context.items.FindAsync(itemid));
            }

            order order = new order
            {
                itemid = itemid,
                quantity = quantity,
                userid = Convert.ToInt32(HttpContext.Session.GetString("userid")),
                buydate = DateTime.Today
            };

            _context.Add(order);
            await _context.SaveChangesAsync();

            // Update item quantity using Entity Framework
            var item = await _context.items.FindAsync(itemid);
            if (item != null)
            {
                item.quantity -= quantity;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(order_detail));
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
