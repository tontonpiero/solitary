using UnityEngine;

namespace Utils.Audio
{
    public class AudioPlayer : MonoBehaviour, IAudioPlayer
    {
        private AudioSource musicAudioSource;
        private AudioSource sfxAudioSource;

        public float MusicVolume { get => musicAudioSource.volume; set => musicAudioSource.volume = value; }
        public float SfxVolume { get => sfxAudioSource.volume; set => sfxAudioSource.volume = value; }

        private void Awake()
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            gameObject.AddComponent<AudioListener>();

            DontDestroyOnLoad(gameObject);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;
            if (musicAudioSource.clip == clip && musicAudioSource.isPlaying) return;
            musicAudioSource.clip = clip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }

        public void PlaySfx(AudioClip clip, float volume)
        {
            if (clip == null) return;
            sfxAudioSource.PlayOneShot(clip, volume);
        }

        public void StopMusic()
        {
            musicAudioSource.Stop();
        }
    }
}
