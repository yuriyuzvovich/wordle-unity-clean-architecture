using Cysharp.Threading.Tasks;
using Wordle.Core.Entities;

namespace Wordle.Core.Interfaces
{
    /// <summary>
    /// Repository interface: Manages word lists for Wordle.
    /// Implementation will be in Infrastructure layer.
    /// </summary>
    public interface IWordRepository
    {
        UniTask InitializeAsync();
        UniTask<Word> GetRandomTargetWordAsync();
        UniTask<string[]> GetValidGuessWordsAsync();
        UniTask ReloadWordsAsync(string languageCode);
        bool IsInitialized { get; }
    }
}
