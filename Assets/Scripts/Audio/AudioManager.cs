using UnityEngine;

namespace Solitary
{

    static public class AudioManager
    {
        private const string SFXVolumeKey = "_sfx_volume_";
        private const string MusicVolumeKey = "_music_volume_";

        static private AudioPlayer musicPlayer;
        static private AudioLibrary library;

        static private float musicGlobalVolume = 0.5f;
        static private float sfxGlobalVolume = 0.5f;
        static private bool isInitialized = false;

        static public float SFXGlobalVolume
        {
            get => sfxGlobalVolume;
            set
            {
                Initialize();
                if (!isInitialized) return;
                sfxGlobalVolume = value;
                PlayerPrefs.SetFloat(SFXVolumeKey, value);
            }
        }

        static public float MusicGlobalVolume
        {
            get => musicGlobalVolume;
            set
            {
                Initialize();
                if (!isInitialized) return;
                musicGlobalVolume = value;
                musicPlayer.SetVolume(musicGlobalVolume);
                PlayerPrefs.SetFloat(MusicVolumeKey, musicGlobalVolume);
            }
        }

        static private void Initialize()
        {
            if (isInitialized) return;
            if (!Application.isPlaying) return;
            sfxGlobalVolume = PlayerPrefs.GetFloat(SFXVolumeKey, musicGlobalVolume);
            musicGlobalVolume = PlayerPrefs.GetFloat(MusicVolumeKey, sfxGlobalVolume);

            library = Resources.Load<AudioLibrary>("AudioLibrary");
            if(library == null) return;

            musicPlayer = new GameObject("MusicPlayer").AddComponent<AudioPlayer>();
            GameObject.DontDestroyOnLoad(musicPlayer.gameObject);

            isInitialized = true;
        }

        static public void PlaySound(string name, Vector3 position, float volume = 1f)
        {
            Initialize();
            if (!isInitialized) return;
            AudioClip clip = library.GetSound(name);
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, position, volume * sfxGlobalVolume);
        }

        static public void PlaySound(string name, float volume)
        {
            PlaySound(name, Camera.main.transform.position, volume);
        }

        static public void PlaySound(string name)
        {
            PlaySound(name, Camera.main.transform.position, 1f);
        }

        static public void PlayMusic(string name)
        {
            Initialize();
            if (!isInitialized) return;
            AudioClip clip = library.GetSound(name);
            if (clip != null)
            {
                musicPlayer.Play(clip);
                musicPlayer.SetVolume(musicGlobalVolume);
            }
        }

        static public void StopMusic()
        {
            Initialize();
            if (!isInitialized) return;
            musicPlayer.Stop();
        }
    }

}