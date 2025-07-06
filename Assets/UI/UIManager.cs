using System;
using UnityEngine;

namespace Assets.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _gameOverPanel;
        public void ShowGameOverPanel()
        {
            _gameOverPanel.SetActive(true);
        }
    }
}

