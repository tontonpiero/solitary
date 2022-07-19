using Solitary.Manager;
using TMPro;
using UnityEngine;

namespace Solitary.UI
{
    public class ScoreCounterUI : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TMP_Text textCounter;

        private void Start()
        {
            gameManager.OnScoreChanged += OnGameScoreChanged;
            UpdateCounterText();
        }

        private void OnGameScoreChanged()
        {
            UpdateCounterText();
        }

        private void UpdateCounterText()
        {
            textCounter.text = gameManager.GetScore().ToString();
        }
    }
}
