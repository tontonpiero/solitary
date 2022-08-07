using UnityEngine;

namespace Solitary
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip, bool loop = false)
        {
            if (audioSource.clip == clip && audioSource.isPlaying) return;
            audioSource.clip = clip;
            audioSource.Play();
            audioSource.loop = loop;
        }

        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }

        public void Stop()
        {
            audioSource.Stop();
        }
    }
}
