using System;

namespace CubeCollector.Core
{
    public class ScoreTracker
    {
        public int Collected { get; private set; }
        public int Total { get; }
        public event Action<int> OnScoreChanged;

        public ScoreTracker(int totalCount)
        {
            if (totalCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count must be greater than zero.");

            Total = totalCount;
        }

        public void AddPoint()
        {
            if (Collected >= Total)
                return;

            Collected++;
            OnScoreChanged?.Invoke(Collected);
        }
    }
}
