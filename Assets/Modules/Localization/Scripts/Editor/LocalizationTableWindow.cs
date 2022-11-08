using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace Modules.Localization
{

    public class LocalizationTableWindow : EditorWindow
    {
        private struct CellData
        {
            public bool MatchSearch;
            public Language Language;
            public string Translation;
        }

        private string keyToFocus = null;
        private Vector2 scrollPos;
        private string rawSearchFilter;
        private Dictionary<string, bool> selectionList;

        [MenuItem(itemName: "Window/Modules/Localization Table")]
        public static void ShowWindow()
        {
            LocalizationTableWindow window = (LocalizationTableWindow)EditorWindow.GetWindow(typeof(LocalizationTableWindow));
            window.Show();
        }

        public static void ShowWindow(string focusKey, Language focusLanguage = null)
        {
            LocalizationTableWindow window = (LocalizationTableWindow)EditorWindow.GetWindow(typeof(LocalizationTableWindow));
            window.Show();
            window.FocusOn(focusKey, focusLanguage);
        }

        private void FocusOn(string key, Language language = null)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (language != null) keyToFocus = string.Format("{0}_{1}", key, language.LanguageId);
            else keyToFocus = key;
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Localization Table");

            GUIStyle headerLineStyle = new GUIStyle(GUI.skin.box)
            {
            };

            GUIStyle lineStyle = new GUIStyle(GUI.skin.box)
            {
            };

            GUIStyle cellHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold
            };

            GUIStyle cellStyle = new GUIStyle(GUI.skin.label)
            {
            };

            GUIStyle editableCellStyle = new GUIStyle(GUI.skin.textField)
            {
            };

            GUIStyle searchHeaderCellStyle = new GUIStyle(GUI.skin.textField)
            {
            };

            GUIStyle searchCellStyle = new GUIStyle(cellStyle)
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState() { textColor = Color.yellow }
            };

            float cellWidth = 200f;
            GUILayoutOption cellWidthOption = GUILayout.Width(cellWidth);


            if (selectionList == null) selectionList = new Dictionary<string, bool>();
            Language defaultLanguage = LocalizationManager.Instance.GetDefaultLanguage();
            List<Language> languages = LocalizationManager.Instance.GetLanguages();
            languages.Sort((a, b) => (a == defaultLanguage ? -1 : 1));
            Dictionary<string, string> defaultTranslations = defaultLanguage.GetTranslations();

            // Search
            EditorGUILayout.BeginHorizontal(headerLineStyle);
            EditorGUILayout.PrefixLabel("Search");
            if (!string.IsNullOrEmpty(rawSearchFilter)) GUI.backgroundColor = Color.cyan;
            rawSearchFilter = EditorGUILayout.TextField(rawSearchFilter, searchHeaderCellStyle);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            // Add
            EditorGUILayout.BeginHorizontal(headerLineStyle);
            if (GUILayout.Button("Add new translation"))
            {
                string newKey = "new key";
                if (defaultLanguage.GetTranslation(newKey) == null)
                {
                    defaultLanguage.SetTranslation(newKey, "");
                    selectionList = null;
                }
                keyToFocus = newKey;
            }
            EditorGUILayout.EndHorizontal();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Header
            EditorGUILayout.BeginHorizontal(headerLineStyle);
            EditorGUILayout.LabelField("", GUILayout.Width(16f));
            GUILayout.Label("Key", cellWidthOption);
            foreach (Language language in languages)
            {
                string strDefault = language == defaultLanguage ? " (default)" : "";
                string name = string.Format("{0} ({1}){2}", language.LanguageName, language.CountryCode, strDefault);
                EditorGUILayout.LabelField(new GUIContent(name), cellHeaderStyle, cellWidthOption);

            }
            EditorGUILayout.EndHorizontal();


            // Translations
            if (defaultTranslations != null)
            {
                float viewTop = scrollPos.y - 30f;
                float viewBottom = viewTop + position.height - 50f;
                float lineHeight = EditorGUIUtility.singleLineHeight + 2f;

                string searchFilter = rawSearchFilter?.ToLower();
                List<CellData> cellList = new List<CellData>();
                int index = 0;
                foreach (string key in defaultTranslations.Keys)
                {
                    float lineTop = index * lineHeight;
                    float lineBottom = lineTop + lineHeight;
                    bool visible = lineTop <= viewBottom && lineBottom >= viewTop;

                    if (visible)
                    {
                        bool modified = false;
                        bool searchOk = string.IsNullOrEmpty(searchFilter);
                        cellList.Clear();
                        bool keySearchResult = !string.IsNullOrEmpty(searchFilter) && key.Contains(searchFilter);

                        if (!searchOk && keySearchResult) searchOk = true;

                        foreach (Language language in languages)
                        {
                            string t = language.GetTranslation(key);
                            bool searchRes = !string.IsNullOrEmpty(searchFilter) && t != null && t.ToLower().Contains(searchFilter);
                            cellList.Add(new CellData() { MatchSearch = searchRes, Language = language, Translation = t });
                            if (!searchOk && searchRes) searchOk = true;
                            // EditorGUILayout.LabelField(new GUIContent(t, t), cellStyle, cellWidthOption);

                        }
                        if (searchOk)
                        {
                            index++;
                            EditorGUILayout.BeginHorizontal();
                            if (selectionList != null) selectionList[key] = EditorGUILayout.Toggle(selectionList.ContainsKey(key) && selectionList[key], GUILayout.Width(16f));
                            if (keySearchResult) GUI.backgroundColor = Color.cyan;
                            GUI.SetNextControlName(key);
                            string newKey = EditorGUILayout.DelayedTextField(key, editableCellStyle, cellWidthOption);
                            GUI.backgroundColor = Color.white;
                            if (newKey != key)
                            {
                                foreach (Language language in languages)
                                {
                                    language.RenameKey(key, newKey);
                                }
                                modified = true;
                            }

                            for (int i = 0; i < cellList.Count; i++)
                            {
                                string value = cellList[i].Translation;
                                bool res = cellList[i].MatchSearch && !string.IsNullOrEmpty(searchFilter);

                                if (res) GUI.backgroundColor = Color.cyan;
                                GUI.SetNextControlName(string.Format("{0}_{1}", key, cellList[i].Language.LanguageId));
                                string newValue = EditorGUILayout.TextArea(value, editableCellStyle, cellWidthOption);
                                GUI.backgroundColor = Color.white;
                                if (newValue != value)
                                {
                                    cellList[i].Language.SetTranslation(key, newValue, true);
                                    modified = true;
                                }

                            }

                            EditorGUILayout.EndHorizontal();
                        }
                        if (modified) break;
                    }
                    else
                    {
                        GUI.SetNextControlName(key);
                        GUILayout.Space(lineHeight);
                        index++;
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            int selectionCount = selectionList != null ? selectionList.Values.Count(b => b) : 0;
            if (selectionCount > 0)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(string.Format("Remove selection ({0})", selectionCount), GUILayout.Width(150f)))
                {
                    foreach (string key in selectionList.Keys)
                    {
                        if (selectionList[key])
                            defaultLanguage.RemoveTranslation(key);
                    }
                    selectionList = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (keyToFocus != null)
            {
                GUI.FocusControl(keyToFocus);
                keyToFocus = null;
            }
        }
    }

}