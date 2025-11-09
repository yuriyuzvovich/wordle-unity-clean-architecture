
namespace Wordle.Application.Interfaces
{
    public interface IConfigRegistry
    {
        void Register<TConfig>(TConfig config) where TConfig : class;
        void Register<TConfig>(string key, TConfig config) where TConfig : class;
        TConfig Get<TConfig>() where TConfig : class;
        TConfig Get<TConfig>(string key) where TConfig : class;
        bool TryGet<TConfig>(out TConfig config) where TConfig : class;
        bool TryGet<TConfig>(string key, out TConfig config) where TConfig : class;
        bool IsRegistered<TConfig>() where TConfig : class;
        bool IsRegistered<TConfig>(string key) where TConfig : class;
    }
}
