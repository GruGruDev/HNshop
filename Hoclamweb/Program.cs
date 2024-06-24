using Suitshop.DataAccess.Data;
using Suitshop.DataAccess.Repository;
using Suitshop.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Suitshop.Utility;
using Suitshop.Models;
using Stripe;
using Suitshop.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>
	(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//truyền dữ liệu từ appsetting vào trong lớp Stripe theo đúng tên các thuộc tính.
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(option =>
{
	option.LoginPath = $"/Identity/Account/Login";
	option.LogoutPath = $"/Identity/Account/Logout";
	option.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication().AddFacebook(option =>
{
	option.AppId = "3503848449877571";
	option.AppSecret = "7e4047a9cf134877631614760420cdfb";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
	option.IdleTimeout = TimeSpan.FromMinutes(10);
	option.Cookie.HttpOnly = true;
	option.Cookie.IsEssential = true;
});

builder.Services.AddHealthChecks();

builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
SeedData();

app.Run();

void SeedData()
{
	using (var scope = app.Services.CreateScope())
	{
		var dbInititalizer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInititalizer.Initializer();
	}
}