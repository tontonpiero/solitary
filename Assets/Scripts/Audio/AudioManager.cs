using UnityEngine;

namespace Solitary
{

    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        static private AudioManager instance;

        [SerializeField] private AudioLibrary library;

        private const string SFXVolumeKey = "_sfx_volume_";
        private const string MusicVolumeKey = "_music_volume_";
        private float musicGlobalVolume = 0.5f;
        private float sfxGlobalVolume = 0.5f;

        private AudioSource musicSource;

        public float SFXGlobalVolume
        {
            get => sfxGlobalVolume;
            set
            {
                sfxGlobalVolume = value;
                PlayerPrefs.SetFloat(SFXVolumeKey, value);
            }
        }
        public float MusicGlobalVolume
        {
            get => musicGlobalVolume;
            set
            {
                musicGlobalVolume = value;
                musicSource.volume = musicGlobalVolume;
                PlayerPrefs.SetFloat(MusicVolumeKey, musicGlobalVolume);
            }
        }

        static public AudioManager Instance => instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            musicSource = GetComponent<AudioSource>();
            sfxGlobalVolume = PlayerPrefs.GetFloat(SFXVolumeKey, musicGlobalVolume);
            musicGlobalVolume = PlayerPrefs.GetFloat(MusicVolumeKey, sfxGlobalVolume);
        }



        public void PlaySound(string name, Vector3 position, float volume = 1f)
        {
            AudioClip clip = library.GetSound(name);
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, position, volume * SFXGlobalVolume);
        }

        public void PlaySound(string name, float volume)
        {
            PlaySound(name, Camera.main.transform.position, volume);
        }

        public void PlaySound(string name)
        {
            PlaySound(name, Camera.main.transform.position, 1f);
        }

        public void PlayMusic(string name)
        {
            AudioClip clip = library.GetSound(name);
            if (clip != null)
            {
                musicSource.volume = musicGlobalVolume;
                if (clip != musicSource.clip)
                {
                    musicSource.clip = library.GetSound(name);
                    musicSource.Play();
                }
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }
    }

}