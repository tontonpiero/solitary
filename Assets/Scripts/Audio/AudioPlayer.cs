using UnityEngine;

namespace Solitary
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, ISoundPlayer
    {
        private AudioSource musicSource;

        private void Awake()
        {
            musicSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip)
        {
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void SetVolume(float volume)
        {
            musicSource.volume = volume;
        }

        public void Stop()
        {
            musicSource.Stop();
        }
    }
}
