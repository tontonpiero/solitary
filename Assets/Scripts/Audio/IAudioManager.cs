using UnityEngine;

namespace Solitary
{
    public interface IAudioManager
    {
        float MusicGlobalVolume { get; set; }
        float SFXGlobalVolume { get; set; }

        void PlaySound(string name);
        void PlaySound(string name, float volume);
        void PlaySound(string name, Vector3 position, float volume = 1f);
        void PlayMusic(string name);
        void StopMusic();
    }
}