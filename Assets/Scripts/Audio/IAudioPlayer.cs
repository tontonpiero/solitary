using UnityEngine;

namespace Solitary
{
    public interface IAudioPlayer
    {
        void Play(AudioClip clip, bool loop = false);
        void SetVolume(float volume);
        void Stop();
    }
}