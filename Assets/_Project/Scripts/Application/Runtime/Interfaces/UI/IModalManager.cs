using Cysharp.Threading.Tasks;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Manages game modals (Win/Lose) lifecycle.
    /// </summary>
    public interface IModalManager
    {
        /// <summary>
        /// Initializes and loads all modals asynchronously.
        /// </summary>
        UniTask InitializeAsync();
    }
}
