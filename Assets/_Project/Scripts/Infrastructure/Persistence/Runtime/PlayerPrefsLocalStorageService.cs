using UnityEngine;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Persistence
{
    public class PlayerPrefsLocalStorageService : ILocalStorageService
    {
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }
    }
}
