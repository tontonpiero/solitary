using Modules.Localization.MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Modules.Localization
{
    [Serializable]
    public class Language
    {
        public int LanguageId;
        public SystemLanguage LanguageName;
        public string LanguageCode;
        public string CountryCode;

        private LocalizationData data;
        private Dictionary<string, string> translations;

        private CultureInfo cultureInfo = null;
        public CultureInfo CultureInfo
        {
            get
            {
                if (cultureInfo == null)
                    cultureInfo = CultureInfo.CreateSpecificCulture(GetLocale());
                return cultureInfo;
            }
        }

        public Language()
        {
            LanguageName = SystemLanguage.English;
            LanguageId = LanguageHelper.GetLanguageId(LanguageName);
            LanguageCode = LanguageHelper.GetLanguageCode(LanguageName);
            CountryCode = null;
        }

        public Language(SystemLanguage languageName)
        {
            LanguageName = languageName;
            LanguageId = LanguageHelper.GetLanguageId(languageName);
            LanguageCode = LanguageHelper.GetLanguageCode(languageName);
            CountryCode = null;
        }

        public void Reset()
        {
            data = null;
            translations = null;
        }

        virtual protected LocalizationData GetData()
        {
            if (data == null)
            {
                data = LocalizationData.GetAsset();
            }
            return data;
        }

        public string GetFileName()
        {
            return string.Format("{0}{1}", LanguageName, !string.IsNullOrEmpty(CountryCode) && CountryCode != "All" ? CountryCode : "");
        }

        public string GetLocale()
        {
            return LanguageHelper.GetLocale(LanguageCode, CountryCode);
        }

        public void LoadTextFile(bool overrideValues = true)
        {
            TextAsset asset = Resources.Load<TextAsset>(string.Format("Localization/{0}", GetFileName()));
            if (asset != null)
            {
                AddTranslations(asset.text, overrideValues);
            }
        }

        public virtual async void LoadExternalFile(string url, bool overrideValues = true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var handler = request.SendWebRequest();
                while (!handler.isDone) { await Task.Yield(); }
                if (handler.webRequest.result == UnityWebRequest.Result.Success)
                {
                    AddTranslations(handler.webRequest.downloadHandler.text, overrideValues);

                    if (LocalizationManager.Instance.GetCurrentLanguage().LanguageId == LanguageId)
                    {
                        LocalizationManager.Instance.DispatchLanguageChanged();
                    }
                }
                else
                {
                    Debug.LogWarning($"Language - LoadExternalFile() locale={GetLocale()} error={handler.webRequest.error}");
                }
            }
        }

        public void AddTranslations(string jsonValues, bool overrideValues = true)
        {
            if (!string.IsNullOrEmpty(jsonValues))
            {
                Dictionary<string, string> newEntries = (Json.Deserialize(jsonValues) as Dictionary<string, object>)
                    .ToDictionary(k => k.Key, k => k.Value.ToString());
                AddTranslations(newEntries, overrideValues);
            }
        }

        public void AddTranslations(Dictionary<string, string> newEntries, bool overrideValues = true)
        {
            if (newEntries != null && newEntries.Count > 0)
            {
                foreach (string key in newEntries.Keys)
                {
                    SetTranslation(key, newEntries[key], overrideValues);
                }
            }
        }

        public void SetTranslation(string key, string value, bool overrideValue = true)
        {
            if (!overrideValue && GetTranslation(key) != null) return;

            // Set in memory
            if (GetTranslations().ContainsKey(key)) GetTranslations()[key] = value;
            else GetTranslations().Add(key, value);

            // Store data
            if (!Application.isPlaying) GetData().SetTranslation(this, key, value);
        }

        public void RemoveTranslation(string key)
        {
            // Remove in memory
            if (GetTranslations().ContainsKey(key)) GetTranslations().Remove(key);

            // Remove stored data
            if (!Application.isPlaying) GetData().RemoveTranslation(key);
        }

        public string GetTranslation(string key)
        {
            if (Application.isPlaying)
            {
                // Get from memory
                return GetTranslations().ContainsKey(key) ? GetTranslations()[key] : null;
            }
            return GetData().GetTranslation(this, key);
        }

        public void RenameKey(string oldKey, string newKey)
        {
            if (GetTranslations().ContainsKey(oldKey) && !GetTranslations().ContainsKey(newKey))
            {
                string value = GetTranslations()[oldKey];
                GetTranslations().Add(newKey, value);
                GetTranslations().Remove(oldKey);
            }

            if (!Application.isPlaying) GetData().RenameKey(oldKey, newKey);
        }

        public Dictionary<string, string> GetTranslations()
        {
            if (!Application.isPlaying) return GetData().GetTranslations(this);
            if (translations == null) translations = GetData().GetTranslations(this);
            if (translations == null) translations = new Dictionary<string, string>();
            return translations;
        }

        public void Unload()
        {
            translations = null;
        }
    }
}
