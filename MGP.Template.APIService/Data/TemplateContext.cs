using MGP.Models.NetCoreLibrary31.Auth;
using MGP.Models.NetCoreLibrary31.BackOffice;
using Microsoft.EntityFrameworkCore;

namespace MGP.Template.APIService.Data
{
    public class TemplateContext : DbContext
    {
        public TemplateContext(DbContextOptions<TemplateContext> options) : base(options)
        {
        }

        public DbSet<MGPLogin_v> MGPLogin_vSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MGPLogin_v>().ToTable("MGPLogin_v");
        }
    }
}
