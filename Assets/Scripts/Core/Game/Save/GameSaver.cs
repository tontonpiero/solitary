namespace Solitary.Core
{
    public partial class Game
    {
        public class Saver : IGameSaver
        {
            //public const string 
            private readonly IDataSource<GameSaveData> dataSource;

            public Saver()
            {
                dataSource = new PlayerPrefsDataSource<GameSaveData>("last_saved_game");
            }

            public void Save(Game game)
            {
                GameSaveData data = GetDataToSave(game);
                dataSource.SaveData(data);
            }

            public GameSaveData GetDataToSave(Game game)
            {
                GameSaveData data = new GameSaveData()
                {
                    Moves = game.Moves,
                    Score = game.Score,
                    TotalTime = game.TotalTime,
                    sData = game.StockDeck.Save(),
                    wData = game.WasteDeck.Save(),
                    fData = new DeckData[FoundationsCount],
                    cData = new DeckData[ColumnsCount]
                };
                for (int i = 0; i < FoundationsCount; i++)
                {
                    data.fData[i] = game.FoundationDecks[i].Save();
                }
                for (int i = 0; i < ColumnsCount; i++)
                {
                    data.cData[i] = game.ColumnDecks[i].Save();
                }
                return data;
            }

            public void Load(Game game)
            {
                GameSaveData data = dataSource.LoadData();
                LoadSaveData(game, data);
            }

            private void LoadSaveData(Game game, GameSaveData data)
            {
                game.SetMoves(data.Moves);
                game.SetScore(data.Score);
                game.TotalTime = data.TotalTime;
                game.StockDeck.Load(data.sData);
                game.WasteDeck.Load(data.wData);
                for (int i = 0; i < FoundationsCount; i++)
                {
                    game.FoundationDecks[i].Load(data.fData[i]);
                }
                for (int i = 0; i < ColumnsCount; i++)
                {
                    game.ColumnDecks[i].Load(data.cData[i]);
                }
                game.IsNew = false;
            }

            public void ClearData() => dataSource.ClearData();

            public bool HasData() => dataSource.HasData();
        }
    }
}
