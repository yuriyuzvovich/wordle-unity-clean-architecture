using Cysharp.Threading.Tasks;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Manages the main game canvas lifecycle.
    /// </summary>
    public interface ICanvasManager
    {
        /// <summary>
        /// Initializes and loads the canvas asynchronously.
        /// </summary>
        UniTask InitializeAsync();
    }
}
