using BulkyWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;

namespace BulkyWeb.Data
{
    //Beforewe used Dbcontext and our  ApplicationDbContext Inerhit the DbContext class which comes from the EntityFrameworkCore
    //Now using IdentityDbContext can still does the same work as Dbcontext , for its use we must import
    //using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    //<IdentityUser> we must import using Microsoft.AspNetCore.Identity;
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers {get;set;}
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        //Seeding data , adding some additional behavior to the function

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (builder.IsConfigured)
                return;
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();
            builder.UseSqlServer(configuration.GetConnectionString("Default"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // base.OnModelCreating(modelBuilder) allows to map the model table in EF in your database
            base.OnModelCreating(modelBuilder);

            //Convertion Dateonly to DateTime

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                dateOnly => dateOnly.ToDateTime(new System.TimeOnly(0,0)),
                dateTime => DateOnly.FromDateTime(dateTime)); 
            modelBuilder.Entity<OrderHeader>().Property(e => e.PaymentDueDate).HasConversion(dateOnlyConverter);

            modelBuilder.Entity<Category>().HasData(

                new Category { Id=1,Name="Biography",DisplayOrder=9},
                new Category { Id = 2, Name = "ScFi", DisplayOrder = 9 },
                new Category { Id=3,Name="Motivation",DisplayOrder=5}
                );
            modelBuilder.Entity<Company>().HasData(

                new Company { 
                    Id = 1, Name = "Tesla", 
                    StreetAddress = "st 10th essex" ,
                    City="Texas" ,
                    State="Texas State",
                    PostalCode="1556334",
                    PhoneNumber="+1723456788" },
                new Company { 
                    Id = 2, 
                    Name = "Amazon", 
                    StreetAddress = "st 10th essex", 
                    City = "Texas",
                    State = "Texas State", 
                    PostalCode = "1556334", 
                    PhoneNumber = "+1723456788" },
                new Company { Id = 3    , Name = "Space X", 
                    StreetAddress = "st 10th essex", 
                    City = "Texas", 
                    State = "Texas State", 
                    PostalCode = "1556334", 
                    PhoneNumber = "+1723456788" }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Fortune of Time",
                    Author = "Billy Spark",
                    Description = "Praesent vitae sodales libero. Praesent molecule",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId= 1,
                    ImageUrl= @"\images\Products\pexels-ron-lach-9758163.jpg"
                },
                 new Product
                 {
                     Id = 2,
                     Title = "Can't hurt me",
                     Author = "David Goggins",
                     Description = "For David Goggins, childhood was a nightmare -- poverty, prejudice, and physical abuse colored his days and haunted his nights",
                     ISBN = "SWD9999002",
                     ListPrice = 9.9,
                     Price = 9,
                     Price50 = 8.5,
                     Price100 = 8,
                     CategoryId = 2,
                     ImageUrl = @"\images\Products\pexels-ron-lach-9758163.jpg"
                 },
                 new Product
                 {
                     Id = 3,
                     Title = "Attack on Titan",
                     Author = "Isayama Hajime",
                     Description = "In a fictional world of terror , how can freedom be defined ? ",
                     ISBN = "SWD9999002",
                     ListPrice = 59,
                     Price = 50,
                     Price50 = 55,
                     Price100 = 40,
                     CategoryId = 3,
                     ImageUrl = @"\images\Products\pexels-ron-lach-9758163.jpg"
                 },
                  new Product
                  {
                      Id = 4,
                      Title = "Ma vie sans Gravite",
                      Author = "Thomas Pesquet",
                      Description = "Dive into his fascination , the childhood dream , from the aircraf pilote to the nation hero",
                      ISBN = "SWD9999001",
                      ListPrice = 29,
                      Price = 20,
                      Price50 = 25,
                      Price100 = 10,
                      CategoryId = 2,
                      ImageUrl = @"\images\Products\pexels-ron-lach-9758163.jpg"
                  },
                  new Product
                  {
                      Id = 5,
                      Title = "Freedom",
                      Author = "Jeremy Griffith",
                      Description = "In the conspiracies theory , what's the meaning of life",
                      ISBN = "SWD9999005",
                      ListPrice = 70,
                      Price = 79,
                      Price50 = 55,
                      Price100 = 40,
                      CategoryId = 2,
                      ImageUrl = @"\images\Products\pexels-ron-lach-9758163.jpg"
                  }
                ); 
        }
    }
}