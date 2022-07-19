using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Solitary
{
    public class ButtonSound : MonoBehaviour
    {
        [SerializeField] private string soundName = "click";

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (!string.IsNullOrEmpty(soundName))
            {
                AudioManager.Instance.PlaySound(soundName);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }
            button = null;
        }
    }
}
