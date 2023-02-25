using Habit.API;
using Habit.API.Infastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaskManager.WebHost.Customization;
using Microsoft.Extensions.Configuration;

namespace Habit.FunctionalTest
{
    public class HabitScenariosBase
    {

        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(HabitScenariosBase))
                .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                }).UseStartup<HabitTestsStartup>();


            var testServer = new TestServer(hostBuilder);

            testServer.Host.
                MigrateDbContext<HabitContext>((_, __) => { });

            return testServer;
        }

        public static class Get
        {

            public static string GetHabitsByUserId()
            {
                return "api/v1/habit/habit";
            }
        }
    }
}
