using RotaryHeart.Lib.AutoComplete;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Modules.Localization
{

    [CustomEditor(typeof(LocalizeText))]
    public class LocalizeTextEditor : UnityEditor.Editor
    {
        private SerializedProperty KeyProperty;
        private SerializedProperty PluralKeyProperty;
        private SerializedProperty ParametersProperty;
        private SerializedProperty ToUpperProperty;
        private SerializedProperty FirstCharToUpperProperty;
        private SerializedProperty VerboseLoggingProperty;
        private LocalizeText localizationText { get { return target as LocalizeText; } }

        private void OnEnable()
        {
            KeyProperty = serializedObject.FindProperty("Key");
            PluralKeyProperty = serializedObject.FindProperty("PluralKey");
            ParametersProperty = serializedObject.FindProperty("Parameters");
            ToUpperProperty = serializedObject.FindProperty(nameof(localizationText.ToUpper));
            FirstCharToUpperProperty = serializedObject.FindProperty(nameof(localizationText.FirstCharToUpper));
            VerboseLoggingProperty = serializedObject.FindProperty(nameof(localizationText.VerboseLogging));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Language defaultLanguage = LocalizationManager.Instance.GetDefaultLanguage();

            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = Color.yellow;
            EditorGUILayout.LabelField(localizationText.GetLocalizedText(), EditorStyles.helpBox);
            GUI.contentColor = Color.white;
            if (!LocalizationManager.Instance.HasLocalization(localizationText.GetKey(), defaultLanguage) && !string.IsNullOrWhiteSpace(localizationText.GetKey()))
            {
                if (GUILayout.Button(string.Format("Add translation ({0})", defaultLanguage.LanguageName), GUILayout.Width(180f)))
                {
                    defaultLanguage.SetTranslation(localizationText.GetKey(), localizationText.GetRawText(), true);
                }
            }
            else
            {
                if (GUILayout.Button("Edit", GUILayout.Width(180f)))
                {
                    LocalizationTableWindow.ShowWindow(localizationText.GetKey(), defaultLanguage);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUIStyle copyButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 40f
            };

            GUIStyle clearButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedWidth = 20f
            };

            GUILayout.Space(10f);

            string[] allKeys = LocalizationManager.Instance.GetCurrentLanguage().GetTranslations().Keys.ToArray();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Key");
            KeyProperty.stringValue = AutoCompleteTextField.EditorGUILayout.AutoCompleteTextField(KeyProperty.stringValue, allKeys, true);
            if (GUILayout.Button("Copy", copyButtonStyle)) EditorGUIUtility.systemCopyBuffer = KeyProperty.stringValue;
            if (GUILayout.Button("X", clearButtonStyle)) KeyProperty.stringValue = string.Empty;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Plural Key");
            PluralKeyProperty.stringValue = AutoCompleteTextField.EditorGUILayout.AutoCompleteTextField(PluralKeyProperty.stringValue, allKeys, true);
            if (GUILayout.Button("Copy", copyButtonStyle)) EditorGUIUtility.systemCopyBuffer = PluralKeyProperty.stringValue;
            if (GUILayout.Button("X", clearButtonStyle)) localizationText.SetPluralKey(string.Empty);
            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.PropertyField(KeyProperty);
            //EditorGUILayout.PropertyField(PluralKeyProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
            ShowParametersList(ParametersProperty);

            EditorGUILayout.PropertyField(ToUpperProperty);
            EditorGUILayout.PropertyField(FirstCharToUpperProperty);

            EditorGUILayout.PropertyField(VerboseLoggingProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowParametersList(SerializedProperty list)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty item = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                item.FindPropertyRelative("Value").stringValue = EditorGUILayout.TextField(item.FindPropertyRelative("Value").stringValue);
                EditorGUILayout.LabelField("Localize", GUILayout.Width(45f));
                item.FindPropertyRelative("Localize").boolValue = EditorGUILayout.Toggle(item.FindPropertyRelative("Localize").boolValue, GUILayout.Width(30f));
                if (GUILayout.Button("X", GUILayout.Width(25f), GUILayout.Height(15f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+", GUILayout.Width(40f)))
            {
                list.InsertArrayElementAtIndex(list.arraySize);
            }
        }
    }

}