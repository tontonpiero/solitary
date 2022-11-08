using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Localization
{
    [Serializable]
    public class LocalizedObject
    {
        public int LanguageId;
        public GameObject TargetObject;
    }

    [AddComponentMenu("Modules/Localization/LocalizeObject")]
    [ExecuteInEditMode]
    public class LocalizeObject : MonoBehaviour
    {
        [SerializeField]
        protected List<LocalizedObject> Objects;

        public bool FallbackToDefaultLanguage = true;

        void Start()
        {
            UpdateObjects();
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateObjects;
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateObjects;
        }

#if UNITY_EDITOR
        void Update()
        {
            UpdateObjects();
        }
#endif

        private void UpdateObjects()
        {
            if (Objects != null && Objects.Count > 0)
            {
                if (!SetLanguageObject(LocalizationManager.Instance.GetCurrentLanguage()) && FallbackToDefaultLanguage)
                {
                    SetLanguageObject(LocalizationManager.Instance.GetDefaultLanguage());
                }
            }
        }

        private bool SetLanguageObject(Language language)
        {
            bool found = false;
            if (language != null)
            {
                foreach (LocalizedObject item in Objects)
                {
                    if (item.LanguageId == language.LanguageId)
                    {
                        found = true;
                        if (item.TargetObject != null) item.TargetObject.SetActive(true);
                    }
                    else if (item.TargetObject != null) item.TargetObject.SetActive(false);
                }
            }
            return found;
        }
    }
}