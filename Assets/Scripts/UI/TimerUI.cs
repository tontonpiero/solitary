using Solitary.Manager;
using System;
using TMPro;
using UnityEngine;

namespace Solitary.UI
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TMP_Text textTimer;

        private float previousTime = 0f;

        private void Start()
        {
            UpdateTimerText(0f);
        }

        private void Update()
        {
            float totalTime = gameManager.GetTotalTime();
            if (Mathf.FloorToInt(previousTime) != Mathf.FloorToInt(totalTime))
            {
                UpdateTimerText(totalTime);
            }
            previousTime = gameManager.GetTotalTime();
        }

        private void UpdateTimerText(float totalTime)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalTime);
            textTimer.text = time.ToString(@"mm\:ss");
        }
    }
}
