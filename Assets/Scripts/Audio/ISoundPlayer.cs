using UnityEngine;

namespace Solitary
{
    public interface ISoundPlayer
    {
        void Play(AudioClip clip);
        void SetVolume(float volume);
        void Stop();
    }
}