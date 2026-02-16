using CubeCollector.Core;
using UnityEngine;

namespace CubeCollector
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private ScoreTracker _scoreTracker;

        public event System.Action<int> OnScoreChanged
        {
            add => _scoreTracker.OnScoreChanged += value;
            remove => _scoreTracker.OnScoreChanged -= value;
        }

        public int Collected => _scoreTracker.Collected;
        public int Total => _scoreTracker.Total;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("[GameManager] Duplicate GameManager detected — destroying this instance.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _scoreTracker = new ScoreTracker(5);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void CollectPickup(Pickup pickup)
        {
            _scoreTracker.AddPoint();
            Destroy(pickup.gameObject);
        }
    }
}
