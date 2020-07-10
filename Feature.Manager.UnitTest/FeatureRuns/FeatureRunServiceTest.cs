using System;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns;
using Feature.Manager.Api.FeatureRuns.Exceptions;
using Feature.Manager.Api.FeatureRuns.ViewModels;
using Feature.Manager.Api.Features.Exceptions;
using Moq;
using NUnit.Framework;

namespace Feature.Manager.UnitTest.FeatureRuns
{
    public class FeatureRunServiceTest
    {
        private Mock<IFeatureRunRepository> _mock;
        private FeatureRunService _featureRunService;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IFeatureRunRepository>();
            mock.Setup(x => x.GetById("RUN-UUID-1")).ReturnsAsync(new FeatureRun
            {
                Allocation = 100,
                Id = "RUN-UUID-1",
                EndAt = DateTime.Now.Add(TimeSpan.FromDays(5)),
                StartAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                FeatId = "APP-1",
                RunToken = "RAND-UUID-TOKEN",
            });
            mock.Setup(x => x.GetById("problematic_id_1")).ReturnsAsync(new FeatureRun
            {
                Allocation = 100,
                Id = "problematic_id_1",
                EndAt = DateTime.Now.Add(TimeSpan.FromDays(5)),
                StartAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                FeatId = "APP-1",
                RunToken = "RAND-UUID-TOKEN",
            });
            mock.Setup(x => x.GetById("RUN-UUID-3")).ThrowsAsync(new InvalidCastException());
            mock.Setup(x => x.StopFeatureRun(It.IsAny<StopFeatureRunRequest>())).ReturnsAsync(
                (StopFeatureRunRequest request) =>
                {
                    if (request.RunId == "problematic_id_1")
                    {
                        throw new InvalidCastException();
                    }

                    Enum.TryParse(request.StopResult, out StopResult stopResult);
                    return new FeatureRun
                    {
                        Allocation = 100,
                        Id = request.RunId,
                        EndAt = DateTime.Now.Add(TimeSpan.FromDays(5)),
                        FeatId = "APP-1",
                        RunToken = "run-token",
                        StartAt = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                        StopResult = stopResult
                    };
                });
            _mock = mock;
            _featureRunService = new FeatureRunService(_mock.Object);
        }

        [Test]
        public async Task TestFindByIdReturnsFromRepository()
        {
            var result = await _featureRunService.GetById("RUN-UUID-1");
            Assert.AreSame("APP-1", result.FeatId);
            Assert.IsNull(await _featureRunService.GetById("RUN-UUID-2"));
        }

        [Test]
        public async Task TestFindByIdHandlesException()
        {
            Assert.ThrowsAsync<UnknownDbException>(() => _featureRunService.GetById("RUN-UUID-3"));
        }

        [Test]
        public async Task TestStopFeatureRunThrowsNotFound()
        {
            Assert.ThrowsAsync<FeatureRunNotFoundException>(() => _featureRunService.StopFeatureRun(new StopFeatureRunRequest
            {
                RunId = "RUN-UUID-2",
                StopResult = StopResult.ChangeSettings.ToString()
            }));
        }

        [Test]
        public async Task TestStopFeatureRunFailsWhenStopResultIsInvalid()
        {
            Assert.ThrowsAsync<InvalidStopResultValueException>(() => _featureRunService.StopFeatureRun(new StopFeatureRunRequest
            {
                RunId = "RUN-UUID-1",
                StopResult = "ALL"
            }));
            Assert.ThrowsAsync<InvalidStopResultValueException>(() => _featureRunService.StopFeatureRun(new StopFeatureRunRequest
            {
                RunId = "RUN-UUID-1",
                StopResult = "BLAH"
            }));
        }

        [Test]
        public async Task TestStopFeatureRunHandlesException()
        {
            Assert.ThrowsAsync<UnknownDbException>(() => _featureRunService.StopFeatureRun(new StopFeatureRunRequest
            {
                RunId = "problematic_id_1",
                StopResult = "AllA"
            }));
        }

        [Test]
        public async Task TestStopFeatureRunReturnsNewData()
        {
            var result = await _featureRunService.StopFeatureRun(new StopFeatureRunRequest
            {
                RunId = "RUN-UUID-1",
                StopResult = "AllA"
            });
            Assert.AreEqual(StopResult.AllA, result.StopResult);
        }
    }
}
