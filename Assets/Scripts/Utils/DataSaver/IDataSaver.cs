namespace Solitary.Utils
{
    public interface IDataSaver<T> where T : new()
    {
        T LoadData();
        void SaveData(T data);
        void ClearData();
        bool HasData();
    }
}
