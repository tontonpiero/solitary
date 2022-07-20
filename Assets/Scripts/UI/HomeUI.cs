using Solitary.Core;
using System.Threading.Tasks;
using UnityEngine;

namespace Solitary.UI
{
    public class HomeUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private LevelManager levelManager;

        public async void OnClickNewGame()
        {
            GameSaver gameSaver = new GameSaver();
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
