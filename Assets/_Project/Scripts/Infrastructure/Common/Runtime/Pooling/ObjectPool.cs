using System;
using System.Collections.Generic;
using Wordle.Application.Interfaces.Pooling;

namespace Wordle.Infrastructure.Pooling
{
    /// <summary>
    /// Generic object pool implementation for reusable objects.
    /// </summary>
    public class ObjectPool<T> : IObjectPool<T> where T: class
    {
        private readonly Queue<T> _available;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onReturn;
        private readonly Action<T> _onDestroy;
        private readonly int _maxSize;
        private int _totalCount;

        public int AvailableCount => _available.Count;
        public int TotalCount => _totalCount;

        public ObjectPool(
            Func<T> createFunc,
            Action<T> onGet = null,
            Action<T> onReturn = null,
            Action<T> onDestroy = null,
            int initialSize = 0,
            int maxSize = 0
        )
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onReturn = onReturn;
            _onDestroy = onDestroy;
            _maxSize = maxSize;
            _available = new Queue<T>(initialSize > 0 ? initialSize : 8);

            for (int i = 0; i < initialSize; i++)
            {
                var item = _createFunc();
                _available.Enqueue(item);
                _totalCount++;
            }
        }

        public T Get()
        {
            T item;

            if (_available.Count > 0)
            {
                item = _available.Dequeue();
            }
            else
            {
                item = _createFunc();
                _totalCount++;
            }

            _onGet?.Invoke(item);

            if (item is IPoolable poolable)
            {
                poolable.OnSpawnFromPool();
            }

            return item;
        }

        public void Return(T item)
        {
            if (item == null)
            {
                return;
            }

            if (_maxSize > 0 && _available.Count >= _maxSize)
            {
                _onDestroy?.Invoke(item);
                _totalCount--;
                return;
            }

            _onReturn?.Invoke(item);

            if (item is IPoolable poolable)
            {
                poolable.OnReturnToPool();
            }

            _available.Enqueue(item);
        }

        public void Clear()
        {
            if (_onDestroy != null)
            {
                foreach (var item in _available)
                {
                    _onDestroy(item);
                }
            }

            _available.Clear();
            _totalCount = 0;
        }
    }
}