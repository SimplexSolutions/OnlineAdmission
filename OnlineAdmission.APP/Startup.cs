using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineAdmission.APP.Utilities.NagadSetting;
using OnlineAdmission.BLL.IManager;
using OnlineAdmission.BLL.Manager;
using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.DAL.Repository;
using OnlineAdmission.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineAdmission.APP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                //options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            

            services.AddDbContext<OnlineAdmissionDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<OnlineAdmissionDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<OnlineAdmissionDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
            });


            services.AddControllersWithViews();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlDataContractSerializerFormatters();

            services.AddMvc(options => options.EnableEndpointRouting = false);
            //services.AddRazorPages();
            
            //Code for AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IStudentManager, StudentManager>();
            services.AddScoped<IStudentRepository, StudentRepository>();

            services.AddScoped<IMeritStudentManager, MeritStudentManager>();
            services.AddScoped<IMeritStudentRepository, MeritStudentRepository>();

            services.AddScoped<IDepartmentManager, DepartmentManager>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            services.AddScoped<IDistrictManager, DistrictManager>();
            services.AddScoped<IDistrictRepository, DistrictRepository>();

            services.AddScoped<IPaymentTransactionManager, PaymentTransactionManager>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

            services.AddScoped<ISubjectManager, SubjectManager>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();

            services.AddScoped<IAppliedStudentManager, AppliedStudentManager>();
            services.AddScoped<IAppliedStudentRepository, AppliedStudentRepository>();

            services.AddScoped<ISMSManager, SMSManager>();
            services.AddScoped<ISMSRepository, SMSRepository>();

            //services.AddScoped<INagadManager, NagadManager>();
            //services.AddScoped<INagadRepository, NagadRepository>();

            services.AddScoped<IStudentCategoryRepository, StudentCategoryRepository>();
            services.AddScoped<IStudentCategoryManager, StudentCategoryManager>();

            services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddScoped<IPaymentTypeManager, PaymentTypeManager>();

            services.AddScoped<IMeritTypeRepository, MeritTypeRepository>();
            services.AddScoped<IMeritTypeManager, MeritTypeManager>();

            services.AddScoped<IAcademicSessionRepository, AcademicSessionRepository>();
            services.AddScoped<IAcademicSessionManager, AcademicSessionManager>();

            services.AddScoped<ISecurityKey, SecurityKey>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Students}/{action=Search}/{id?}");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Students}/{action=Search}/{id?}");
            //        //pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
