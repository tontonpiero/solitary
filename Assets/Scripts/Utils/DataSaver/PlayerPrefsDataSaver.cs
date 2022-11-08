using System;
using UnityEngine;

namespace Solitary.Utils
{
    public class PlayerPrefsDataSaver<T> : IDataSaver<T> where T : new()
    {
        private readonly string key;

        public PlayerPrefsDataSaver(string key)
        {
            this.key = key;
        }

        public T LoadData()
        {
            string strData = PlayerPrefs.GetString(key, null);
            Debug.Log($"PlayerPrefsDataSource - LoadData() {strData}");
            if (string.IsNullOrEmpty(strData)) return default;
            try
            {
                return JsonUtility.FromJson<T>(strData);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public void LoadData(T target)
        {
            string strData = PlayerPrefs.GetString(key, null);
            //Debug.Log($"PlayerPrefsDataSource - LoadData() {strData}");
            if (string.IsNullOrEmpty(strData)) return;
            try
            {
                JsonUtility.FromJsonOverwrite(strData, target);
            }
            catch (Exception) { }
        }

        public void SaveData(T data)
        {
            string strData = JsonUtility.ToJson(data);
            //Debug.Log($"PlayerPrefsDataSource - SaveData() {strData}");
            PlayerPrefs.SetString(key, strData);
        }

        public void ClearData() => PlayerPrefs.DeleteKey(key);

        public bool HasData() => PlayerPrefs.HasKey(key);
    }
}
