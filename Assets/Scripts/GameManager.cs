using System;
using CubeCollector.Core;
using UnityEngine;

namespace CubeCollector
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private ScoreTracker _scoreTracker;
        private WinCondition _winCondition;

        public event Action<int> OnScoreChanged
        {
            add => _scoreTracker.OnScoreChanged += value;
            remove => _scoreTracker.OnScoreChanged -= value;
        }

        public event Action OnWinCondition;

        public int Collected => _scoreTracker.Collected;
        public int Total => _scoreTracker.Total;
        public bool IsGameWon { get; private set; }

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
            _winCondition = new WinCondition(_scoreTracker.Total);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void CollectPickup(Pickup pickup)
        {
            if (IsGameWon) return;

            _scoreTracker.AddPoint();
            Destroy(pickup.gameObject);

            if (_winCondition.CheckWin(_scoreTracker.Collected))
            {
                IsGameWon = true;
                OnWinCondition?.Invoke();
            }
        }
    }
}
