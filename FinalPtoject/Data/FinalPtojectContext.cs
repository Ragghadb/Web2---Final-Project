using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinalPtoject.Models;
using FinalPtoject;

namespace FinalPtoject.Data
{
    public class FinalPtojectContext : DbContext
    {
        public FinalPtojectContext (DbContextOptions<FinalPtojectContext> options)
            : base(options)
        {
        }

        public DbSet<FinalPtoject.Models.Usersall> Usersall { get; set; } = default!;

        public DbSet<FinalPtoject.getname>? getname { get; set; }

       

        public DbSet<FinalPtoject.Models.orderdetail>? orderdetail { get; set; }
    

        public DbSet<FinalPtoject.Models.items>? items { get; set; }

        public DbSet<FinalPtoject.Models.orderdetail>? orderdetail_1 { get; set; }

        public DbSet<FinalPtoject.Models.orders>? orders { get; set; }
        public DbSet<FinalPtoject.Models.Report>? Report { get; set; }

    }
}
