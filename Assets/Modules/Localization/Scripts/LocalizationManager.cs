using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Globalization;
using System.Linq;
using System.IO;

namespace Modules.Localization
{
    public enum LocalizeDefaultBehaviour
    {
        UseLocalizeKey,
        EmptyString,
        CustomString
    }

    public class LocalizationManager
    {
        private static string configPath = "Localization/LocalizationConfig";
        private const string LanguagePrefKey = "_saved_language_";

        private List<Language> languages = null;
        private Language currentLanguage = null;
        private int loadedFilesVersion = -1;

        public LocalizationConfig Config = null;

        static public event Action<SystemLanguage> OnSystemLanguageChanged;
        public event Action OnLanguageChanged;

        private static LocalizationManager instance;

        public static LocalizationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LocalizationManager();
                }
                return instance;
            }
        }

        protected LocalizationManager()
        {
            Reset();
        }

        public void Reset()
        {
            // load config
            Config = null;
            LoadConfig();

            // set current language as device language / stored language
            SetDefaultLanguage();
        }

        private void SetDefaultLanguage()
        {
            if (Application.isPlaying && languages != null && languages.Count > 0)
            {
                int languageId = PlayerPrefs.GetInt(LanguagePrefKey, 0);
                if (languageId > 0)
                {
                    if (SetCurrentLanguage(languageId)) return;
                }
            }
            SetCurrentLanguage(GetDefaultLanguage());
        }

        virtual protected void LoadConfig()
        {
            if (Config == null && !string.IsNullOrEmpty(configPath))
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    LocalizationConfig originalConfig = Resources.Load<LocalizationConfig>(configPath);
                    if (originalConfig != null)
                    {
                        // clone original config so we can't modify the original at runtime
                        Config = UnityEngine.Object.Instantiate(originalConfig) as LocalizationConfig;
                    }
                }
                else
                {
                    Config = Resources.Load<LocalizationConfig>(configPath);
                }
#else
                Config = Resources.Load<LocalizationConfig>(configPath);
