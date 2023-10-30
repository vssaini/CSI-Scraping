using System.Data.Entity;

namespace CSI.Data
{
    public class ScrapperContext : DbContext
    {
        public ScrapperContext() : base("name=ScrapperEntities")
        {
        }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
