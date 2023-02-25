using Habit.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Habit.FunctionalTest
{
    public class HabitTestsStartup : Startup
    {
        public HabitTestsStartup(IConfiguration env) : base(env)
        {
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(Configuration);
            return base.ConfigureServices(services);
        }

        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}
