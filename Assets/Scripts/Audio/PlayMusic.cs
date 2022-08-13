using UnityEngine;

namespace Utils.Audio
{
    public class PlayMusic : MonoBehaviour
    {
        [SerializeField] private string musicName;
        [SerializeField] private bool playOnStart = true;

        private void Start()
        {
            if (playOnStart) Play();
        }

        public void Play()
        {
            if (string.IsNullOrEmpty(musicName)) return;
            AudioManager.PlayMusic(musicName);
        }
    }
}
