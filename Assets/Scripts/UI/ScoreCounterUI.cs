using Solitary.Manager;
using TMPro;
using UnityEngine;

namespace Solitary.UI
{
    public class ScoreCounterUI : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TMP_Text textCounter;

        private float currentScore = 0f;
        private int targetScore = 0;
        private float velocity = 0f;

        private void Start()
        {
            gameManager.OnScoreChanged += OnGameScoreChanged;
            targetScore = gameManager.GetScore();
            currentScore = gameManager.GetScore();
        }

        private void OnGameScoreChanged()
        {
            targetScore = gameManager.GetScore();
        }

        private void Update()
        {
            if (currentScore < targetScore)
            {
                currentScore = Mathf.SmoothDamp(currentScore, targetScore, ref velocity, 0.1f);
                if (currentScore > targetScore) currentScore = targetScore;
                UpdateCounterText();
            }
            else if (currentScore > targetScore)
            {
                currentScore = targetScore;
                UpdateCounterText();
            }
        }

        private void UpdateCounterText()
        {
            textCounter.text = Mathf.RoundToInt(currentScore).ToString();
        }
    }
}
