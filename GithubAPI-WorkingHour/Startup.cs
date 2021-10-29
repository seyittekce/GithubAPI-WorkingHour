using Business.Repository;
using Business.User;
using Core;
using Core.Abstracts;
using Core.Concrete;
using Core.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Octokit;
using System;
namespace GithubAPI_WorkingHour
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
            services.AddTransient<IGitHubClient>(s => new GitHubClient(new ProductHeaderValue("Test")));
            services.AddTransient<IRepositoryService, RepositoryManager>();
            services.AddTransient<IUserService, UserManager>();
            services.AddSingleton<IPassword, Password>();
            services.AddSingleton<IWorkingHourCalculator, WorkingHourCalculator>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<GithubApiKeys>(Configuration.GetSection("GithubApi"));
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(int.MaxValue);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}