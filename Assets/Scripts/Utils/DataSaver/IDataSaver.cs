namespace Solitary.Utils
{
    public interface IDataSaver<T> where T : new()
    {
        T LoadData();
        void LoadData(T target);
        void SaveData(T data);
        void ClearData();
        bool HasData();
    }
}
