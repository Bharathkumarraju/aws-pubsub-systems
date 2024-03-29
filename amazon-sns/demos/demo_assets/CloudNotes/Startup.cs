using System;
using CloudNotes.Models.Settings;
using CloudNotes.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloudNotes
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
            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // Setup options with DI
            services.AddOptions();

            services.AddAuthorization();

            services.AddAuthentication(o => {
                o.DefaultChallengeScheme = Constants.GitHubAuthenticationScheme;
                o.DefaultSignInScheme = Constants.CookieAuthenticationScheme;
                o.DefaultAuthenticateScheme = Constants.CookieAuthenticationScheme;
            })
                .AddCookie(Constants.CookieAuthenticationScheme, o =>
                {
                    o.LoginPath = new PathString("/Account/Login/");
                    o.AccessDeniedPath = new PathString("/Account/AccessDenied/");
                })
                .AddGitHub(
                    Configuration["Authentication:GitHub:ClientId"],
                    Configuration["Authentication:GitHub:ClientSecret"],
                    Constants.CookieAuthenticationScheme
                );

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            services.AddSingleton(provider =>
                new DistributedCacheEntryOptions()
                .SetSlidingExpiration(
                    TimeSpan.FromMinutes(
                        int.Parse(Configuration["Caching:CacheExpirationInMinutes"])
                    )
                )
            );

            // Configure S3Settings using config
            services.Configure<S3Settings>(Configuration.GetSection("Storage"));
            services.Configure<SNSSettings>(Configuration.GetSection("EventPublishing"));

            services.AddSingleton<INoteStorageService, S3NoteStorageService>();
            services.AddSingleton<IEventPublisher, SNSEventPublisher>();

            services.AddControllersWithViews(config =>
            {
                foreach (var formatter in config.InputFormatters)
                {
                    Console.WriteLine(formatter.GetType().ToString());
                    if (formatter.GetType() == typeof(SystemTextJsonInputFormatter))
                        ((SystemTextJsonInputFormatter)formatter).SupportedMediaTypes.Add(
                            Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/plain"));
                }
            }
            );

            // call this in case you need aspnet-user-authtype/aspnet-user-identity
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
