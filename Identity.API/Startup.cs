﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;

namespace Services.Identity.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        RegisterAppInsights(services);

        // Add framework services.
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionString"],
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                }));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.AddCors();
        services.Configure<AppSettings>(Configuration);

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });


        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddSqlServer(Configuration["ConnectionString"],
                name: "IdentityDB-check",
                tags: new string[] { "IdentityDB" });

        services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();
        services.AddTransient<IRedirectService, RedirectService>();

        var connectionString = Configuration["ConnectionString"];
        var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        // Adds IdentityServerAddIdentityServer
        services.AddIdentityServer(x =>
        {
            x.IssuerUri = "null";
            x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
        })
        //.AddSigningCredential(Certificate.Get())
        .AddAspNetIdentity<ApplicationUser>()
        .AddDeveloperSigningCredential()
        .AddJwtBearerClientAuthentication()
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
        })
        .AddProfileService<ProfileService>();

        services.AddControllers();

        services.AddControllersWithViews();
        services.AddRazorPages();


        var container = new ContainerBuilder();
        container.Populate(services);

        return new AutofacServiceProvider(container.Build());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        var pathBase = Configuration["PATH_BASE"];
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }
        app.UseForwardedHeaders();
        app.UseStaticFiles();

        // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
        //app.Use(async (context, next) =>
        //{
        ////    context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
        //    await next();
        //});

        app.UseForwardedHeaders();
        app.UseCors(options =>
        {
            options.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
        app.UseIdentityServer();

        app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });
    }

    private void RegisterAppInsights(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry(Configuration);
        services.AddApplicationInsightsKubernetesEnricher();
    }
}
