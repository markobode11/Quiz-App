using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DAL.App.EF;
using DAL.App.EF.AppDataInit;
using Domain.App.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApp
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
            services.AddDbContext<AppDbContext>(options =>
                options
                    .UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging());

            services.AddDatabaseDeveloperPageExceptionFilter();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication()
                .AddCookie(options => { options.SlidingExpiration = true; }
                )
                .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidIssuer = Configuration["JWT:Issuer"],
                            ValidAudience = Configuration["JWT:Issuer"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                            ClockSkew = TimeSpan.Zero
                        };
                    }
                );

            services
                .AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();

            services.AddCors(options =>
                {
                    options.AddPolicy("CorsAllowAll", builder =>
                    {
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowAnyOrigin();
                    });
                }
            );

            services.AddAutoMapper(
                typeof(PublicApi.DTO.v1.MappingProfiles.AutoMapperProfile)
            );

            // add support for api versioning
            services.AddApiVersioning(options => { options.ReportApiVersions = true; });
            // add support for m2m api documentation
            services.AddVersionedApiExplorer(options => { options.GroupNameFormat = "'v'VVV"; });
            // add support to generate human readable documentation from m2m docs
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var appSupportedCultures = new[]
                {
                    new CultureInfo("en-GB"),
                };

                options.SupportedCultures = appSupportedCultures;
                options.SupportedUICultures = appSupportedCultures;
                options.DefaultRequestCulture = new RequestCulture("en-GB", "en-GB");
                options.SetDefaultCulture("en-GB");
                options.RequestCultureProviders = new List<IRequestCultureProvider>()
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            SetupAppData(app, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var apiVersionDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{apiVersionDescription.GroupName}/swagger.json",
                        apiVersionDescription.GroupName.ToUpperInvariant()
                    );
                }
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization(
                app.ApplicationServices
                    .GetService<IOptions<RequestLocalizationOptions>>()?.Value
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsAllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        private static void SetupAppData(IApplicationBuilder app, IConfiguration configuration)
        {
            var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();

            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            // in case of testing do not setup
            if (context!.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory") return;

            var userManager = serviceScope.ServiceProvider.GetService<UserManager<AppUser>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<AppRole>>();

            var logger = serviceScope.ServiceProvider.GetService<ILogger<Startup>>();
            if (logger == null)
            {
                throw new ApplicationException("Problem in services. Can't initialize logger");
            }

            if (context == null)
            {
                throw new ApplicationException("Problem in services. Can't initialize AppDbContext");
            }

            if (userManager == null || roleManager == null)
            {
                throw new ApplicationException(
                    $"Problem in services. UserManager {(userManager == null ? "null" : "ok")}" +
                    $" RoleManager {(roleManager == null ? "null" : "ok")}");
            }

            if (configuration.GetValue<bool>("AppData:DropDatabase"))
            {
                DataInit.DropDatabase(context, logger);
            }

            if (configuration.GetValue<bool>("AppData:Migrate"))
            {
                DataInit.MigrateDatabase(context, logger);
            }

            if (configuration.GetValue<bool>("AppData:SeedIdentity"))
            {
                DataInit.SeedIdentity(userManager, roleManager, logger);
            }

            if (configuration.GetValue<bool>("AppData:SeedData"))
            {
                DataInit.SeedAppData(context, userManager, logger);
            }
        }
    }
}