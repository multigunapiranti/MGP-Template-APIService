using MGP.Models.NetCoreLibrary31.Log;
using MGP.Template.APIService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MGP.Template.APIService.Data
{
    public class LogContext : DbContext
    {
        public LogContext(DbContextOptions<LogContext> options) : base(options)
        {
        }

        public DbSet<ErrorException_TH> ErrorException_THSet { get; set; }
        public DbSet<LogAPI_TH> LogAPI_THSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ErrorException_TH>().ToTable("errorexception_th");
            modelBuilder.Entity<LogAPI_TH>().ToTable("logapi_th");
        }
    }
}
