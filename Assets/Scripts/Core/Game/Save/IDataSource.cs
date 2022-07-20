using System.Threading.Tasks;

namespace Solitaire.Core
{
    public interface IDataSource<T> where T : new()
    {
        T LoadData();
        void SaveData(T data);
        void ClearData();
        bool HasData();
    }
}
