using System.Collections.Generic;
using UnityEngine;

namespace Modules.Localization
{
    [CreateAssetMenu(fileName = "LocalizationConfig", menuName = "Modules/Localization/Config File")]
    public class LocalizationConfig : ScriptableObject
    {
        public List<Language> Languages = new List<Language>();

        public SystemLanguage EditorLanguage = SystemLanguage.Unknown;
        public string EditorCountry = null;
        [Tooltip("e.g. https://your-storage-server.com/{0}.txt?v={1}\nWhere {0} is the language name and {1} is the version")]
        public string ExternalFilesUrl = "";
        public int ExternalFilesVersion = 0;
        public LocalizeDefaultBehaviour DefaultBehaviour = LocalizeDefaultBehaviour.UseLocalizeKey;
        public string CustomDefaultString = "<missing translation>";
        public bool VerboseLogging = true;
        public bool DebugTextsInEditor = false;

        private void OnEnable()
        {
            if (EditorLanguage == SystemLanguage.Unknown)
            {
                EditorLanguage = Application.systemLanguage;
            }

            if (string.IsNullOrEmpty(EditorCountry))
            {
                EditorCountry = PreciseLocale.GetRegion();
            }
        }
    }
}
