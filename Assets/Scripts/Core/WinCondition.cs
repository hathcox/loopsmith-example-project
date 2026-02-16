using System;

namespace CubeCollector.Core
{
    public class WinCondition
    {
        private readonly int _totalCount;

        public WinCondition(int totalCount)
        {
            if (totalCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count must be greater than zero.");

            _totalCount = totalCount;
        }

        public bool CheckWin(int collectedCount)
        {
            return collectedCount >= _totalCount;
        }
    }
}
