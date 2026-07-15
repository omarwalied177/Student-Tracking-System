using StudentTrackingSystem.BLL.Interfaces;
using StudentTrackingSystem.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using StudentTrackingSystem.BLL.Repositories;
using StudentTrackingSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using StudentTrackingSystem.Pl.Services;
using Serilog;
using Serilog.Events;

namespace StudentTrackingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting web application");
                var builder = WebApplication.CreateBuilder(args);

                // Add Serilog to the application
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
                builder.Services.AddScoped<IStudentRepository, StudentRepository>();

                builder.Services.AddScoped<ITeatcherRepository, TeatcherRepository>();
                builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
                builder.Services.AddScoped<IParentRepository, ParentRepository>();
                builder.Services.AddScoped<IGradeRepository, GradeRepository>();
                builder.Services.AddScoped<IEmailService, EmailService>();
                builder.Services.AddScoped<EmailService>();

                // Add Generic Repository for Attendance
                builder.Services.AddScoped<IGenaricRepository<Attendance>, GenaricRepository<Attendance>>();

                // Ensure the 'UseSqlServer' method is available by adding the correct using directive
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 5;

                    // User settings
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/SignIn";
                    options.LogoutPath = "/Account/SignOut";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                    options.SlidingExpiration = true;
                });

                //builder.Services.AddAutoMapper(typeof(StudentMapper));

                var app = builder.Build();

                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();
                    dbContext.DataSeed();
                }

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
