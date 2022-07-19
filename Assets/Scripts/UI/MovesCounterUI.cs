using Solitary.Manager;
using TMPro;
using UnityEngine;

namespace Solitary.UI
{
    public class MovesCounterUI : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TMP_Text textCounter;

        private void Start()
        {
            gameManager.OnMovesChanged += OnGameMovesChanged;
            UpdateCounterText();
        }

        private void OnGameMovesChanged()
        {
            UpdateCounterText();
        }

        private void UpdateCounterText()
        {
            textCounter.text = gameManager.GetMoves().ToString();
        }
    }
}
