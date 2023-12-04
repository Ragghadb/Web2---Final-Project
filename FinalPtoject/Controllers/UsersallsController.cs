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
using System.Net.Mail;

namespace FinalPtoject.Controllers
{
    public class UsersallsController : Controller
    {
        private readonly FinalPtojectContext _context;

        public UsersallsController(FinalPtojectContext context)
        {
            _context = context;
        }

        // GET: Usersalls
        public async Task<IActionResult> Index()
        {
              return _context.Usersall != null ? 
                          View(await _context.Usersall.ToListAsync()) :
                          Problem("Entity set 'FinalPtojectContext.Usersall'  is null.");
        }



    

        public IActionResult addadmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> addadmin([Bind("name,password")] Usersall Usersall)
        {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalPtoject");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "select * from userall where name ='" + Usersall.name + "' ";
            Boolean flage = false;
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                flage = true;
            }
            reader.Close();
            if (flage == true)
            {
                ViewData["message"] = "name already exists";
            }
            else
            {
                var role = "admin";
                sql = "insert into userall (name,password,role) values ('" + Usersall.name + "','" + Usersall.password + "','" + role + "')";
                comm = new SqlCommand(sql, conn1);
                comm.ExecuteNonQuery();
                return RedirectToAction(nameof(Index));
            }
            conn1.Close();

            return View();
        }


     


        public IActionResult customer_home()
        {
            return View();
        }


        public IActionResult email()
        {
            return View();

        }

        [HttpPost]
        public IActionResult email(string address, string body, string subject)
        {

            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("ireemnafea@gmail.com");
            mail.To.Add(address); // receiver email address
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("ireemnafea@gmail.com", "ztna qidl exzb rkdy");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["Message"] = "Email sent.";
            return View();
        }
   

        // GET: Usersalls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usersall == null)
            {
                return NotFound();
            }

            var usersall = await _context.Usersall
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersall == null)
            {
                return NotFound();
            }

            return View(usersall);
        }

        // GET: Usersalls/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult login()
        {
            return View();
        }

        [HttpPost, ActionName("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(string na, string pa)
        {
            SqlConnection conn1 = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=Final;Integrated Security=True;Pooling=False");
            string sql;
            sql = "SELECT * FROM usersall where name ='" + na + "' and  pass ='" + pa + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string role = (string)reader["role"];
                string id = Convert.ToString((int)reader["Id"]);
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("userid", id);
                reader.Close();
                conn1.Close();
                if (role == "customer")
                    return RedirectToAction("catalogue", "books");

                else
                    return RedirectToAction("Index", "books");

            }
            else
            {
                ViewData["Message"] = "wrong user name password";
                return View();
            }
        }

        // POST: Usersalls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,password,RegistDate")] Usersall usersall)
        {
            if (ModelState.IsValid)
            {
                usersall.role = "castomer";
                _context.Add(usersall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(login));
            }
            return View(usersall);
        }

        // GET: Usersalls/Registration
        public IActionResult Registration()
        {
            return View();
        }

        // POST: useralls/Registration
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration([Bind("name,password")] Usersall userall)
        {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "select * from usersall where name ='" + userall.name + "' ";
            Boolean flage = false;
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                flage = true;
            }
            reader.Close();
            if (flage == true)
            {
                ViewData["message"] = "name already exists";
            }
            else
            {
                var role = "customer";
                sql = "insert into usersall (name,password,role) values ('" + userall.name + "','" + userall.password + "','" + role + "')";
                comm = new SqlCommand(sql, conn1);
                comm.ExecuteNonQuery();
                return RedirectToAction(nameof(Index));
            }
            conn1.Close();

            return View();
        }


        public async Task<IActionResult> Searchall()
        {

            {
                Usersall brItem = new Usersall();

                return View(brItem);
            }
        }

        [HttpPost]

        public async Task<IActionResult> SearchAll(string tit)
        {
            var bkItems = await _context.Usersall.FromSqlRaw("select * from usersall where name = '" + tit + "' ").FirstOrDefaultAsync();

            return View(bkItems);
        }



        // GET: Usersalls/Edit/5
        public async Task<IActionResult> Edit()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            var usersall = await _context.Usersall.FindAsync(id);
            
            return View(usersall);
        }

        // POST: Usersalls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,password,role,RegistDate")] Usersall usersall)
        {
 
                    _context.Update(usersall);
                    await _context.SaveChangesAsync();
                
             
              
                return RedirectToAction(nameof(login));
            }
          

        // GET: Usersalls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usersall == null)
            {
                return NotFound();
            }

            var usersall = await _context.Usersall
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usersall == null)
            {
                return NotFound();
            }

            return View(usersall);
        }

        // POST: Usersalls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usersall == null)
            {
                return Problem("Entity set 'FinalPtojectContext.Usersall'  is null.");
            }
            var usersall = await _context.Usersall.FindAsync(id);
            if (usersall != null)
            {
                _context.Usersall.Remove(usersall);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersallExists(int id)
        {
          return (_context.Usersall?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