#endif
            }

            if (Config == null)
            {
                Config = ScriptableObject.CreateInstance<LocalizationConfig>();
                Config.Languages.Add(new Language(SystemLanguage.English));
            }

            languages = new List<Language>();
            foreach (Language language in Config.Languages)
            {
                AddLanguage(language);
            }
        }

        public virtual Language GetDeviceLanguage()
        {
            return GetLanguage(GetDeviceLanguageName(), GetDeviceCountryCode());
        }

        public virtual string GetDeviceLocale()
        {
            return LanguageHelper.GetLocale(GetDeviceLanguageName(), GetDeviceCountryCode());
        }

        public virtual string GetDeviceCountryCode()
        {
#if UNITY_EDITOR
            return Config.EditorCountry;
#else
            return PreciseLocale.GetRegion();
#endif
        }

        protected virtual SystemLanguage GetDeviceLanguageName()
        {
#if UNITY_EDITOR
            return Config.EditorLanguage;
#else
            return Application.systemLanguage;
#endif
        }

        public void AddLanguage(Language language)
        {
            if (languages == null) languages = new List<Language>();
            if (languages.Find(l => l.LanguageId == language.LanguageId) == null)
            {
                //if (Config.DEBUG && Application.isPlaying) Debug.LogFormat("LocalizationManager - AddLanguage() name={0}", language.GetLocale());
                languages.Add(language);
                language.Reset();
                //language.LoadTextFile(false);
            }
        }

        public void RemoveLanguage(int languageId)
        {
            if (languages == null) return;
            RemoveLanguage(languages.Find(l => l.LanguageId == languageId));
        }

        public void RemoveLanguage(Language language)
        {
            if (languages != null && languages.Contains(language))
            {
                //if (Config.DEBUG) Debug.LogFormat("LocalizationManager - RemoveLanguage() name={0}", language.GetLocale());
                languages.Remove(language);
                language.Unload();

                if (currentLanguage == language)
                {
                    SetDefaultLanguage();
                }
            }
        }

        public bool SetCurrentLanguage(int languageId)
        {
            if (languages == null) return false;
            return SetCurrentLanguage(languages.Find(l => l.LanguageId == languageId));
        }

        protected bool SetCurrentLanguage(Language language)
        {
            if (language != null && language != currentLanguage)
            {
                if (Application.isPlaying && Config.VerboseLogging) Debug.LogFormat("LocalizationManager - SetCurrentLanguage() name={0}", language.GetLocale());
                currentLanguage = language;
                PlayerPrefs.SetInt(LanguagePrefKey, language.LanguageId);
                DispatchLanguageChanged();
                return true;
            }
            return false;
        }

        public Language GetDefaultLanguage()
        {
            Language language;

            // try to get device language first
            language = GetDeviceLanguage();
            if (language != null) return language;

            // fallback to default language
            return languages != null && languages.Count > 0 ? languages[0] : null;
        }

        public Language GetCurrentLanguage()
        {
            return currentLanguage;
        }

        public Language GetLanguage(SystemLanguage languageName, string countryCode = null)
        {
            if (languages == null || languages.Count == 0) return null;
            Language language = languages.Find(l => l.LanguageName == languageName && l.CountryCode == countryCode);
            if (language == null)
            {
                language = languages.Find(l => l.LanguageName == languageName);
            }
            return language;
        }

        public CultureInfo GetCultureInfo()
        {
            if (currentLanguage != null)
                return currentLanguage.CultureInfo;
            return CultureInfo.InvariantCulture;
        }

        public CultureInfo GetCultureInfoFromCurrency(string currencyCode)
        {
            currencyCode = currencyCode.ToUpper();
            CultureInfo cultureInfo = LocalizationManager.Instance.GetCultureInfo();
            RegionInfo regionInfo = null;
            try
            {
                regionInfo = new RegionInfo(cultureInfo.LCID);
            }
            catch (Exception)
            {
                Debug.LogWarning("LocalizationManager - GetCultureInfoFromCurrency : Create RegionInfo throw an error on cultureInfo.LCID=" + cultureInfo.LCID);
            }

            if (regionInfo == null || regionInfo.ISOCurrencySymbol.ToUpper() != currencyCode)
            {
                cultureInfo = (from c in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                               let r = new RegionInfo(c.LCID)
                               where r != null
                               && r.ISOCurrencySymbol.ToUpper() == currencyCode
                               select c).FirstOrDefault();
            }

            return cultureInfo;
        }

        public void LoadExternalFiles(bool overrideValues = true)
        {
            if (Config.ExternalFilesVersion != loadedFilesVersion)
            {
                if (!string.IsNullOrWhiteSpace(Config.ExternalFilesUrl) && Config.ExternalFilesUrl.StartsWith("http", StringComparison.InvariantCulture))
                {
                    foreach (Language language in languages)
                    {
                        string url = string.Format(Config.ExternalFilesUrl, language.GetFileName(), Config.ExternalFilesVersion);
                        language.LoadExternalFile(url, overrideValues);
                    }
                    loadedFilesVersion = Config.ExternalFilesVersion;
                }
            }
        }

        public void DispatchLanguageChanged()
        {
            OnLanguageChanged?.Invoke();
            OnSystemLanguageChanged?.Invoke(GetCurrentLanguage().LanguageName);
        }

        public string Localize(string key, string pluralKey, object[] parameters = null)
        {
            return Localize(key, pluralKey, currentLanguage, parameters);
        }

        public string Localize(string key, string pluralKey, Language targetLanguage, object[] parameters = null)
        {
            if (!string.IsNullOrEmpty(pluralKey) && parameters != null && parameters.Length > 0)
            {
                // try to find an Integer parameter to determinate if we need to use the plural key
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (int.TryParse(parameters[i].ToString(), out int n))
                    {
                        if (n > 1) key = pluralKey;
                        break;
                    }
                }
            }

            return Localize(key, targetLanguage, parameters);
        }

        public string Localize(string key, object[] parameters = null)
        {
            return Localize(key, currentLanguage, parameters);
        }

        public string Localize(string key, Language targetLanguage, object[] parameters = null)
        {
#if UNITY_EDITOR
            if (Config.DebugTextsInEditor) return key != null ? $"[{key}]" : "NULL KEY";
#endif
            if (string.IsNullOrEmpty(key) || targetLanguage == null) return GetDefaultTranslationValue(key);

            // get current language value
            string translation = targetLanguage.GetTranslation(key);

            // fallback to backup language
            if (translation == null && languages != null && languages.Count > 0 && targetLanguage.CountryCode != null && targetLanguage.CountryCode != "All")
            {
                Language backupLanguage = GetLanguage(targetLanguage.LanguageName);
                if (backupLanguage != null && backupLanguage != targetLanguage)
                {
                    translation = backupLanguage.GetTranslation(key);
                }
            }

            // fallback to default languages
            if (translation == null && languages != null && languages.Count > 0 && languages[0] != targetLanguage)
            {
                translation = languages[0].GetTranslation(key);
            }

            // default behaviour
            if (translation == null)
            {
                translation = GetDefaultTranslationValue(key);
            }

            if (parameters == null || parameters.Length == 0)
            {
                return translation;
            }

            try
            {
                translation = string.Format(translation, parameters);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return translation;
        }

        private string GetDefaultTranslationValue(string key)
        {
            switch (Config.DefaultBehaviour)
            {
                case LocalizeDefaultBehaviour.EmptyString: return string.Empty;
                case LocalizeDefaultBehaviour.CustomString: return Config.CustomDefaultString;
                default: return key ?? string.Empty;
            }
        }

        public bool HasLocalization(string key, Language targetLanguage = null)
        {
            if (string.IsNullOrEmpty(key)) return false;
            if (targetLanguage == null) targetLanguage = currentLanguage;
            if (targetLanguage == null) return false;

            return targetLanguage.GetTranslation(key) != null;
        }

        public List<Language> GetLanguages()
        {
            return languages;
        }

        public void Refresh()
        {
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            Reset();
        }

#if UNITY_EDITOR
        public void EditConfig()
        {
            LocalizationConfig config = Resources.Load<LocalizationConfig>(configPath);

            if (config == null)
            {
                string defaultResourcesPath = Path.Combine("Assets", "Resources", "Localization");
                Directory.CreateDirectory(defaultResourcesPath);

                config = ScriptableObject.CreateInstance<LocalizationConfig>();
                AssetDatabase.CreateAsset(config, Path.Combine(defaultResourcesPath, "LocalizationConfig.asset"));
            }

            Selection.activeObject = config;
        }
#endif
    }
}