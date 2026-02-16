using TMPro;
using UnityEngine;

namespace CubeCollector
{
    public class UIManager : MonoBehaviour
    {
        private TextMeshProUGUI _scoreText;
        private GameObject _winMessageObject;
        private GameManager _gameManager;
        private bool _isInitialized;

        public void Initialize(TextMeshProUGUI scoreText, TextMeshProUGUI winText, GameManager gameManager)
        {
            _scoreText = scoreText;
            _winMessageObject = winText.gameObject;
            _gameManager = gameManager;
            _isInitialized = true;

            _scoreText.text = $"0/{_gameManager.Total}";
            _winMessageObject.SetActive(false);

            Subscribe();
        }

        private void OnEnable()
        {
            if (!_isInitialized) return;
            Subscribe();
        }

        private void OnDisable()
        {
            if (!_isInitialized) return;
            Unsubscribe();
        }

        private void Subscribe()
        {
            _gameManager.OnScoreChanged += UpdateScore;
            _gameManager.OnWinCondition += ShowWinMessage;
        }

        private void Unsubscribe()
        {
            _gameManager.OnScoreChanged -= UpdateScore;
            _gameManager.OnWinCondition -= ShowWinMessage;
        }

        private void UpdateScore(int collected)
        {
            if (_scoreText == null) return;
            _scoreText.text = $"{collected}/{_gameManager.Total}";
        }

        private void ShowWinMessage()
        {
            if (_winMessageObject == null) return;
            _winMessageObject.SetActive(true);
        }
    }
}
