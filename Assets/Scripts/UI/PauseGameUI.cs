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

        private Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
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
            canvas.enabled = true;
        }

        public void Hide()
        {
            canvas.enabled = false;
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
