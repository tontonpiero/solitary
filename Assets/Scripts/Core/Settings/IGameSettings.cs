namespace Solitary.Core
{
    public interface IGameSettings
    {
        bool AllowHelp { get; set; }
        bool AllowUndo { get; set; }
        bool ThreeCardsMode { get; set; }

        void Load();
        void Save();
    }
}