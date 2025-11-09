using Cysharp.Threading.Tasks;
using Wordle.Core.Entities;

namespace Wordle.Core.Interfaces
{
    /// <summary>
    /// Repository interface: Manages game state persistence.
    /// </summary>
    public interface IGameStateRepository
    {
        UniTask SaveAsync(GameState gameState);
        UniTask<GameState> LoadAsync();
        UniTask<bool> HasSavedStateAsync();
        UniTask ClearAsync();
    }
}
