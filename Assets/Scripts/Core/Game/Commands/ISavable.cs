namespace Solitary.Core
{
    public interface ISavable<T>
    {
        T Save();
        void Load(T data);
    }
}