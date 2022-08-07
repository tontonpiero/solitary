using UnityEngine;

namespace Solitary
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private SettingsPopup settingsPopup;

        public void ShowSettingsPopup()
        {
            settingsPopup.Show();
        }
    }
}
