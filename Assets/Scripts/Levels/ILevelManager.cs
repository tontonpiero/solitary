using System.Threading.Tasks;

namespace Solitary
{
    public interface ILevelManager
    {
        void Load(string name, bool additive = false);
        Task LoadAsync(string name, bool additive = false);
        void Restart();
        Task RestartAsync();
    }
}