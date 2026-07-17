using GymSystem.BLL.Services.Classes;
using GymSystem.BLL.Services.Interfaces;
using GymSystem.BLL.Utilities;
using GymSystem.DAL.Contexts;
using GymSystem.DAL.Entities;
using GymSystem.DAL.Repositories.Classes;
using GymSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace A_MVC01
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
            });
            //builder.Services.AddScoped<IPlanRepository, PlanRepository>();
            //builder.Services.AddScoped(typeof(IGenericRepository<>) ,typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IMemberServices,MemberServices>();
            builder.Services.AddScoped<ISessionServices, SessionServices>();
            builder.Services.AddScoped<IPlanServices, PlanServices>();
            builder.Services.AddScoped<ITrainerServices,TrainerServices>();
            builder.Services.AddScoped<IAnalyticsServices,AnalyticsServices>();
            builder.Services.AddScoped<IAttachementServices,AttachementServices>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                //option.Password.RequiredLength = 6;
                //option.Password.RequireLowercase = true;
                //option.Password.RequireUppercase = true;

                option.User.RequireUniqueEmail = false;
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
            })
                .AddEntityFrameworkStores<GymDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";

                options.AccessDeniedPath = "/Account/AccessDenied";
            
            });

            builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
            var app = builder.Build();

            await app.MigrateAndSeedAsync();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
