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
