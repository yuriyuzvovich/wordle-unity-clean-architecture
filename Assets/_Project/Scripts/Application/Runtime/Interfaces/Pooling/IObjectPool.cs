using System;

namespace Wordle.Application.Interfaces.Pooling
{
    /// <summary>
    /// Generic object pool interface for managing reusable objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to pool</typeparam>
    public interface IObjectPool<T> where T : class
    {
        /// <summary>
        /// Gets an object from the pool. Creates a new one if pool is empty.
        /// </summary>
        T Get();

        /// <summary>
        /// Returns an object to the pool for reuse.
        /// </summary>
        void Return(T item);

        /// <summary>
        /// Clears all objects from the pool.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the current number of objects available in the pool.
        /// </summary>
        int AvailableCount { get; }

        /// <summary>
        /// Gets the total number of objects created (both active and pooled).
        /// </summary>
        int TotalCount { get; }
    }
}
