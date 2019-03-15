using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectGrpSVA.DataAccess.DL
{
    public class DataModel : DbContext
    {
        public DataModel() : base("DataModel")
        {
        }

        private readonly string _connectionString;

        public DataModel(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Aisle> Aisle { get; set; }
        public DbSet<Order_Products> Order_Products { get; set; }
        public DbSet<Departments> Departments { get; set; }
    }
}