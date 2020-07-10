using System;
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
        [SetUp]
        public void Setup()
        {
            var repo = new Mock<IFeatureRepository>();
            repo.Setup(x => x.FindByFeatId("TEST-123")).ReturnsAsync(new Api.Features.Feature
            {
                Description = "test description",
                Hypothesis = "test hypo",
                Id = "test-id",
                ExperimentToken = "exp-token",
                FeatId = "TEST-123"
            });
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
                        ExperimentToken = uuid.GenerateUuId()
                    };
                });
            _mockRepository = repo;
        }

        [Test]
        public async Task CreateThrowsErrorIfDuplicate()
        {
            var systemUnderTest = new FeatureService(_mockRepository.Object);
            Assert.ThrowsAsync<FeatureAlreadyExistsException>(async () =>
            {
                await systemUnderTest.Create(new CreateFeatureRequest
                {
                    Description = "random new description",
                    Hypothesis = "random hypo",
                    FeatId = "TEST-123"
                });
            });
        }

        [Test]
        public async Task CreateReturnsNewlyCreatedDataIfNotDuplicate()
        {
            var systemUnderTest = new FeatureService(_mockRepository.Object);
            var response = await systemUnderTest.Create(new CreateFeatureRequest
            {
                Description = "description",
                Hypothesis = "hypothesis",
                FeatId = "TEST-124"
            });
            Assert.NotNull(response.ExperimentToken);
            Assert.NotNull(response.Id);
            Assert.AreEqual("TEST-124", response.FeatId);
            Assert.AreEqual("hypothesis", response.Hypothesis);
            Assert.AreEqual("description", response.Description);
        }

        [Test]
        public async Task CreateThrowsFeatureCreatingExceptionIfCreationFailsTest()
        {
            var systemUnderTest = new FeatureService(_mockRepository.Object);
            Assert.ThrowsAsync<FeatureCreatingFailedException>(async () =>
            {
                await systemUnderTest.Create(new CreateFeatureRequest
                {
                    Description = "description",
                    Hypothesis = "hypothesis",
                    FeatId = "TEST-1"
                });
            });
        }
    }
}
