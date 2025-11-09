using UnityEngine;
using Wordle.Application.Interfaces.Pooling;
using Wordle.Infrastructure.Common.Pooling;

namespace Wordle.Infrastructure.Pooling
{
    /// <summary>
    /// Object pool specifically for Unity GameObjects with components.
    /// Handles GameObject activation/deactivation and parent management.
    /// </summary>
    public class GameObjectPool<T> : IObjectPool<T> where T: Component
    {
        private readonly ObjectPool<T> _pool;
        private readonly T _prefab;
        private readonly Transform _parent;

        public int AvailableCount => _pool.AvailableCount;
        public int TotalCount => _pool.TotalCount;

        public GameObjectPool(T prefab, Transform parent = null, int initialSize = 0, int maxSize = 0)
        {
            _prefab = prefab ? prefab : throw new System.ArgumentNullException(nameof(prefab));
            _parent = parent;

            _pool = new ObjectPool<T>(
                CreateInstance,
                OnGet,
                OnReturn,
                OnDestroy,
                initialSize,
                maxSize
            );
        }

        private T CreateInstance()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private void OnGet(T item)
        {
            if (item)
            {
                item.gameObject.SetActive(true);
            }
        }

        private void OnReturn(T item)
        {
            if (item)
            {
                item.gameObject.SetActive(false);

                if (_parent && item.transform.parent != _parent)
                {
                    item.transform.SetParent(_parent);
                }
            }
        }

        private void OnDestroy(T item)
        {
            if (item)
            {
                // Use DestroyImmediate in edit mode (tests), Destroy in play mode
                if (UnityEngine.Application.isPlaying)
                {
                    Object.Destroy(item.gameObject);
                }
                else
                {
                    Object.DestroyImmediate(item.gameObject);
                }
            }
        }

        public T Get() => _pool.Get();

        public void Return(T item) => _pool.Return(item);

        public void Clear() => _pool.Clear();
    }
}