using Newtonsoft.Json;
using UnityEngine;

namespace Core.Utils
{
    public class SaveLoadHelper<T>
    {
        //may be use encryption later
        private string _key;

        public SaveLoadHelper(string key)
        {
            _key = key;
        }

        public void SaveData(T data)
        {
            string dataStr = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(_key, dataStr);
        }

        public T LoadData()
        {
            string dataStr = PlayerPrefs.GetString(_key, string.Empty);
            return Parse(dataStr);
        }

        private T Parse(string dataStr)
        {
            T data = JsonConvert.DeserializeObject<T>(dataStr);
            return data;
        }
    }
}
