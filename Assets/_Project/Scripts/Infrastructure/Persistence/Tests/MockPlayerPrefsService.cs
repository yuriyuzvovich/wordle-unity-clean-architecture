using System.Collections.Generic;
using Wordle.Application.Interfaces;

namespace Tests
{
    public class MockPlayerPrefsService : ILocalStorageService
    {
        private readonly Dictionary<string, string> _storage = new Dictionary<string, string>();

        public bool HasKey(string key)
        {
            return _storage.ContainsKey(key);
        }

        public string GetString(string key)
        {
            return _storage.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public void SetString(string key, string value)
        {
            _storage[key] = value;
        }

        public void DeleteKey(string key)
        {
            _storage.Remove(key);
        }

        public void Save()
        {
        }

        public void Clear()
        {
            _storage.Clear();
        }
    }
}
