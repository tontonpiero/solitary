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
            GameSaver gameSaver = new GameSaver();
            gameSaver.ClearData();
            await levelManager.RestartAsync();
        }

        public async void ExitGame()
        {
            await levelManager.LoadAsync("home");
        }
    }
}
