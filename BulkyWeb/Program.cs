using BulkyWeb.Data;
using BulkyWeb.Repository;
using Microsoft.AspNetCore.Identity;
using BulkyWeb.BulkyUtilities;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Stripe;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using BulkyWeb.DbInitializer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Adding entityframeworkcore in the contaning
//Indicating we wanna use the Sql Definition
builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//
//Configure StripeSettings which is a class in bulkyutilities folder
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//Add Sessions Service
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout= TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly= true;
    options.Cookie.IsEssential= true;
});


// AddDefaultIdentity allows us to have the default so we can't have the role for the use , however below in AddIdentity
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

//In AddIdentity we can add role
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//
//To properly Manage the display of the error such as 404
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//GoogleServices
//builder.Services.AddAuthentication()

//facebook services
builder.Services.AddAuthentication().AddFacebook(option =>
{
    option.AppId = SD.AppIdFacebook;
    option.AppSecret = SD.AppSecretFacebook;
}
) ;

//We added builder.Services.AddRazorPages() so to allow razor pages to be displayed in our project 
// we can site the register and the login and we call them further in program by app.MapRazorPages();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
//add DbInitializer service
builder.Services.AddScoped<IDbInitializer,DbInitializer>();
builder.Services.AddScoped<EmailSender>();
var app = builder.Build();
//
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//Api stripe configuration
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();
// Essential to use authentification app.UseAuthentication() always comes before app.UseAuthorization();
app.UseAuthentication();
app.UseAuthorization();
//Add Session service to the app
app.UseSession();
//Call SeedDb
SeedDataBase();
// Call razor pages
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
//Invoke DbInitializer
void SeedDataBase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        //Never forget dbInitializer.Initialize();
        dbInitializer.Initialize();
    }
}