using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using WebApplication11.Models;


public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult FailedStudentCount()
        {
    
        SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=1;Integrated Security=True;Pooling=False");

        string sql;
     sql = "SELECT COUNT( Id) FROM su where status  ='failed' and classid =  '1200'";
        SqlCommand comm = new SqlCommand(sql, conn);
        conn.Open();

        int count= (int)comm.ExecuteScalar();
        string mm = Convert.ToString(count);




      
        conn.Close();
        ViewData["F"] = mm;
        return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
