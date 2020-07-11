using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns;
using Feature.Manager.Api.FeatureRuns.Exceptions;
using Feature.Manager.Api.FeatureRuns.ViewModels;
using Feature.Manager.Api.Features;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Uuid;
using Moq;
using NUnit.Framework;

namespace Feature.Manager.UnitTest.FeatureRuns
{
    public class FeatureRunCreateServiceTest
    {
        private FeatureRunService _featureRunService;
        private Mock<IFeatureRepository> _featureRepository;
        private Mock<IFeatureRunRepository> _mock;
        private UuidService _uuidService;

        private FeatureRun MakeFeatureRun(StopResult? stopResult, string featId, DateTime? endAt)
        {
            var run = new FeatureRun
            {
                Allocation = 100,
                Id = _uuidService.GenerateUuId(),
                StartAt = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                RunToken = _uuidService.GenerateUuId(),
                FeatId = featId
            };
            if (stopResult.HasValue) run.StopResult = stopResult.Value;

            if (endAt.HasValue) run.EndAt = endAt;

            return run;
        }

        [SetUp]
        public void Setup()
        {
            _uuidService = new UuidService();
            var mock = new Mock<IFeatureRunRepository>();
            mock.Setup(x => x.CreateFeatureRun(It.IsAny<CreateFeatureRunRequest>())).ReturnsAsync(new FeatureRun
            {
                Allocation = 100,
                Id = "rand",
                FeatId = "APP-2",
                RunToken = "run-token",
                StartAt = DateTime.Now.Subtract(TimeSpan.FromHours(2))
            });

            // APP-1 will have 1 item with no end date, (NO STOP RESULT)
            // APP-2 will have 2 item with second one with no end date, (ONE OF THEM WILL HAVE STOP RESULT)
            // APP-3 will have end date on all of them, (EACH ONE WILL HAVE STOP RESULT)
            // APP-4 will have no items,
            // APP-5 will have 1 item with end date BUT with stop result of all B (to show that even if you set it to B, you can still create new runs)
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-1")).ReturnsAsync(new List<FeatureRun>
            {
                MakeFeatureRun(null, "APP-1", null)
            });
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-2")).ReturnsAsync(new List<FeatureRun>
            {
                MakeFeatureRun(StopResult.ChangeSettings, "APP-2", DateTime.Now.Subtract(TimeSpan.FromDays(2))),
                MakeFeatureRun(null, "APP-2", null)
            });
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-3")).ReturnsAsync(new List<FeatureRun>
            {
                MakeFeatureRun(StopResult.ChangeSettings, "APP-3", DateTime.Now.Subtract(TimeSpan.FromDays(2))),
                MakeFeatureRun(StopResult.ChangeSettings, "APP-3", DateTime.Now.Subtract(TimeSpan.FromDays(1))),
                MakeFeatureRun(StopResult.ChangeSettings, "APP-3", DateTime.Now.Subtract(TimeSpan.FromHours(12)))
            });
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-4")).ReturnsAsync(new List<FeatureRun>());
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-5")).ReturnsAsync(new List<FeatureRun>
            {
                MakeFeatureRun(StopResult.AllB, "APP-5", DateTime.Now.Subtract(TimeSpan.FromHours(12)))
            });
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-19")).ReturnsAsync(new List<FeatureRun>());
            _mock = mock;
            var featureRepository = new Mock<IFeatureRepository>();
            featureRepository.Setup(x => x.FindByFeatId(It.IsAny<string>())).ReturnsAsync((string featId) =>
            {
                if (featId == "APP-19")
                {
                    return null;
                }
                return new Api.Features.Feature
                {
                    Description = "asdfasdfasdf",
                    Hypothesis = "asdfasdfsd",
                    Id = "asdfasdf",
                    FeatId = featId,
                    FeatureToken = "asldf"
                };
            });
            _featureRepository = featureRepository;
            _featureRunService = new FeatureRunService(_mock.Object, _featureRepository.Object);
        }

        [Test]
        public async Task TestCannotCreateNewRunIfARunIsAlreadyRunning()
        {
            Assert.ThrowsAsync<FeatureAlreadyRunningException>(() => _featureRunService.CreateFeatureRun(
                new CreateFeatureRunRequest
                {
                    Allocation = 100,
                    EndAt = DateTime.Now.Add(TimeSpan.FromDays(20)),
                    StartAt = DateTime.Now,
                    FeatId = "APP-1"
                }));
            Assert.ThrowsAsync<FeatureAlreadyRunningException>(() => _featureRunService.CreateFeatureRun(
                new CreateFeatureRunRequest
                {
                    Allocation = 100,
                    EndAt = DateTime.Now.Add(TimeSpan.FromDays(20)),
                    StartAt = DateTime.Now,
                    FeatId = "APP-2"
                }));
        }

        [Test]
        public async Task TestCreatesNewRunWhenNoFeaturesAreRunning()
        {
            var result = await _featureRunService.CreateFeatureRun(new CreateFeatureRunRequest
            {
                Allocation = 100,
                EndAt = DateTime.Now.Add(TimeSpan.FromDays(20)),
                StartAt = DateTime.Now,
                FeatId = "APP-3"
            });
            Assert.NotNull(result);
        }

        [Test]
        public async Task TestCreateFailsIfFeatureDoesNotExist()
        {
            Assert.ThrowsAsync<FeatureNotFoundException>(() => _featureRunService.CreateFeatureRun(
                new CreateFeatureRunRequest
                {
                    Allocation = 100,
                    EndAt = DateTime.Now.Add(TimeSpan.FromDays(20)),
                    StartAt = DateTime.Now,
                    FeatId = "APP-19"
                }));
        }
    }
}
