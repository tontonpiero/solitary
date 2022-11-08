using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Modules.Localization
{
    [Serializable]
    public struct LocalizationDataItem
    {
        public string Key;
        public List<TranslationData> Translations;

        public string GetTranslation(int languageId)
        {
            if (Translations == null) Translations = new List<TranslationData>();
            return Translations.Find(t => t.Lang == languageId).Value;
        }

        public void SetTranslation(int languageId, string value)
        {
            if (Translations == null) Translations = new List<TranslationData>();
            TranslationData translation = Translations.Find(t => t.Lang == languageId);
            int index = Translations.IndexOf(translation);
            if (index >= 0)
            {
                Translations[index] = new TranslationData() { Lang = languageId, Value = value };
            }
            else
            {
                Translations.Add(new TranslationData() { Lang = languageId, Value = value });
            }
        }
    }

    [Serializable]
    public struct TranslationData
    {
        public int Lang;
        public string Value;
    }

    public class LocalizationData : ScriptableObject
    {
        public List<LocalizationDataItem> Translations;

        static public LocalizationData GetAsset()
        {
            LocalizationData asset = Resources.Load<LocalizationData>("Localization/LocalizationData");
#if UNITY_EDITOR
            if (asset == null) asset = CreateAsset();
#endif
            return asset;
        }

#if UNITY_EDITOR
        static private LocalizationData CreateAsset()
        {
            string folder = AssetDatabase.GetAssetPath(LocalizationManager.Instance.Config);
            folder = Path.GetDirectoryName(folder);
            string path = Path.Combine(folder, "LocalizationData.asset");
            LocalizationData asset = CreateInstance<LocalizationData>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
#endif

        public void SetTranslation(Language language, string key, string value)
        {
            if (language == null || language.LanguageId == 0) return;
            if (string.IsNullOrEmpty(key)) return;
            if (value == null) value = "";
            if (Translations == null) Translations = new List<LocalizationDataItem>();
            LocalizationDataItem item = GetItem(key);
            if (item.Key != null)
            {
                int index = Translations.IndexOf(item);
                if (index >= 0) Translations[index].SetTranslation(language.LanguageId, value);
            }
            else
            {
                item = new LocalizationDataItem() { Key = key };
                item.SetTranslation(language.LanguageId, value);
                Translations.Add(item);
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void RemoveTranslation(string key)
        {
            if (Translations == null) return;
            Translations.Remove(GetItem(key));
        }

        public LocalizationDataItem GetItem(string key)
        {
            if (Translations == null) return default;
            return Translations.Find(t => t.Key == key);
        }

        public string GetTranslation(Language language, string key)
        {
            if (language == null || language.LanguageId == 0) return null;
            return GetItem(key).GetTranslation(language.LanguageId);
        }

        public void RenameKey(string oldKey, string newKey)
        {
            if (string.IsNullOrEmpty(oldKey)) return;
            if (string.IsNullOrEmpty(newKey)) return;

            LocalizationDataItem item = GetItem(oldKey);
            if (item.Key != null)
            {
                int index = Translations.IndexOf(item);
                if (index >= 0)
                {
                    item.Key = newKey;
                    Translations[index] = item;
                }
            }
        }

        public Dictionary<string, string> GetTranslations(Language language)
        {
            if (language == null || language.LanguageId == 0) return null;
            if (Translations == null) Translations = new List<LocalizationDataItem>();
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (LocalizationDataItem item in Translations)
            {
                if (!result.ContainsKey(item.Key))
                    result.Add(item.Key, item.GetTranslation(language.LanguageId));
            }
            return result;
        }
    }

}