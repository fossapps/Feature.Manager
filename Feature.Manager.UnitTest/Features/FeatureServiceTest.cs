using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.Features;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Features.ViewModels;
using Feature.Manager.Api.Uuid;
using Moq;
using NUnit.Framework;

namespace Feature.Manager.UnitTest.Features
{
    public class FeatureServiceTest
    {
        private Mock<IFeatureRepository> _mockRepository;
        private FeatureService _systemUnderTest;
        private readonly List<Api.Features.Feature> _featureList = new List<Api.Features.Feature>
        {
            new Api.Features.Feature
            {
                Description = "desc1",
                Hypothesis = "hypo2",
                Id = "123123123",
                FeatId = "APP-1",
                FeatureToken = "2134123123"
            },
            new Api.Features.Feature
            {
                Description = "desc2",
                Hypothesis = "hypo2",
                Id = "098098098",
                FeatId = "APP-1",
                FeatureToken = "sdfalsdjfkdf"
            },
        };
        [SetUp]
        public void Setup()
        {
            var repo = new Mock<IFeatureRepository>();
            repo.Setup(x => x.All()).ReturnsAsync(_featureList);
            repo.Setup(x => x.FindByFeatId("TEST-123")).ReturnsAsync(new Api.Features.Feature
            {
                Description = "test description",
                Hypothesis = "test hypo",
                Id = "test-id",
                FeatureToken = "exp-token",
                FeatId = "TEST-123"
            });
            repo.Setup(x => x.FindByFeatId("TEST-2")).ReturnsAsync(new Api.Features.Feature
            {
                Description = "test description cant reset token",
                Hypothesis = "test hypo",
                Id = "test-id",
                FeatureToken = "exp-token",
                FeatId = "TEST-123"
            });
            repo.Setup(x => x.FindByFeatId("TEST-3")).ThrowsAsync(new InvalidCastException());
            repo.Setup(x => x.CreateFeature(It.IsAny<CreateFeatureRequest>())).ReturnsAsync(
                (CreateFeatureRequest request) =>
                {
                    var uuid = new UuidService();
                    if (request.FeatId == "TEST-1")
                    {
                        throw new InvalidCastException("something wrong");
                    }
                    return new Api.Features.Feature
                    {
                        Description = request.Description,
                        Hypothesis = request.Hypothesis,
                        FeatId = request.FeatId,
                        Id = uuid.GenerateUuId(),
                        FeatureToken = uuid.GenerateUuId()
                    };
                });
            _mockRepository = repo;
            repo.Setup(x => x.ResetFeatureToken("TEST-2", It.IsAny<string>())).ThrowsAsync(new InvalidCastException());

            repo.Setup(x => x.ResetFeatureToken(It.Is<string>(x => x != "TEST-2"), It.IsAny<string>()))
                .ReturnsAsync((string id, string newToken) =>
                {
                    var uuid = new UuidService();
                    return new Api.Features.Feature
                    {
                        Description = "rand description",
                        Hypothesis = "rand hypo",
                        Id = uuid.GenerateUuId(),
                        FeatureToken = newToken,
                        FeatId = id,
                    };
                });
            _systemUnderTest = new FeatureService(_mockRepository.Object, new UuidService());
        }

        [Test]
        public async Task CreateThrowsErrorIfDuplicate()
        {
            Assert.ThrowsAsync<FeatureAlreadyExistsException>(() => _systemUnderTest.Create(new CreateFeatureRequest
            {
                Description = "random new description",
                Hypothesis = "random hypo",
                FeatId = "TEST-123"
            }));
        }

        [Test]
        public async Task CreateReturnsNewlyCreatedDataIfNotDuplicate()
        {
            var response = await _systemUnderTest.Create(new CreateFeatureRequest
            {
                Description = "description",
                Hypothesis = "hypothesis",
                FeatId = "TEST-124"
            });
            Assert.NotNull(response.FeatureToken);
            Assert.NotNull(response.Id);
            Assert.AreEqual("TEST-124", response.FeatId);
            Assert.AreEqual("hypothesis", response.Hypothesis);
            Assert.AreEqual("description", response.Description);
        }

        [Test]
        public async Task CreateThrowsFeatureCreatingExceptionIfCreationFailsTest()
        {
            Assert.ThrowsAsync<FeatureCreatingFailedException>(() => _systemUnderTest.Create(new CreateFeatureRequest
            {
                Description = "description",
                Hypothesis = "hypothesis",
                FeatId = "TEST-1"
            }));
        }

        [Test]
        public async Task ResetFeatureTokenThrowsErrorIfFeatureDoesNotExist()
        {
            Assert.ThrowsAsync<FeatureNotFoundException>(() => _systemUnderTest.ResetFeatureToken("TEST-124"));
        }

        [Test]
        public async Task ResetFeatureTokenThrowsErrorIfResettingFails()
        {
            Assert.ThrowsAsync<FeatureTokenResetFailedException>(() => _systemUnderTest.ResetFeatureToken("TEST-2"));
        }

        [Test]
        public async Task ResetFeatureTokenReturnsNewData()
        {
            var originalData = await _mockRepository.Object.FindByFeatId("TEST-123");
            var result = await _systemUnderTest.ResetFeatureToken("TEST-123");
            Assert.AreNotSame(result.FeatureToken, originalData.FeatureToken);
        }

        [Test]
        public async Task GetFeatureByFeatIdReturnsDataFromRepo()
        {
            var response = await _systemUnderTest.GetFeatureByFeatId("TEST-123");
            Assert.NotNull(response);
            var mockData = await _mockRepository.Object.FindByFeatId("TEST-123");
            Assert.AreSame(mockData.Description, response.Description);
            Assert.AreSame(mockData.FeatureToken, response.FeatureToken);
            Assert.AreSame(mockData.FeatId, response.FeatId);
            Assert.AreSame(mockData.Id, response.Id);
        }

        [Test]
        public async Task GetFeatureByFeatIdReturnsNullIfNotFound()
        {
            var response = await _systemUnderTest.GetFeatureByFeatId("TEST-5555");
            Assert.Null(response);
        }

        [Test]
        public async Task GetFeatureByFeatIdHandlesExceptions()
        {
            Assert.ThrowsAsync<UnknownDbException>(async () => await _systemUnderTest.GetFeatureByFeatId("TEST-3"));
        }

        [Test]
        public async Task GetAllFeaturesHandlesExceptions()
        {
            var mock = new Mock<IFeatureRepository>();
            mock.Setup(x => x.All()).ThrowsAsync(new InvalidCastException());
            var systemUnderTest = new FeatureService(mock.Object, null);
            Assert.ThrowsAsync<UnknownDbException>(() => systemUnderTest.GetAllFeatures());
        }

        [Test]
        public async Task GetAllFeaturesReturnsFromRepo()
        {
            var result = await _systemUnderTest.GetAllFeatures();
            Assert.AreSame(_featureList, result);
        }
    }
}
