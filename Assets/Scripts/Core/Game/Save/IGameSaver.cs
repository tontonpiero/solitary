using System.Threading.Tasks;

namespace Solitary.Core
{
    public interface IGameSaver
    {
        void ClearData();
        bool HasData();
        void Load(Game game);
        void Save(Game game);
    }
}