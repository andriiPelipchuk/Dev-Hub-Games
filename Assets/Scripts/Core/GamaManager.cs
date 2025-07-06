using Assets.Scripts.Gameplay.Bubble;
using Assets.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core 
{
    public class GamaManager : MonoBehaviour
    {
        [SerializeField] private BubbleGrid _bubbleGrid;
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private UIManager _UI;
        private void OnEnable()
        {
            GameEvents.OnBubblePopped += HandleBubblePopped;
            GameEvents.OnGameOver += EndGame;
        }

        private void OnDisable()
        {
            GameEvents.OnBubblePopped -= HandleBubblePopped;
            GameEvents.OnGameOver -= EndGame;
        }
        private void HandleBubblePopped()
        {
            _scoreManager.AddScore();
        }

        private void EndGame()
        {
            Time.timeScale = 0f;
            _UI.ShowGameOverPanel();
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

