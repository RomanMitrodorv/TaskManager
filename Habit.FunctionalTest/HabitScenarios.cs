using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Habit.FunctionalTest
{
    public class HabitScenarios : HabitScenariosBase
    {
        [Fact]
        public async Task Get_habits_and_response_ok_status_code()
        {
            using var server = CreateServer();
            var response = await server.CreateClient()
                .GetAsync(Get.GetHabitsByUserId());

            response.EnsureSuccessStatusCode();
        }

    }
}
