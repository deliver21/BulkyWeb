using Bulky.Models;
using Bulky.Models.Models;
using Microsoft.EntityFrameworkCore;
namespace Bulky.DataAccess.Data
{
    //Inerhit the DbCont class which comes from the EntityFrameworkCore
    public class ApplicationDbContext: DbContext
    {       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        //Seeding data , adding some additional behavior to the function
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id=1,Name="Action", DisplayOrder=1},
                new Category { Id = 2, Name = "Scifi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 },
                new Category { Id=4,Name="Cisco",DisplayOrder=4},
                new Category { Id=5,Name="Psycholgy", DisplayOrder=5},
                new Category { Id=6,Name="Cinematography",DisplayOrder=6},
                new Category { Id=11,Name="German",DisplayOrder=7},
                new Category { Id=15,Name="Law",DisplayOrder=9}
                );
        }
    }
}
