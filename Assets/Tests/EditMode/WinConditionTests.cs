using System;
using NUnit.Framework;
using CubeCollector.Core;

namespace CubeCollector.Tests
{
    [TestFixture]
    public class WinConditionTests
    {
        [Test]
        public void CheckWin_CollectedLessThanTotal_ReturnsFalse()
        {
            var winCondition = new WinCondition(5);

            var result = winCondition.CheckWin(3);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckWin_CollectedEqualsTotal_ReturnsTrue()
        {
            var winCondition = new WinCondition(5);

            var result = winCondition.CheckWin(5);

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckWin_CollectedZero_ReturnsFalse()
        {
            var winCondition = new WinCondition(5);

            var result = winCondition.CheckWin(0);

            Assert.IsFalse(result);
        }

        [Test]
        public void CheckWin_CollectedGreaterThanTotal_ReturnsTrue()
        {
            var winCondition = new WinCondition(5);

            var result = winCondition.CheckWin(7);

            Assert.IsTrue(result);
        }

        [Test]
        public void Constructor_ZeroTotalCount_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WinCondition(0));
        }

        [Test]
        public void Constructor_NegativeTotalCount_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WinCondition(-1));
        }

        [Test]
        public void CheckWin_WithScoreTracker_WinConditionEvaluatesCorrectly()
        {
            var scoreTracker = new ScoreTracker(5);
            var winCondition = new WinCondition(scoreTracker.Total);

            for (int i = 0; i < 4; i++)
            {
                scoreTracker.AddPoint();
                Assert.IsFalse(winCondition.CheckWin(scoreTracker.Collected));
            }

            scoreTracker.AddPoint();
            Assert.IsTrue(winCondition.CheckWin(scoreTracker.Collected));
        }
    }
}
