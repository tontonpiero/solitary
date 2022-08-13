using UnityEngine;

namespace Utils.Audio
{

    static public class AudioManager
    {
        private const string SFXVolumeKey = "_sfx_volume_";
        private const string MusicVolumeKey = "_music_volume_";

        static private AudioPlayer audioPlayer;
        static private AudioLibrary library;

        static private bool isInitialized = false;

        static public float SFXGlobalVolume
        {
            get
            {
                Initialize();
                if (!isInitialized) return 0f;
                return audioPlayer.SfxVolume;
            }
            set
            {
                Initialize();
                if (!isInitialized) return;
                audioPlayer.SfxVolume = value;
                PlayerPrefs.SetFloat(SFXVolumeKey, value);
            }
        }

        static public float MusicGlobalVolume
        {
            get
            {
                Initialize();
                if (!isInitialized) return 0f;
                return audioPlayer.MusicVolume;
            }
            set
            {
                Initialize();
                if (!isInitialized) return;
                audioPlayer.MusicVolume = value;
                PlayerPrefs.SetFloat(MusicVolumeKey, value);
            }
        }

        static private void Initialize()
        {
            if (isInitialized) return;
            if (!Application.isPlaying) return;

            library = Resources.Load<AudioLibrary>("AudioLibrary");
            if (library == null) return;

            audioPlayer = new GameObject("AudioPlayer").AddComponent<AudioPlayer>();

            isInitialized = true;

            SFXGlobalVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 0.5f);
            MusicGlobalVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        }

        static public void PlaySound(string name, float volume = 1f)
        {
            Initialize();
            if (!isInitialized) return;
            audioPlayer.PlaySfx(library.GetSound(name), volume);
        }

        static public void PlayMusic(string name)
        {
            Initialize();
            if (!isInitialized) return;
            audioPlayer.PlayMusic(library.GetSound(name));
        }

        static public void StopMusic()
        {
            Initialize();
            if (!isInitialized) return;
            audioPlayer.StopMusic();
        }
    }

}