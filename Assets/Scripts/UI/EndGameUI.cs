using Solitary.Core;
using Solitary.Manager;
using Solitary.Utils;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Audio;

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
        [SerializeField] private TMP_Text textBestScore;
        [SerializeField] private Image bestScoreIcon;

        private void Awake()
        {
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
            gameObject.SetActive(true);

            StatsManager statsManager = new StatsManager();
            statsManager.SaveScore(gameManager.GetScore());

            textMoves.text = gameManager.GetMoves().ToString();
            TimeSpan time = TimeSpan.FromSeconds(gameManager.GetTotalTime());
            textTime.text = time.ToString(@"mm\:ss");
            textScore.text = gameManager.GetScore().ToString();
            textBestScore.text = $"Best score: {statsManager.Data.BestScore}";

            bestScoreIcon.gameObject.SetActive(gameManager.GetScore() == statsManager.Data.BestScore);

            StartCoroutine(DelayPlayVictorySound());
        }

        private IEnumerator DelayPlayVictorySound()
        {
            yield return new WaitForSeconds(1f);
            AudioManager.PlaySound("victory");
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
