using System;
using System.Collections.Generic;
using UnityEngine;
using Wordle.Application.Interfaces;
using Wordle.Application.Interfaces.Pooling;
using Wordle.Infrastructure.Pooling;

namespace Wordle.Infrastructure.Common.Pooling
{
    /// <summary>
    /// Service for managing multiple GameObject pools.
    /// Provides a centralized way to create and manage pools for different component types.
    /// </summary>
    public class ObjectPoolService : IObjectPoolService
    {
        private readonly Dictionary<Type, object> _pools;
        private readonly ILogService _logger;

        public ObjectPoolService(ILogService logger)
        {
            _pools = new Dictionary<Type, object>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void CreatePool<T>(T prefab, Transform parent = null, int initialSize = 0, int maxSize = 0) where T : Component
        {
            var type = typeof(T);

            if (_pools.ContainsKey(type))
            {
                _logger.LogWarning($"ObjectPoolService: Pool for type {type.Name} already exists");
                return;
            }

            var pool = new GameObjectPool<T>(prefab, parent, initialSize, maxSize);
            _pools[type] = pool;

            _logger.LogInfo($"ObjectPoolService: Created pool for {type.Name} (initialSize: {initialSize}, maxSize: {maxSize})");
        }

        public T Get<T>() where T : Component
        {
            var type = typeof(T);

            if (!_pools.TryGetValue(type, out var poolObj))
            {
                _logger.LogError($"ObjectPoolService: No pool found for type {type.Name}. Call CreatePool<{type.Name}>() first.");
                return null;
            }

            var pool = poolObj as IObjectPool<T>;
            return pool.Get();
        }

        public void Return<T>(T item) where T : Component
        {
            if (!item)
            {
                return;
            }

            var type = typeof(T);

            if (!_pools.TryGetValue(type, out var poolObj))
            {
                _logger.LogWarning($"ObjectPoolService: No pool found for type {type.Name}. Destroying object instead.");
                UnityEngine.Object.Destroy(item.gameObject);
                return;
            }

            var pool = poolObj as IObjectPool<T>;
            pool.Return(item);
        }

        public bool HasPool<T>() where T : Component
        {
            return _pools.ContainsKey(typeof(T));
        }

        public void ClearPool<T>() where T : Component
        {
            var type = typeof(T);

            if (!_pools.TryGetValue(type, out var poolObj))
            {
                _logger.LogWarning($"ObjectPoolService: No pool found for type {type.Name}");
                return;
            }

            var pool = poolObj as IObjectPool<T>;
            pool.Clear();
            _pools.Remove(type);

            _logger.LogInfo($"ObjectPoolService: Cleared pool for {type.Name}");
        }

        public void ClearAllPools()
        {
            foreach (var kvp in _pools)
            {
                if (kvp.Value is IObjectPool<Component> pool)
                {
                    pool.Clear();
                }
            }

            _pools.Clear();
            _logger.LogInfo("ObjectPoolService: Cleared all pools");
        }
    }
}
