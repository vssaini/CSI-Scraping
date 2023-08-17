using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CSI.Data
{
    public partial class ScrapperContext : DbContext
    {
        public ScrapperContext()
            : base("name=ScrapperEntities")
        {
        }

        public virtual DbSet<Staging_ProductExtract> Staging_ProductExtract { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
