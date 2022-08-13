using UnityEngine;

namespace Utils.Audio
{
    public class PlaySound : MonoBehaviour
    {
        [SerializeField] private string soundName;
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;
        [SerializeField] private bool playOnStart = false;

        private void Start()
        {
            if (playOnStart) Play();
        }

        public void Play()
        {
            if (string.IsNullOrEmpty(soundName)) return;
            AudioManager.PlaySound(soundName, volume);
        }
    }
}
