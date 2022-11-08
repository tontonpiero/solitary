using Solitary.Core;
using UnityEngine;
using UnityEngine.UI;
using Utils.Audio;

namespace Solitary
{
    public class SettingsPopup : BasePopup
    {
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle helpToggle;
        [SerializeField] private Toggle undoToggle;
        [SerializeField] private Toggle threeCardsToggle;
        [SerializeField] private Toggle ensureSolvableToggle;

        private float previousMusicVolume;
        private float previousSfxVolume;

        protected override void OnShown()
        {
            Initialize();
        }

        private void Initialize()
        {
            IGameSettings settings = new GameSettings();
            settings.Load();

            previousMusicVolume = AudioManager.MusicGlobalVolume;
            previousSfxVolume = AudioManager.SFXGlobalVolume;

            // music volume
            musicVolumeSlider.value = AudioManager.MusicGlobalVolume;

            // sfx volume
            sfxVolumeSlider.value = AudioManager.SFXGlobalVolume;

            // allow help
            helpToggle.isOn = settings.AllowHelp;

            // allow undo
            undoToggle.isOn = settings.AllowUndo;

            // 3 cards mode
            threeCardsToggle.isOn = settings.ThreeCardsMode;

            // ensure solbable
            ensureSolvableToggle.isOn = settings.EnsureSolvable;
        }

        public void OnMusicVolumeValueChange()
        {
            AudioManager.MusicGlobalVolume = musicVolumeSlider.value;
        }

        public void OnSfxVolumeValueChange()
        {
            AudioManager.SFXGlobalVolume = sfxVolumeSlider.value;
        }

        public void Cancel()
        {
            AudioManager.MusicGlobalVolume = previousMusicVolume;
            AudioManager.SFXGlobalVolume = previousSfxVolume;
            Hide();
        }

        public void Apply()
        {
            AudioManager.MusicGlobalVolume = musicVolumeSlider.value;
            AudioManager.SFXGlobalVolume = sfxVolumeSlider.value;

            IGameSettings settings = new GameSettings();
            settings.Load();

            if (threeCardsToggle.isOn != settings.ThreeCardsMode)
            {
                IGameSaver gameSaver = new Game.Saver();
                gameSaver.ClearData();
            }

            settings.AllowHelp = helpToggle.isOn;
            settings.AllowUndo = undoToggle.isOn;
            settings.ThreeCardsMode = threeCardsToggle.isOn;
            settings.EnsureSolvable = ensureSolvableToggle.isOn;

            settings.Save();

            Hide();
        }
    }
}
