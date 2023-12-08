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
using static System.Reflection.Metadata.BlobBuilder;

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


        public async Task<IActionResult> logout()
        {
        
HttpContext.Session.Remove( "Name");
HttpContext.Session.Remove("Role");
HttpContext.Response.Cookies.Delete("Name");
return RedirectToAction("Login");
    }





        //Role
        public IActionResult addadmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> addadmin([Bind("name,password")] Usersall Usersall)

{
    string ss = HttpContext.Session.GetString("Role");
    if (ss == "admin")
    {

        var builder = WebApplication.CreateBuilder();
        string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
        SqlConnection conn1 = new SqlConnection(conStr);
        string sql;
        sql = "select * from usersall where name ='" + Usersall.name + "' ";
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
            sql = "insert into usersall (name,password,role) values ('" + Usersall.name + "','" + Usersall.password + "','" + role + "')";
            comm = new SqlCommand(sql, conn1);
            comm.ExecuteNonQuery();
            return RedirectToAction(nameof(Index));
        }
        conn1.Close();

        return View();
    }
            else
                return RedirectToAction("login", "Usersalls");
        }




        //8
        public async Task<IActionResult> customer_home()
        {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "customer")
            {
                ViewData["NAME"] = HttpContext.Session.GetString("Name");
                return View(await _context.items.ToListAsync());


            }  
            else
                return RedirectToAction("login", "Usersalls");
        }




        //8

        public async Task<IActionResult> admin_home(int? id)
			
              {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
            {
                ViewData["NAME"] = HttpContext.Session.GetString("Name");
                return View(await _context.items.ToListAsync());


    }  
            else
                return RedirectToAction("login", "Usersalls");
}




        //Role

        public IActionResult email()
        {
            return View();

        }

        [HttpPost]
        public IActionResult email(string address, string body, string subject)
        {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
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
            else
                    return RedirectToAction("Login", "usersall");


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









        //login
        public IActionResult login()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Name"))
                return View();
            else
            {
                string na = HttpContext.Request.Cookies["Name"].ToString();
                string ro = HttpContext.Request.Cookies["Role"].ToString();
                string userid = HttpContext.Request.Cookies["id"].ToString();
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", ro);
                HttpContext.Session.SetString("userid", userid);

                return View();
            }
        }

        [HttpPost, ActionName("login")]
        public async Task<IActionResult> login(string na, string pa, string auto)
        {
            SqlConnection conn1 = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=Final;Integrated Security=True;Pooling=False");
            string sql = "SELECT * FROM usersall where name ='" + na + "' and  password ='" + pa + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string na1 = (string)reader["name"];
                string ro = (string)reader["role"];
                int id = (int)reader["id"];
                HttpContext.Session.SetString("Name", na1);
                HttpContext.Session.SetString("Role", ro);
                HttpContext.Session.SetInt32("userid", id);

                reader.Close();
                conn1.Close();

                if (auto == "true")
                {
                    HttpContext.Response.Cookies.Append("Name", na1);
                    HttpContext.Response.Cookies.Append("Role", ro);
                }


                if (ro == "admin")
                {
                    return RedirectToAction("admin_home", "Usersalls");
                }
                else
                {
                    return RedirectToAction("customer_home", "Usersalls");
                }

            }
            else
            {
                ViewData["Message"] = "wrong user name or password";
                return View();
            }
        }

        // GET: Usersalls/Registration
        public IActionResult registration()
        {

            return View();
        }

        [HttpPost]
        public IActionResult registration(string name, string password)
        {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
            SqlConnection conn = new SqlConnection(conStr);
            conn.Open();
            string sql;
            Boolean flage = false;
            sql = "select * from usersall where name = '" + name + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {

                flage = true;
            }
            reader.Close();
            if (flage == true)
            {
                ViewData["error"] = "name already exists";
                conn.Close();
                return View();
            }
            else
            {
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", "customer");
                sql = "insert into usersall (name,password,Registdate,role) values ('" + name + "','" + password + "',CURRENT_TIMESTAMP,'customer')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                sql = "select id from usersall where name = '" + name + "'";
                comm = new SqlCommand(sql, conn);
                int userId = (int)comm.ExecuteScalar();
                HttpContext.Session.SetInt32("userid", userId);

                return RedirectToAction(nameof(customer_home));
            }
        }





        public async Task<IActionResult> customer_search()
        {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
            {

                {
                Usersall brItem = new Usersall();

                return View(brItem);
            }
            }
            else
                return RedirectToAction("Login", "usersall");


        }

        [HttpPost]
        public async Task<IActionResult> customer_search(string tit)
        {
           
                var bkItems = await _context.Usersall.FromSqlRaw("select * from usersall where name = '" + tit + "' ").FirstOrDefaultAsync();
            return View(bkItems);
            }
     




        // GET: Usersalls/Edit/5
        public async Task<IActionResult> Edit()
        {
           
            int id = (int)HttpContext.Session.GetInt32("userid");
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
