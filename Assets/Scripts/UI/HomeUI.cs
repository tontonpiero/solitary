using Solitary.Core;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Solitary.UI
{
    public class HomeUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private LevelManager levelManager;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;

        private void Start()
        {
            AudioManager.Instance.MusicGlobalVolume = 0.2f;
            AudioManager.Instance.SFXGlobalVolume = 1f;
            AudioManager.Instance.PlayMusic("main_music");

            IGameSaver gameSaver = new Game.Saver();
            resumeButton.interactable = gameSaver.HasData();
        }

        public async void OnClickNewGame()
        {
            IGameSaver gameSaver = new Game.Saver();
            gameSaver.ClearData();
            await levelManager.LoadAsync("game");
        }

        public async void OnClickResumeGame()
        {
            await levelManager.LoadAsync("game");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
