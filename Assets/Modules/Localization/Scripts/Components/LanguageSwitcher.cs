using UnityEngine;

namespace Modules.Localization
{
    [AddComponentMenu("Modules/Localization/LanguageSwitcher")]
    public class LanguageSwitcher : MonoBehaviour
    {
        public void SetLanguageById(int languageId)
        {
            LocalizationManager.Instance.SetCurrentLanguage(languageId);
        }
    }
}