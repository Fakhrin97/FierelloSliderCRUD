using Fiorello.Areas.Admin.Data;
using Fiorello.DAL;
using Fiorello.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fiorello
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMvc().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            builder.Services
                .AddSession(opt => opt.IdleTimeout = TimeSpan.FromSeconds(45));
            builder.Services
                .AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.
                AddIdentity<User, IdentityRole>(options =>
                {
                    options.Lockout.MaxFailedAccessAttempts= 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(45);

                    options.SignIn.RequireConfirmedEmail = false;

                    options.User.RequireUniqueEmail = true;

                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;   
                    options.Password.RequireUppercase = false;   
                   
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders(); 
            Constants.RootPath = builder.Environment.WebRootPath;
            var app = builder.Build();
            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/ErrorPages/Error","?code={0}");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=dashboard}/{action=Index}/{id?}"
                  );
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=home}/{action=index}/{id?}"
                 );
              
            });
            app.Run();
        }
    }
}