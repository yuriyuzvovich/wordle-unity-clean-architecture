using NUnit.Framework;
using Wordle.Application.DTOs;

namespace Wordle.Application.Tests.DTOs
{
    [TestFixture]
    public class StatisticsDTOTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesDTO()
        {
            var winDistribution = new int[] { 1, 2, 3, 4, 5, 6 };

            var dto = new StatisticsDTO(10, 8, 2, 3, 5, winDistribution);

            Assert.AreEqual(10, dto.TotalGamesPlayed);
            Assert.AreEqual(8, dto.TotalGamesWon);
            Assert.AreEqual(2, dto.TotalGamesLost);
            Assert.AreEqual(3, dto.CurrentStreak);
            Assert.AreEqual(5, dto.MaxStreak);
            Assert.AreEqual(6, dto.WinDistribution.Length);
            Assert.AreEqual(0.8f, dto.WinRate, 0.01f);
        }

        [Test]
        public void Constructor_ZeroGamesPlayed_WinRateIsZero()
        {
            var winDistribution = new int[6];

            var dto = new StatisticsDTO(0, 0, 0, 0, 0, winDistribution);

            Assert.AreEqual(0f, dto.WinRate);
        }

        [Test]
        public void Constructor_AllGamesWon_WinRateIsOne()
        {
            var winDistribution = new int[] { 5, 0, 0, 0, 0, 0 };

            var dto = new StatisticsDTO(5, 5, 0, 5, 5, winDistribution);

            Assert.AreEqual(1f, dto.WinRate, 0.01f);
        }

        [Test]
        public void Constructor_AllGamesLost_WinRateIsZero()
        {
            var winDistribution = new int[6];

            var dto = new StatisticsDTO(5, 0, 5, 0, 0, winDistribution);

            Assert.AreEqual(0f, dto.WinRate);
        }

        [Test]
        public void CreateEmpty_CreatesEmptyStatistics()
        {
            var dto = StatisticsDTO.CreateEmpty();

            Assert.AreEqual(0, dto.TotalGamesPlayed);
            Assert.AreEqual(0, dto.TotalGamesWon);
            Assert.AreEqual(0, dto.TotalGamesLost);
            Assert.AreEqual(0, dto.CurrentStreak);
            Assert.AreEqual(0, dto.MaxStreak);
            Assert.AreEqual(6, dto.WinDistribution.Length);
            Assert.AreEqual(0f, dto.WinRate);
        }

        [Test]
        public void CreateEmpty_WinDistributionIsAllZeros()
        {
            var dto = StatisticsDTO.CreateEmpty();

            foreach (var count in dto.WinDistribution)
            {
                Assert.AreEqual(0, count);
            }
        }

        [Test]
        public void Constructor_HalfWinRate_CalculatesCorrectly()
        {
            var winDistribution = new int[] { 1, 1, 1, 1, 1, 0 };

            var dto = new StatisticsDTO(10, 5, 5, 0, 3, winDistribution);

            Assert.AreEqual(0.5f, dto.WinRate, 0.01f);
        }

        [Test]
        public void Constructor_WinDistributionValues_ArePreserved()
        {
            var winDistribution = new int[] { 10, 20, 30, 15, 5, 2 };

            var dto = new StatisticsDTO(82, 82, 0, 10, 20, winDistribution);

            Assert.AreEqual(10, dto.WinDistribution[0]);
            Assert.AreEqual(20, dto.WinDistribution[1]);
            Assert.AreEqual(30, dto.WinDistribution[2]);
            Assert.AreEqual(15, dto.WinDistribution[3]);
            Assert.AreEqual(5, dto.WinDistribution[4]);
            Assert.AreEqual(2, dto.WinDistribution[5]);
        }
    }
}
