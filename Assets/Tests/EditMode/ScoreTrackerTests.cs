using System;
using NUnit.Framework;
using CubeCollector.Core;

namespace CubeCollector.Tests
{
    [TestFixture]
    public class ScoreTrackerTests
    {
        [Test]
        public void Constructor_WhenInitialized_CollectedIsZero()
        {
            var tracker = new ScoreTracker(5);

            Assert.That(tracker.Collected, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithTotalFive_TotalEqualsFive()
        {
            var tracker = new ScoreTracker(5);

            Assert.That(tracker.Total, Is.EqualTo(5));
        }

        [Test]
        public void Constructor_WithZeroTotal_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScoreTracker(0));
        }

        [Test]
        public void Constructor_WithNegativeTotal_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ScoreTracker(-3));
        }

        [Test]
        public void AddPoint_WhenCalled_IncrementsCollectedCount()
        {
            var tracker = new ScoreTracker(5);

            tracker.AddPoint();

            Assert.That(tracker.Collected, Is.EqualTo(1));
        }

        [Test]
        public void AddPoint_WhenCalled_FiresOnScoreChangedEvent()
        {
            var tracker = new ScoreTracker(5);
            int receivedScore = -1;
            tracker.OnScoreChanged += score => receivedScore = score;

            tracker.AddPoint();

            Assert.That(receivedScore, Is.EqualTo(1));
        }

        [Test]
        public void AddPoint_CalledThreeTimes_CollectedEqualsThree()
        {
            var tracker = new ScoreTracker(5);

            tracker.AddPoint();
            tracker.AddPoint();
            tracker.AddPoint();

            Assert.That(tracker.Collected, Is.EqualTo(3));
        }

        [Test]
        public void AddPoint_CalledThreeTimes_EventFiredExactlyThreeTimes()
        {
            var tracker = new ScoreTracker(5);
            int eventCount = 0;
            tracker.OnScoreChanged += score => eventCount++;

            tracker.AddPoint();
            tracker.AddPoint();
            tracker.AddPoint();

            Assert.That(eventCount, Is.EqualTo(3));
        }

        [Test]
        public void AddPoint_WhenCollectedEqualsTotal_DoesNotIncrement()
        {
            var tracker = new ScoreTracker(2);
            tracker.AddPoint();
            tracker.AddPoint();

            tracker.AddPoint();

            Assert.That(tracker.Collected, Is.EqualTo(2));
        }

        [Test]
        public void AddPoint_WhenCollectedEqualsTotal_DoesNotFireEvent()
        {
            var tracker = new ScoreTracker(2);
            tracker.AddPoint();
            tracker.AddPoint();

            int eventCount = 0;
            tracker.OnScoreChanged += score => eventCount++;

            tracker.AddPoint();

            Assert.That(eventCount, Is.EqualTo(0));
        }
    }
}
