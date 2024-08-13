using BulkyWeb.BulkyUtilities;
using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public DbInitializer(
      UserManager<IdentityUser> userManager,
      RoleManager<IdentityRole> roleManager,
      ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }


        public void Initialize()
        {

            //migration if it's not created
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            //_db.Database.EnsureCreated();
            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                //Adding role incase where it's not set
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();


                //if roles are not created , then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser{
                    UserName = "delivrewelo",
                    Email = "admin@gmail.com",
                    Name = "Delivre Ahanga",
                    PhoneNumber = "+375257694851",
                    StreetAddress = "Dubko 20",
                    PostalCode = "23005",
                    State = "Grodno State",
                    City = "Grodno"
                },"Admin1.").GetAwaiter().GetResult();


                var user =_db.ApplicationUsers.FirstOrDefault(u => u.Email == "delivrewelo@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }      
    }
}
