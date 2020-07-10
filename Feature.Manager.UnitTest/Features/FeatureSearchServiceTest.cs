using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.Manager.Api.Features;
using Feature.Manager.Api.Features.Exceptions;
using Feature.Manager.Api.Uuid;
using Moq;
using NUnit.Framework;

namespace Feature.Manager.UnitTest.Features
{
    public class FeatureSearchServiceTest
    {
        private FeatureSearchService _systemUnderTest;
        private Mock<IFeatureSearchRepository> _mock;

        [SetUp]
        public void Setup()
        {
            var mock = new Mock<IFeatureSearchRepository>();
            mock
                .Setup(x => x.SearchFeatureByFeatId(It.Is<string>(x => !x.StartsWith("ER"))))
                .ReturnsAsync((string x) =>
                {
                    var uuid = new UuidService();
                    return new List<Api.Features.Feature>
                    {
                        new Api.Features.Feature
                        {
                            Description = "rand description 1",
                            Hypothesis = "rand hypo 2",
                            Id = uuid.GenerateUuId(),
                            FeatureToken = uuid.GenerateUuId(),
                            FeatId = $"{x}-001"
                        },
                        new Api.Features.Feature
                        {
                            Description = "rand description 2",
                            Hypothesis = "rand hypo 2",
                            Id = uuid.GenerateUuId(),
                            FeatureToken = uuid.GenerateUuId(),
                            FeatId = $"{x}-002"
                        },
                        new Api.Features.Feature
                        {
                            Description = "rand description 3",
                            Hypothesis = "rand hypo 3",
                            Id = uuid.GenerateUuId(),
                            FeatureToken = uuid.GenerateUuId(),
                            FeatId = $"{x}-003"
                        }
                    };
                });
            mock
                .Setup(x => x.SearchFeatureByFeatId(It.Is<string>(x => x.StartsWith("ER"))))
                .ThrowsAsync(new InvalidCastException());
            _mock = mock;
            _systemUnderTest = new FeatureSearchService(_mock.Object);
        }

        [Test]
        public async Task FeatureSearchServiceReturnsWhateverRepositoryReturns()
        {
            var result = await _systemUnderTest.SearchFeatureByFeatId("TEST");
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void FeatureSearchServiceHandlesExceptions()
        {
            Assert.ThrowsAsync<UnknownDbException>(() => _systemUnderTest.SearchFeatureByFeatId("ERR"));
        }
    }
}
