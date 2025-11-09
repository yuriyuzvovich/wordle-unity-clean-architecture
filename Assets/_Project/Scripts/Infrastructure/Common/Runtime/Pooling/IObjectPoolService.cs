using UnityEngine;

namespace Wordle.Infrastructure.Pooling
{
    /// <summary>
    /// Service for managing multiple object pools, specifically for Unity GameObjects with components.
    /// </summary>
    public interface IObjectPoolService
    {
        void CreatePool<T>(T prefab, Transform parent = null, int initialSize = 0, int maxSize = 0) where T : Component;
        T Get<T>() where T : Component;
        void Return<T>(T item) where T : Component;
        bool HasPool<T>() where T : Component;
        void ClearPool<T>() where T : Component;
        void ClearAllPools();
    }
}
