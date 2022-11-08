using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Modules.Localization.Editor
{
    [CustomEditor(typeof(LocalizationConfig))]
    public class LocalizationConfigEditor : UnityEditor.Editor
    {
        private ReorderableList languageList;
        private SerializedProperty VerboseLoggingProperty;
        private SerializedProperty EditorLanguageProperty;
        private SerializedProperty EditorCountryProperty;
        private SerializedProperty ExternalFilesVersionProperty;
        private SerializedProperty ExternalFilesUrlProperty;
        private SerializedProperty DefaultBehaviourProperty;
        private SerializedProperty CustomDefaultStringProperty;
        private SerializedProperty DebugTextsInEditorProperty;

        private LocalizationConfig localizationConfig { get { return target as LocalizationConfig; } }

        private void OnEnable()
        {
            VerboseLoggingProperty = serializedObject.FindProperty(nameof(LocalizationConfig.VerboseLogging));
            EditorLanguageProperty = serializedObject.FindProperty(nameof(LocalizationConfig.EditorLanguage));
            EditorCountryProperty = serializedObject.FindProperty(nameof(LocalizationConfig.EditorCountry));
            ExternalFilesVersionProperty = serializedObject.FindProperty(nameof(LocalizationConfig.ExternalFilesVersion));
            ExternalFilesUrlProperty = serializedObject.FindProperty(nameof(LocalizationConfig.ExternalFilesUrl));
            DefaultBehaviourProperty = serializedObject.FindProperty(nameof(LocalizationConfig.DefaultBehaviour));
            CustomDefaultStringProperty = serializedObject.FindProperty(nameof(LocalizationConfig.CustomDefaultString));
            DebugTextsInEditorProperty = serializedObject.FindProperty(nameof(LocalizationConfig.DebugTextsInEditor));

            languageList = new ReorderableList(localizationConfig.Languages, typeof(Language), true, true, true, true);

            languageList.drawHeaderCallback = DrawLanguagesHeader;
            languageList.drawElementCallback = DrawLanguageItem;
            languageList.onReorderCallback = (list) => Save();
        }

        private void DrawLanguagesHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Languages", EditorStyles.boldLabel);
        }

        private void DrawLanguageItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            Language language = localizationConfig.Languages[index];

            EditorGUI.BeginChangeCheck();
            //EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), language, GUIContent.none);
            float w = EditorGUIUtility.currentViewWidth - 40;

            SystemLanguage oldName = language.LanguageName;
            language.LanguageName = (SystemLanguage)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, (w / 3 - 5), EditorGUIUtility.singleLineHeight), language.LanguageName);
            string oldCountry = language.CountryCode;
            string[] allCountries = LanguageHelper.GetCountryCodes(language.LanguageName);
            int countryIndex = allCountries.ToList().IndexOf(language.CountryCode);
            if (countryIndex < 0) countryIndex = 0;
            countryIndex = EditorGUI.Popup(new Rect(rect.x + w / 3, rect.y, (w / 3 - 5), EditorGUIUtility.singleLineHeight), countryIndex, allCountries);
            language.CountryCode = allCountries[countryIndex];
            if (language.LanguageName != oldName || language.CountryCode != oldCountry)
            {
                language.LanguageCode = LanguageHelper.GetLanguageCode(language.LanguageName);
                language.LanguageId = LanguageHelper.GetLanguageId(language.LanguageName, language.CountryCode);
            }
            //language.ISOCode = EditorGUI.TextField(new Rect(rect.x + w / 3, rect.y, (w / 3 - 5), EditorGUIUtility.singleLineHeight), language.ISOCode);
            //language.CountryCode = EditorGUI.TextField(new Rect(rect.x + w / 3, rect.y, (w / 3 - 5), EditorGUIUtility.singleLineHeight), language.CountryCode);
            language.LanguageId = EditorGUI.IntField(new Rect(rect.x + 2 * w / 3, rect.y, (w / 3 - 5), EditorGUIUtility.singleLineHeight), language.LanguageId);
            if (EditorGUI.EndChangeCheck())
            {
                Save();
            }
        }

        private void Save()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            LocalizationManager.Instance.Refresh();
            Debug.Log("Saved... default language set to " + LocalizationManager.Instance.GetCurrentLanguage().LanguageName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (!AssetDatabase.GetAssetPath(serializedObject.targetObject).EndsWith("Resources/Localization/LocalizationConfig.asset"))
            {
                EditorGUILayout.HelpBox("Localization Config File must be placed in a 'Resources/Localization' folder and named 'LocalizationConfig.asset'", MessageType.Warning);
            }

            int editorLanguageIndex = EditorLanguageProperty.enumValueIndex;

            EditorGUILayout.LabelField("Editor locale", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(EditorLanguageProperty);
            EditorGUILayout.PropertyField(EditorCountryProperty);
            EditorGUILayout.Space();

            if (EditorLanguageProperty.enumValueIndex != editorLanguageIndex)
            {
                SystemLanguage editorLanguage = ((SystemLanguage)EditorLanguageProperty.enumValueIndex);
                Debug.LogFormat("Set Editor Language to {0}", editorLanguage);
                Language language = LocalizationManager.Instance.GetLanguage(editorLanguage);
                if (language == null) language = LocalizationManager.Instance.GetDefaultLanguage();
                if (language != null)
                {
                    LocalizationManager.Instance.SetCurrentLanguage(language.LanguageId);
                }
            }

            EditorCountryProperty.stringValue = EditorCountryProperty.stringValue.ToUpper();

            EditorGUILayout.LabelField("External files", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(ExternalFilesVersionProperty);
            EditorGUILayout.PropertyField(ExternalFilesUrlProperty);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("When no translation found...", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(DefaultBehaviourProperty);
            if (localizationConfig.DefaultBehaviour == LocalizeDefaultBehaviour.CustomString)
            {
                EditorGUILayout.PropertyField(CustomDefaultStringProperty);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(VerboseLoggingProperty);
            EditorGUILayout.PropertyField(DebugTextsInEditorProperty);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            languageList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void AddItem(ReorderableList list)
        {
            localizationConfig.Languages.Add(new Language());

            EditorUtility.SetDirty(target);
        }

        private void RemoveItem(ReorderableList list)
        {
            localizationConfig.Languages.RemoveAt(list.index);

            EditorUtility.SetDirty(target);
        }
    }
}
