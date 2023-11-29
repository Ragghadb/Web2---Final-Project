using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalPtoject;
using FinalPtoject.Data;
using Microsoft.Data.SqlClient;

namespace FinalPtoject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class newbookController : ControllerBase
    {
        // getting all book search catagory
        [HttpGet("{cat}")]
        public IEnumerable<getname> Get(string cat)
        {
            List<getname> li = new List<getname>();
          
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT * FROM useresall where role ='" + cat + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {
                li.Add(new getname
                {
                    name = (string)reader["name"],
                });

            }

            reader.Close();
            conn1.Close();
            return li;
        }
    }
}








