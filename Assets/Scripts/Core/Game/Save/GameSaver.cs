using Solitaire.Core;

namespace Solitary.Core
{
    public class GameSaver : IGameSaver
    {
        //public const string 
        private readonly IDataSource<GameSaveData> dataSource;

        public GameSaver()
        {
            dataSource = new PlayerPrefsDataSource<GameSaveData>("last_saved_game");
        }

        public void Save(Game game)
        {
            GameSaveData data = game.GetSaveData();
            dataSource.SaveData(data);
        }

        public void Load(Game game)
        {
            GameSaveData data = dataSource.LoadData();
            game.LoadSaveData(data);
        }

        public void ClearData() => dataSource.ClearData();

        public bool HasData() => dataSource.HasData();
    }
}
