using Solitary.Core;
using Solitary.Manager;
using System;
using TMPro;
using UnityEngine;

namespace Solitary.UI
{
    public class PauseGameUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] GameManager gameManager;

        private void Awake()
        {
            Hide();
            gameManager.OnStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Paused)
            {
                Show();
            }
            else if( newState == GameState.Started )
            {
                Hide();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnClickNewGame()
        {
            gameManager.StartNewGame();
        }

        public void OnClickResumeGame()
        {
            gameManager.ResumeGame();
        }
    }
}
