using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Localization
{
    [AddComponentMenu("Modules/Localization/LocalizeText")]
    [ExecuteInEditMode]
    public class LocalizeText : MonoBehaviour
    {
        [Header("Keys")]
        [SerializeField]
        protected string Key;
        [Tooltip("PluralKey is used if the first Interger parameter is > 1")]
        [SerializeField]
        protected string PluralKey;

        [SerializeField]
        protected List<LocalizeParameter> Parameters;

        [Header("Format")]
        public bool ToUpper = false;
        public bool FirstCharToUpper = false;

        [Header("Debug")]
        public bool VerboseLogging = false;

        private Text text;
        private TMP_Text textMesh;
        private bool needToUpdateText = true;

        public void CopyValues(LocalizeText localizeText)
        {
            SetKey(localizeText.GetKey());
            SetPluralKey(localizeText.GetPluralKey());
            SetParameters(localizeText.GetParameters());
        }

        public void SetKey(string key)
        {
            if (key != Key)
            {
                Key = key;
                needToUpdateText = true;
                Log($"LocalizeText - SetKey() {key}");
            }
        }

        public void SetKey(string key, string pluralKey)
        {
            if (key != Key || pluralKey != PluralKey)
            {
                Key = key;
                PluralKey = pluralKey;
                needToUpdateText = true;
                Log($"LocalizeText - SetKey() {key} plural={pluralKey}");
            }
        }

        public void SetPluralKey(string pluralKey)
        {
            if (pluralKey != PluralKey)
            {
                PluralKey = pluralKey;
                needToUpdateText = true;
                Log($"LocalizeText - SetPluralKey() {pluralKey}");
            }
        }

        public string GetKey() { return Key; }
        public string GetPluralKey() { return PluralKey; }

        public void SetParameters(List<LocalizeParameter> parameters)
        {
            Parameters = parameters;
            needToUpdateText = true;
        }

        public void SetParameters(List<string> parameters)
        {
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (i < Parameters.Count)
                    {
                        Parameters[i] = new LocalizeParameter(parameters[i], false);
                    }
                    else
                    {
                        Parameters.Add(new LocalizeParameter(parameters[i], false));
                    }
                }
                needToUpdateText = true;
            }
        }

        public void SetParameter(int index, object value, bool localize = false)
        {
            SetParameter(index, (value ?? "").ToString(), localize = false);
        }

        public void SetParameter(int index, string value, bool localize = false)
        {
            if (Parameters != null && index < Parameters.Count)
            {
                Parameters[index] = new LocalizeParameter(value, localize);
                needToUpdateText = true;
            }
        }

        public LocalizeParameter GetParameter(int index)
        {
            if (Parameters != null && Parameters.Count >= index)
            {
                return Parameters[index];
            }
            return null;
        }

        public List<LocalizeParameter> GetParameters()
        {
            return Parameters;
        }

        public int CountParameters()
        {
            return Parameters != null ? Parameters.Count : 0;
        }

        void Awake()
        {
            text = GetComponent<Text>();
            textMesh = GetComponent<TMP_Text>();
            needToUpdateText = true;
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
            UpdateText();
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            needToUpdateText = true;
            Log($"LocalizeText - OnLanguageChanged() lang={LocalizationManager.Instance.GetCurrentLanguage().LanguageCode}");
        }

        private void Update()
        {
            if (needToUpdateText || !Application.isPlaying) UpdateText();
        }

        public string GetLocalizedText()
        {
            string value = GetRawText();
            if (Application.isPlaying)
            {
                value = LocalizationManager.Instance.Localize(Key, PluralKey, Parameters?.ToArray());
            }
            else
            {
                if (LocalizationManager.Instance.HasLocalization(Key, LocalizationManager.Instance.GetDefaultLanguage()) || string.IsNullOrEmpty(value))
                {
                    value = LocalizationManager.Instance.Localize(Key, PluralKey, LocalizationManager.Instance.GetDefaultLanguage(), Parameters?.ToArray());
                }
            }

            if (ToUpper) value = value.ToUpper();
            if (FirstCharToUpper) value = value.FirstCharToUpper();

            return value;
        }

        private void UpdateText()
        {
            string value = GetLocalizedText();
            if (text != null) text.text = value;
            if (textMesh != null) textMesh.text = value;
            needToUpdateText = false;
            Log($"LocalizeText - UpdateText() key={Key} lang={LocalizationManager.Instance.GetCurrentLanguage().LanguageCode} text={value}");
        }

        public string GetRawText()
        {
            if (text != null) return text.text;
            if (textMesh != null) return textMesh.text;
            return string.Empty;
        }

        private void Log(string message)
        {
            if (Application.isPlaying && VerboseLogging && LocalizationManager.Instance.Config.VerboseLogging)
            {
                Debug.Log(message, gameObject);
            }
        }
    }
}