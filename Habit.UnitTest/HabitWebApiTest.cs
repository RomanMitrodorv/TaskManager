using Castle.Core.Logging;
using Habit.API.Controllers;
using Habit.API.Infastructure;
using Habit.API.Models;
using Habit.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Moq;

namespace Habit.UnitTest
{
    public class HabitWebApiTest
    {
        private static readonly Guid _fakeUserId = Guid.NewGuid();

        private readonly DbContextOptions<HabitContext> _dbOptions;

        public HabitWebApiTest()
        {
            _dbOptions = new DbContextOptionsBuilder<HabitContext>()
                                    .UseInMemoryDatabase(databaseName: "in-memory")
                                    .Options;

            using var dbContext = new HabitContext(_dbOptions);
            dbContext.AddRange(GetFakeHabits());
            dbContext.SaveChanges();

        }

        [Fact]
        public async Task Get_habit_by_userId_success()
        {
            var habitContext = new HabitContext(_dbOptions);

            var identityServiceMock = new Mock<IIdentityService>();

            identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(_fakeUserId.ToString());

            var loggerMock = new Mock<ILogger<HabitController>>();
            var telegramServiceMock = new Mock<ITelegramService>();

            var controller = new HabitController(habitContext, identityServiceMock.Object, loggerMock.Object, telegramServiceMock.Object);

            var actionResult = await controller.HabitByUserIdAsync();

            Assert.IsType<ActionResult<List<HabitModel>>>(actionResult);

            var habit = Assert.IsAssignableFrom<List<HabitModel>>(actionResult.Value);

            Assert.Equal(((OkObjectResult)actionResult.Result).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal(3, habit.Count);
        }

        [Fact]
        public async Task Get_habit_by_userId_not_found()
        {
            var newUserId = Guid.NewGuid().ToString();

            var habitContext = new HabitContext(_dbOptions);

            var identityServiceMock = new Mock<IIdentityService>();

            identityServiceMock.Setup(x => x.GetUserIdentity()).Returns(newUserId);

            var loggerMock = new Mock<ILogger<HabitController>>();
            var telegramServiceMock = new Mock<ITelegramService>();

            var controller = new HabitController(habitContext, identityServiceMock.Object, loggerMock.Object, telegramServiceMock.Object);

            var actionResult = await controller.HabitByUserIdAsync();

            Assert.Equal(((NotFoundResult)actionResult.Result).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }


        private static List<HabitModel> GetFakeHabits()
        {
            return new List<HabitModel>
            {
                new ()
                {
                    Id = 1,
                    Name = "Test",
                    CompletedCount = 1,
                    Count = 2,
                    DateCreation = DateTime.Now,
                    Notifications= new List<Notification> {},
                    Periodicity = new Periodicity() { Id = 1, Code = "Test", Name = "Test"},
                    PeriodicityId = 1,
                    UserId = _fakeUserId,
                },
                new()
                {
                    Id = 2,
                    Name = "Test 2",
                    CompletedCount = 2,
                    Count = 3,
                    DateCreation = DateTime.Now,
                    Notifications= new List<Notification> {},
                    Periodicity = new Periodicity() { Id = 2, Code = "Test 2", Name = "Test 2"},
                    PeriodicityId = 2,
                    UserId = _fakeUserId,
                },
                new()
                {
                    Id = 3,
                    Name = "Test 3",
                    CompletedCount = 5,
                    Count = 6,
                    DateCreation = DateTime.Now,
                    Notifications= new List<Notification> {},
                    Periodicity = new Periodicity() { Id = 3, Code = "Test 3", Name = "Test 3"},
                    PeriodicityId = 3,
                    UserId = _fakeUserId,
                }
            };
        }
    }
}