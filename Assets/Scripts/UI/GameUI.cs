using Solitary.Core;
using Solitary.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Solitary.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private LevelManager levelManager;

        [Header("Menu")]
        [SerializeField] private Button helpButton;
        [SerializeField] private Button undoButton;

        private void Start()
        {
            IGameSettings settings = new GameSettings();
            settings.Load();

            helpButton.interactable = settings.AllowHelp;
            undoButton.interactable = settings.AllowUndo;
        }

        public async void OnClickNewGame()
        {
            IGameSaver gameSaver = new Game.Saver();
            gameSaver.ClearData();
            await levelManager.RestartAsync();
        }

        public async void OnClickRestartGame()
        {
            IGameSaver gameSaver = new Game.Saver();
            gameSaver.ClearData();
            gameManager.RestartGame();
            await levelManager.RestartAsync();
        }

        public async void ExitGame()
        {
            gameManager.SaveGame();
            await levelManager.LoadAsync("home");
        }
    }
}
