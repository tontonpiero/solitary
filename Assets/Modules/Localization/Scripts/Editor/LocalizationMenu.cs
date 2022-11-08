using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Modules.Localization.Editor
{
    public class LocalizationMenu : MonoBehaviour
    {

        /*[MenuItem("Localization/Refresh resources", false, 0)]
        static void RefreshResources()
        {
            LocalizationManager.Instance.Refresh();
        }*/

        [MenuItem("Localization/Edit Config", false, 1)]
        static void EditConfig()
        {
            LocalizationManager.Instance.EditConfig();
        }

        [MenuItem("Localization/Edit Translations", false, 101)]
        static void EditTranslations()
        {
            LocalizationTableWindow.ShowWindow();
        }

        [MenuItem("Localization/Download Texts", priority = 201)]
        static void UpdateTexts()
        {
            if (!string.IsNullOrWhiteSpace(LocalizationManager.Instance.Config.ExternalFilesUrl) && LocalizationManager.Instance.Config.ExternalFilesUrl.StartsWith("http"))
            {
                UpdateAllTexts(true);
            }
            else
            {
                EditorUtility.DisplayDialog("Localization", "You must set an external files url in localization config.", "ok");
            }
        }

        static async void UpdateAllTexts(bool showProgressbar)
        {
            List<Language> languages = LocalizationManager.Instance.GetLanguages();
            for (int i = 0; i < languages.Count; i++)
            {
                if (showProgressbar) EditorUtility.DisplayProgressBar("Localization", string.Format("Updating {0}...", languages[i].LanguageName), (float)i / (float)languages.Count);
                await UpdateTexts(languages[i]);
            }
            if (showProgressbar) EditorUtility.DisplayProgressBar("Localization", "Updating texts complete!", 1f);
            await Task.Delay(500);
            LocalizationManager.Instance.Refresh();
            LocalizationManager.Instance.SetCurrentLanguage(LocalizationManager.Instance.GetDefaultLanguage().LanguageId);
            EditorApplication.QueuePlayerLoopUpdate();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        static private async Task UpdateTexts(Language language)
        {
            string url = string.Format(LocalizationManager.Instance.Config.ExternalFilesUrl, language.GetFileName(), UnityEngine.Random.value);
            Debug.Log("UpdateTexts url=" + url);

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                var asyncOp = www.SendWebRequest();
                while (asyncOp.isDone == false)
                {
                    await Task.Yield();
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log($"{ www.error }, URL:{ www.url }");
                }
                else
                {
                    string text = www.downloadHandler.text;
                    language.AddTranslations(text, true);
                }
            }
        }

        static private (Type requiredComponent, Type localizeComponent)[] localizeComponents = new (Type requiredComponent, Type localizeComponent)[]
        {
            (typeof(Text), typeof(LocalizeText)),
            (typeof(TMP_Text), typeof(LocalizeText)),
            (typeof(Image), typeof(LocalizeImage)),
        };

        [MenuItem("GameObject/Localization/Localize", false, 0)]
        static void LocalizeObject()
        {
            GameObject obj = Selection.activeTransform.gameObject;
            if (obj != null)
            {
                bool result = false;
                foreach ((Type requiredComponent, Type localizeComponent) in localizeComponents)
                {
                    if (AddLocalizeComponent(obj, requiredComponent, localizeComponent))
                    {
                        result = true;
                        break;
                    }
                }
                if (result == false)
                {
                    if (!obj.GetComponent<LocalizeObject>()) obj.AddComponent<LocalizeObject>();
                }
            }
        }

        static private bool AddLocalizeComponent(GameObject obj, Type requiredComponent, Type localizeComponent)
        {
            if (obj.GetComponent(localizeComponent))
            {
                Debug.LogWarning("This object is already localized");
                return true;
            }

            if (obj.GetComponent(requiredComponent))
            {
                obj.AddComponent(localizeComponent);
                return true;
            }

            return false;
        }
    }
}