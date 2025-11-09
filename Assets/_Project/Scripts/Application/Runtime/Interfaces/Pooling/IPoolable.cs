namespace Wordle.Application.Interfaces.Pooling
{
    /// <summary>
    /// Interface for objects that can be pooled.
    /// Provides lifecycle callbacks for when objects are retrieved from or returned to the pool.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when the object is retrieved from the pool.
        /// Use this to reset state and prepare the object for use.
        /// </summary>
        void OnSpawnFromPool();

        /// <summary>
        /// Called when the object is returned to the pool.
        /// Use this to clean up state and prepare for reuse.
        /// </summary>
        void OnReturnToPool();
    }
}
