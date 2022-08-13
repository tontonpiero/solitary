using Solitary.Utils;
using System;

namespace Solitary
{
    public class StatsManager
    {
        private readonly IDataSaver<StatsData> dataSaver;

        private StatsData data;

        public StatsData Data => data;

        public StatsManager(IDataSaver<StatsData> dataSaver = null)
        {
            if (dataSaver == null) dataSaver = new PlayerPrefsDataSaver<StatsData>("_stats_");
            this.dataSaver = dataSaver;
            data = dataSaver.LoadData();
        }

        public void SaveScore(int score)
        {
            if (score > data.BestScore)
            {
                data.BestScore = score;
                dataSaver.SaveData(data);
            }
        }

        [Serializable]
        public struct StatsData
        {
            public int BestScore;
        }
    }
}
