using UnityEngine;
using TMPro;

namespace Assets.Scripts.Core
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        private int _point = 10;
        private int _score;

        public void AddScore()
        {
            _score += _point;
            
            _scoreText.text = _score.ToString();
        }

    }
}