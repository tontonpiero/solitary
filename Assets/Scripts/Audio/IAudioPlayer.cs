using UnityEngine;

namespace Utils.Audio
{
    public interface IAudioPlayer
    {
        float MusicVolume { get; set; }
        float SfxVolume { get; set; }

        void PlayMusic(AudioClip clip);
        void PlaySfx(AudioClip clip, float volume);
        void StopMusic();
    }
}