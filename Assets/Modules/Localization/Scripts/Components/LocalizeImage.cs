using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Localization
{
    [Serializable]
    public class LocalizedSprite
    {
        public int LanguageId;
        public Sprite Sprite;
    }

    [AddComponentMenu("Modules/Localization/LocalizeImage")]
    [ExecuteInEditMode]
    public class LocalizeImage : MonoBehaviour
    {
        [SerializeField]
        protected List<LocalizedSprite> Sprites;

        [SerializeField]
        protected Sprite DefaultSprite;

        private Image image;
        private bool needToUpdateImage = true;

        void Awake()
        {
            image = GetComponent<Image>();
            if (image != null && DefaultSprite == null) DefaultSprite = image.sprite;
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
            UpdateImage();
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }
        private void Update()
        {
            if (needToUpdateImage || !Application.isPlaying) UpdateImage();
        }

        private void OnLanguageChanged()
        {
            needToUpdateImage = true;
        }

        private void UpdateImage()
        {
            if (Sprites != null && Sprites.Count > 0)
            {
                if (!SetLanguageSprite(LocalizationManager.Instance.GetCurrentLanguage()))
                {
                    if (image != null) image.sprite = DefaultSprite;
                }
            }
            else
            {
                if (image != null) image.sprite = DefaultSprite;
            }
            needToUpdateImage = false;
        }

        private bool SetLanguageSprite(Language language)
        {
            if (language != null)
            {
                LocalizedSprite locSprite = Sprites.Find(s => s.LanguageId == language.LanguageId);
                if (locSprite != null && locSprite.Sprite != null)
                {
                    if (image != null) image.sprite = locSprite.Sprite;
                    return true;
                }
            }
            return false;
        }
    }
}