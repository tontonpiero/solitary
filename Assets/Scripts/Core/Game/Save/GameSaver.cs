using Solitary.Utils;

namespace Solitary.Core
{
    public partial class Game
    {
        public class Saver : IGameSaver
        {
            public const string SaveGamePrefKey = "last_saved_game";
            private readonly IDataSaver<GameSaveData> dataSource;

            public Saver()
            {
                dataSource = new PlayerPrefsDataSaver<GameSaveData>(SaveGamePrefKey);
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
                    Id = game.Id,
                    Moves = game.Moves,
                    Score = game.Score,
                    Time = game.TotalTime,
                    SData = game.StockDeck.Save(),
                    RData = game.ReserveDeck.Save(),
                    FData = new DeckData[FoundationsCount],
                    CData = new DeckData[ColumnsCount]
                };
                for (int i = 0; i < FoundationsCount; i++)
                {
                    data.FData[i] = game.FoundationDecks[i].Save();
                }
                for (int i = 0; i < ColumnsCount; i++)
                {
                    data.CData[i] = game.ColumnDecks[i].Save();
                }

                data.Cmds = game.moveCommandInvoker.Save();

                return data;
            }

            public void Load(Game game)
            {
                GameSaveData data = dataSource.LoadData();
                LoadSaveData(game, data);
            }

            private void LoadSaveData(Game game, GameSaveData data)
            {
                game.Id = data.Id;
                game.SetMoves(data.Moves);
                game.SetScore(data.Score);
                game.TotalTime = data.Time;
                game.StockDeck.Load(data.SData);
                game.ReserveDeck.Load(data.RData);
                for (int i = 0; i < FoundationsCount; i++)
                {
                    game.FoundationDecks[i].Load(data.FData[i]);
                }
                for (int i = 0; i < ColumnsCount; i++)
                {
                    game.ColumnDecks[i].Load(data.CData[i]);
                }
                game.IsNew = false;

                game.moveCommandInvoker.Load(data.Cmds);
            }

            public void ClearData() => dataSource.ClearData();

            public bool HasData() => dataSource.HasData();
        }
    }
}
