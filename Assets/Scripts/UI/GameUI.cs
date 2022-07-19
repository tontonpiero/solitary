using UnityEngine;

namespace Solitary.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private LevelManager levelManager;

        public async void ExitGame()
        {
            await levelManager.LoadAsync("home");
        }
    }
}
