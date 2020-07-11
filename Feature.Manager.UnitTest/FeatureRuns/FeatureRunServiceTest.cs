using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.FeatureRuns;
using Feature.Manager.Api.FeatureRuns.Exceptions;
using Feature.Manager.Api.FeatureRuns.ViewModels;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Features.ViewModels;
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
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-1")).ReturnsAsync(new List<FeatureRun>
            {
                new FeatureRun
                {
                    Allocation = 100,
                    Id = "123",
                    EndAt = DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                    FeatId = "APP-1",
                    RunToken = "12312312313",
                    StartAt = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                    StopResult = StopResult.ChangeSettings,
                },
                new FeatureRun
                {
                    Allocation = 100,
                    Id = "123",
                    EndAt = null,
                    FeatId = "APP-1",
                    RunToken = "12312312313",
                    StartAt = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                }
            });
            mock.Setup(x => x.GetRunsForFeatureByFeatId("APP-2")).ThrowsAsync(new InvalidCastException());
            _mock = mock;
            _featureRunService = new FeatureRunService(_mock.Object, null);
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


        [Test]
        public async Task TestGetRunByFeatureIdHandlesException()
        {
            Assert.ThrowsAsync<UnknownDbException>(() => _featureRunService.GetRunsForFeatureByFeatId("APP-2"));
        }

        [Test]
        public async Task TestGetRunByFeatureIdReturnsListOfFeatures()
        {
            var response = await _featureRunService.GetRunsForFeatureByFeatId("APP-1");
            Assert.IsNotEmpty(response);
        }

        [Test]
        public async Task TestGetRunningFeaturesReturnsFromRepository()
        {
            var mock = new Mock<IFeatureRunRepository>();
            var mockData = new List<RunningFeature>
            {
                new RunningFeature
                {
                    Allocation = 100,
                    FeatureId = "APP-123",
                    FeatureToken = "1231223",
                    RunId = "1231234124",
                    RunStatus = StopResult.AllB,
                    RunToken = "2134142334234",
                },
                new RunningFeature
                {
                    Allocation = 100,
                    FeatureId = "APP-125",
                    FeatureToken = "1231223",
                    RunId = "1231234124",
                    RunStatus = StopResult.AllB,
                    RunToken = "2134142334234",
                },
            };
            mock.Setup(x => x.GetRunningFeatures()).ReturnsAsync(mockData);
            var systemUnderTest = new FeatureRunService(mock.Object, null);
            var result = await systemUnderTest.GetRunningFeatures();
            Assert.AreSame(mockData, result);
        }

        [Test]
        public async Task TestGetRunningFeaturesHandlesException()
        {
            var mock = new Mock<IFeatureRunRepository>();
            mock.Setup(x => x.GetRunningFeatures()).ThrowsAsync(new InvalidCastException());
            var systemUnderTest = new FeatureRunService(mock.Object, null);
            Assert.ThrowsAsync<UnknownDbException>(() => systemUnderTest.GetRunningFeatures());
        }
    }
}
