using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FinalPtoject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class getnameAPI : ControllerBase
    {  // getting all book search catagory
        [HttpGet("{cat}")]
        public IEnumerable<getname> Get(string cat)
        {
            List<getname> li = new List<getname>();
            //  SqlConnection conn1 = new SqlConnection("Data Source=.\sqlexpress;Initial Catalog=Final;Integrated Security=True;Pooling=False");
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("FinalPtojectContext");
            SqlConnection conn1 = new SqlConnection(conStr);
            string sql;
            sql = "SELECT * FROM usersall where role ='" + cat + "' ";
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
