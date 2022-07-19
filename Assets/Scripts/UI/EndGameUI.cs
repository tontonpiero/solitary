using Solitary.Core;
using Solitary.Manager;
using System;
using TMPro;
using UnityEngine;

namespace Solitary.UI
{
    public class EndGameUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] GameManager gameManager;

        [Header("Stats")]
        [SerializeField] private TMP_Text textMoves;
        [SerializeField] private TMP_Text textTime;
        [SerializeField] private TMP_Text textScore;

        private Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            Hide();
            gameManager.OnStateChanged += OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Over)
            {
                Show();
            }
        }

        public void Show()
        {
            canvas.enabled = true;

            textMoves.text = gameManager.GetMoves().ToString();
            TimeSpan time = TimeSpan.FromSeconds(gameManager.GetTotalTime());
            textTime.text = $"{time.TotalMinutes:00}:{time.TotalSeconds:00}";
            textScore.text = gameManager.GetScore().ToString();
        }

        public void Hide()
        {
            canvas.enabled = false;
        }

        public void OnClickNewGame()
        {

        }
    }
}
