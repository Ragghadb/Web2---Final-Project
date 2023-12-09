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
using System.Composition;

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
              return _context.orders != null ? 
                          View(await _context.orders.ToListAsync()) :
                          Problem("Entity set 'FinalPtojectContext.orders'  is null.");
        }

        //-
        public async Task<IActionResult> order_detail(int? id)
        {
            List<orderdetail> list = new List<orderdetail>();

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT usersall.id as Id,usersall.name as username,items.name as itemname,orders.buydate as BuyDate,items.price * orders.quantity as TotalPrice ,orders.quantity as quantity FROM orders JOIN usersall ON orders.userid = usersall.id JOIN items ON orders.itemid = items.id Where orders.userid = '" + id + "' order by orders.buydate DESC";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new orderdetail
                {
                    Id = (int)reader["Id"],
                    username = (string)reader["username"],
                    itemname = (string)reader["itemname"],
                    buydate = (DateTime)reader["Buydate"],
                    totalprice = (int)reader["TotalPrice"],
                    quantity = (int)reader["quantity"]
                });
            }
            return View(list);


        }


        //-
        public async Task<IActionResult> Report()
        {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
            {
                //   var orItems = await _context.report.FromSqlRaw("select userall.id as Id, userall.name as customername, sum(orders.quantity*items.price) as total, orders.buydate as Buydate from items, orders, userall where  itemid = items.Id and userid = userall.Id group by userall.id, userall.name,orders.buydate order by orders.buydate ").ToListAsync();
                // return View(orItems);
                List<Report> list = new List<Report>();

                var builder = WebApplication.CreateBuilder();
                string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
                SqlConnection conn1 = new SqlConnection(conStr);
                string sql;
                sql = "select usersall.id as Id, usersall.name as customername, sum(orders.quantity*items.price) as total from items, orders, usersall where  itemid = items.Id and userid = usersall.Id group by usersall.id, usersall.name";
                SqlCommand comm = new SqlCommand(sql, conn1);
                conn1.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Report
                    {
                        Id = (int)reader["Id"],
                        customername = (string)reader["customername"],
                        total = (int)reader["total"],

                    });
                }
                return View(list);

            }
            else
                return RedirectToAction("Login", "Userslls");
        }


        //-
        public async Task<IActionResult> my_purchase()
        {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "customer")
            {
                int userid = (int)HttpContext.Session.GetInt32("userid");
                //var orItems = await _context.orders.FromSqlRaw("select *  from orders where  userid = '" + userid + "'  ").ToListAsync();

                List<my_purchase> list = new List<my_purchase>();

                var builder = WebApplication.CreateBuilder();
                string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
                SqlConnection conn1 = new SqlConnection(conStr);
                string sql;
                sql = "SELECT usersall.id as Id,items.name as itemname,orders.Buydate as buydate,(items.price * orders.quantity) as Price ,orders.quantity as quantity FROM orders JOIN usersall ON orders.userid = usersall.id JOIN items ON orders.itemid = items.id Where orders.userid = '" + userid + "'";
                SqlCommand comm = new SqlCommand(sql, conn1);
                conn1.Open();
                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new my_purchase
                    {
                        Id = (int)reader["Id"],
                        name = (string)reader["itemname"],
                        buydate = (DateTime)reader["buydate"],
                        price = (int)reader["Price"],
                        quantity = (int)reader["quantity"]
                    });
                }
                return View(list);

            }
            else
                return RedirectToAction("login", "Usersalls");
           

        }

        public async Task<IActionResult> Buy(int? id)
        {
            var item = await _context.items.FindAsync(id);
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> Buy(int itemid, int quantity)
        {
            orders order = new orders();
            order.itemid = itemid;
            order.userid = (int)HttpContext.Session.GetInt32("userid");
            order.quantity = quantity;
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
                ViewData["message"] = "Maxiumam order quantity should be " + qt;
                var item = await _context.items.FindAsync(itemid);
                return View(item);
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
                return RedirectToAction(nameof(my_purchase));
            }
        }














        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
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
        public async Task<IActionResult> Create([Bind("Id,itemid,userid,quantity,buydate")] orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,itemid,userid,quantity,buydate")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
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
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.orders == null)
            {
                return Problem("Entity set 'FinalPtojectContext.orders'  is null.");
            }
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
          return (_context.orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
