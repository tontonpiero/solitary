using Solitary.Utils;
using System;
using UnityEngine;

namespace Solitary.Core
{
    [Serializable]
    public class GameSettings : IGameSettings
    {
        public const string SaveGamePrefKey = "game_settings";
        private readonly IDataSaver<GameSettings> dataSource;

        [SerializeField] private bool allowHelp = true;
        [SerializeField] private bool allowUndo = true;
        [SerializeField] private bool threeCardsMode = true;

        public bool AllowHelp { get { return allowHelp; } set { allowHelp = value; } }
        public bool AllowUndo { get { return allowUndo; } set { allowUndo = value; } }
        public bool ThreeCardsMode { get { return threeCardsMode; } set { threeCardsMode = value; } }

        public GameSettings()
        {
            dataSource = new PlayerPrefsDataSaver<GameSettings>(SaveGamePrefKey);
        }

        public void Load() => dataSource.LoadData(this);

        public void Save() => dataSource.SaveData(this);
    }
}
