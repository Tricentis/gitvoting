using GIG.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GIG.Data {
    public class GIGContext : DbContext {

        public GIGContext() : base("GIGContext") {
        }

        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Vote>()
                .HasKey(c => new { c.Year, c.Username, c.Team });
        }
    }
}