using Solitary.Core;
using UnityEngine;

namespace Solitary.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private LevelManager levelManager;

        public async void OnClickNewGame()
        {
            IGameSaver gameSaver = new Game.Saver();
            gameSaver.ClearData();
            await levelManager.RestartAsync();
        }

        public async void ExitGame()
        {
            await levelManager.LoadAsync("home");
        }
    }
}
