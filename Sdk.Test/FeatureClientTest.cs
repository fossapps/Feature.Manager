using System;
using System.Collections.Generic;
using Fossapps.FeatureManager;
using FossApps.FeatureManager.Models;
using Moq;
using NUnit.Framework;

namespace Sdk.Test
{
    public class FeatureClientTest
    {
        private IFeatureWorker _featureWorker;
        private IUserDataRepo _userDataRepo;
        private static string GetUuid()
        {
            return Guid.NewGuid().ToString();
        }

        [SetUp]
        public void Setup()
        {
            var runningFeatures = new List<RunningFeature>
            {
                new RunningFeature
                {
                    Allocation = 100,
                    FeatureId = "APP-1",
                    FeatureToken = GetUuid(),
                    RunId = GetUuid(),
                    RunToken = GetUuid()
                },
                new RunningFeature
                {
                    Allocation = 50,
                    FeatureId = "APP-2",
                    FeatureToken = GetUuid(),
                    RunId = GetUuid(),
                    RunToken = GetUuid()
                },
                new RunningFeature
                {
                    Allocation = 50,
                    FeatureId = "APP-3",
                    FeatureToken = GetUuid(),
                    RunId = GetUuid(),
                    RunStatus = "AllB",
                    RunToken = GetUuid()
                }
            };
            var mockWorker = new Mock<IFeatureWorker>();
            mockWorker.Setup(x => x.GetRunningFeatures()).Returns(runningFeatures);
            var mockUserData = new Mock<IUserDataRepo>();
            mockUserData.Setup(x => x.GetExperimentsForcedA()).Returns(new List<string>
            {
                "ALL_A-1",
                "ALL_A-2",
                "ALL_A-3",
            });
            mockUserData.Setup(x => x.GetExperimentsForcedB()).Returns(new List<string>
            {
                "ALL_B-1",
                "ALL_B-2",
                "ALL_B-3",
            });
            mockUserData.Setup(x => x.GetUserId()).Returns(GetUuid);
            _userDataRepo = mockUserData.Object;
            _featureWorker = mockWorker.Object;
        }

        [Test]
        public void TestFeatureClientReturnsSameVariantForMultipleRuns()
        {
            var mockUserData = new Mock<IUserDataRepo>();
            mockUserData.Setup(x => x.GetUserId()).Returns("a6e91dde-c35a-11ea-87d0-0242ac130003");
            var client = new FeatureClient(_featureWorker, mockUserData.Object);
            var firstVariant = client.GetVariant("APP-1");
            for (var i = 0; i < 1000; i++)
            {
                Assert.AreEqual(firstVariant, client.GetVariant("APP-1"));
            }
        }

        [Test]
        public void TestFeatureClientReturnsRoughlyHalfVariationsForMultipleRunsForRandomUsers()
        {
            var dictionary = new Dictionary<char, int> {['A'] = 0, ['B'] = 0, ['X'] = 0, ['Z'] = 0};
            var client = new FeatureClient(_featureWorker, _userDataRepo);
            for (var i = 0; i <= 1000000; i++)
            {
                var variant = client.GetVariant("APP-1");
                dictionary[variant] = dictionary[variant] + 1;
            }

            var total = dictionary['A'] + dictionary['B'];
            var diff = dictionary['A'] - dictionary['B'];
            var bias = ((float) diff / total) * 100;
            Assert.AreEqual(0, dictionary['Z']);
            Assert.AreEqual(0, dictionary['X']);
            Assert.LessOrEqual(Math.Abs(bias), 0.5);
        }

        [Test]
        public void TestFeatureWithAllocationHasLowAllocationBias()
        {
            var dictionary = new Dictionary<char, int> {['A'] = 0, ['B'] = 0, ['X'] = 0, ['Z'] = 0};
            var client = new FeatureClient(_featureWorker, _userDataRepo);
            for (var i = 0; i <= 1000000; i++)
            {
                var variant = client.GetVariant("APP-2");
                dictionary[variant] = dictionary[variant] + 1;
            }

            var variantA = dictionary['A'];
            var variantB = dictionary['B'];
            var variantZ = dictionary['Z'];
            Assert.AreEqual(0, dictionary['X']);
            var total = variantA + variantB + variantZ;
            var biasWithZ = variantA + variantB - variantZ;
            var biasWithZPerc = ((float) biasWithZ / total) * 100;
            var totalAllocated = variantA + variantB;
            var biasPerc = ((float) (variantA - variantB) / totalAllocated) * 100;
            Assert.LessOrEqual(biasPerc, 0.5);
            Assert.LessOrEqual(biasWithZPerc, 0.5);
        }

        [Test]
        public void TestFeatureWithAllBAlwaysReturnsB()
        {
            var dictionary = new Dictionary<char, int> {['A'] = 0, ['B'] = 0, ['X'] = 0, ['Z'] = 0};
            var client = new FeatureClient(_featureWorker, _userDataRepo);
            for (var i = 0; i < 1000000; i++)
            {
                var variant = client.GetVariant("APP-3");
                dictionary[variant] = dictionary[variant] + 1;
            }

            Assert.AreEqual(0, dictionary['X']);
            Assert.AreEqual(0, dictionary['Z']);
            Assert.AreEqual(0, dictionary['A']);
            Assert.AreEqual(1000000, dictionary['B']);
        }

        [Test]
        public void TestFeatureWithOverrideExperimentsToA()
        {
            var dictionary = new Dictionary<char, int> {['A'] = 0, ['B'] = 0, ['X'] = 0, ['Z'] = 0};
            var client = new FeatureClient(_featureWorker, _userDataRepo);
            for (var i = 0; i < 100; i++)
            {
                var variant = client.GetVariant("ALL_A-1");
                dictionary[variant] = dictionary[variant] + 1;
            }
            for (var i = 0; i < 100; i++)
            {
                var variant = client.GetVariant("ALL_A-2");
                dictionary[variant] = dictionary[variant] + 1;
            }
            for (var i = 0; i < 100; i++)
            {
                var variant = client.GetVariant("ALL_A-3");
                dictionary[variant] = dictionary[variant] + 1;
            }
            Assert.AreEqual(300, dictionary['A']);
            Assert.AreEqual(0, dictionary['B']);
            Assert.AreEqual(0, dictionary['X']);
            Assert.AreEqual(0, dictionary['Z']);
        }

        [Test]
        public void TestFeatureWithOverrideExperimentsToB()
        {
            var dictionary = new Dictionary<char, int> {['A'] = 0, ['B'] = 0, ['X'] = 0, ['Z'] = 0};
            var client = new FeatureClient(_featureWorker, _userDataRepo);
            for (var i = 0; i < 100; i++)
            {
                var variant = client.GetVariant("ALL_B-1");
                dictionary[variant] = dictionary[variant] + 1;
            }
            for (var i = 0; i < 100; i++)
            {
                var variant = client.GetVariant("ALL_B-2");
                dictionary[variant] = dictionary[variant] + 1;
            }
            for (var i = 0; i < 100; i++)
            {
                var variant = client.GetVariant("ALL_B-3");
                dictionary[variant] = dictionary[variant] + 1;
            }
            Assert.AreEqual(300, dictionary['B']);
            Assert.AreEqual(0, dictionary['A']);
            Assert.AreEqual(0, dictionary['X']);
            Assert.AreEqual(0, dictionary['Z']);
        }
    }
}
