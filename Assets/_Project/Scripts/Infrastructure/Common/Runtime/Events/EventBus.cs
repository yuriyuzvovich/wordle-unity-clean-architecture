using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Common.Events
{
    /// <summary>
    /// Thread-safe event bus implementation.
    /// Supports both synchronous and asynchronous event handlers.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _syncHandlers = new ();
        private readonly Dictionary<Type, List<Delegate>> _asyncHandlers = new ();
        private readonly object _lock = new object();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent: struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (_lock)
            {
                var eventType = typeof(TEvent);

                if (!_syncHandlers.ContainsKey(eventType))
                {
                    _syncHandlers[eventType] = new List<Delegate>();
                }

                _syncHandlers[eventType].Add(handler);
            }
        }

        public void Subscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent: struct
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            lock (_lock)
            {
                var eventType = typeof(TEvent);

                if (!_asyncHandlers.ContainsKey(eventType))
                {
                    _asyncHandlers[eventType] = new List<Delegate>();
                }

                _asyncHandlers[eventType].Add(asyncHandler);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent: struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (_lock)
            {
                var eventType = typeof(TEvent);

                if (_syncHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(handler);

                    if (handlers.Count == 0)
                    {
                        _syncHandlers.Remove(eventType);
                    }
                }
            }
        }

        public void Unsubscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent: struct
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            lock (_lock)
            {
                var eventType = typeof(TEvent);

                if (_asyncHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(asyncHandler);

                    if (handlers.Count == 0)
                    {
                        _asyncHandlers.Remove(eventType);
                    }
                }
            }
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent: struct
        {
            var eventType = typeof(TEvent);
            List<Delegate> syncHandlersCopy;
            List<Delegate> asyncHandlersCopy;

            lock (_lock)
            {
                // Create copies to avoid lock during handler execution
                syncHandlersCopy = _syncHandlers.TryGetValue(eventType, out var syncHandlers)
                    ? new List<Delegate>(syncHandlers)
                    : null;

                asyncHandlersCopy = _asyncHandlers.TryGetValue(eventType, out var asyncHandlers)
                    ? new List<Delegate>(asyncHandlers)
                    : null;
            }

            // Execute sync handlers
            if (syncHandlersCopy != null)
            {
                foreach (var handler in syncHandlersCopy)
                {
                    try
                    {
                        ((Action<TEvent>) handler)(eventData);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Error in sync event handler for {eventType.Name}: {ex}");
                    }
                }
            }

            // Fire-and-forget async handlers
            if (asyncHandlersCopy != null)
            {
                foreach (var handler in asyncHandlersCopy)
                {
                    try
                    {
                        ((Func<TEvent, UniTask>) handler)(eventData).Forget();
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Error in async event handler for {eventType.Name}: {ex}");
                    }
                }
            }
        }

        public async UniTask PublishAsync<TEvent>(TEvent eventData) where TEvent: struct
        {
            var eventType = typeof(TEvent);
            List<Delegate> syncHandlersCopy;
            List<Delegate> asyncHandlersCopy;

            lock (_lock)
            {
                // Create copies to avoid lock during handler execution
                syncHandlersCopy = _syncHandlers.TryGetValue(eventType, out var syncHandlers)
                    ? new List<Delegate>(syncHandlers)
                    : null;

                asyncHandlersCopy = _asyncHandlers.TryGetValue(eventType, out var asyncHandlers)
                    ? new List<Delegate>(asyncHandlers)
                    : null;
            }

            // Execute sync handlers
            if (syncHandlersCopy != null)
            {
                foreach (var handler in syncHandlersCopy)
                {
                    try
                    {
                        ((Action<TEvent>) handler).Invoke(eventData);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Error in sync event handler for {eventType.Name}: {ex}");
                    }
                }
            }

            // Await all async handlers (run in parallel but each handler handles its own exceptions)
            if (asyncHandlersCopy != null)
            {
                var tasks = new List<UniTask>();

                // Local wrapper to ensure each handler's exception is caught and logged independently
                async UniTask InvokeHandlerSafely(Func<TEvent, UniTask> h, TEvent data)
                {
                    try
                    {
                        await h(data);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Error in async event handler for {eventType.Name}: {ex}");
                    }
                }

                foreach (var handler in asyncHandlersCopy)
                {
                    try
                    {
                        var func = (Func<TEvent, UniTask>) handler;
                        tasks.Add(InvokeHandlerSafely(func, eventData));
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Error invoking async event handler for {eventType.Name}: {ex}");
                    }
                }

                try
                {
                    await UniTask.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    // Defensive: per-handler wrapper should prevent rethrow, but log unexpected errors.
                    UnityEngine.Debug.LogError($"Error waiting for async event handlers for {eventType.Name}: {ex}");
                }
            }
        }

        public void Clear<TEvent>() where TEvent: struct
        {
            lock (_lock)
            {
                var eventType = typeof(TEvent);
                _syncHandlers.Remove(eventType);
                _asyncHandlers.Remove(eventType);
            }
        }

        public void ClearAll()
        {
            lock (_lock)
            {
                _syncHandlers.Clear();
                _asyncHandlers.Clear();
            }
        }
    }
}