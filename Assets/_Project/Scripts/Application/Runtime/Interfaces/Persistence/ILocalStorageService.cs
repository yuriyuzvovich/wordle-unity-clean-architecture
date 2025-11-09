namespace Wordle.Application.Interfaces
{
    public interface ILocalStorageService
    {
        bool HasKey(string key);
        string GetString(string key);
        void SetString(string key, string value);
        void DeleteKey(string key);
        void Save();
    }
}
