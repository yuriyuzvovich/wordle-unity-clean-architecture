using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;

namespace Wordle.Application.Tests.Mocks
{
    public class MockEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();
        public List<object> PublishedEvents = new List<object>();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<Delegate>();
            }
            _handlers[eventType].Add(handler);
        }

        public void Subscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent : struct
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<Delegate>();
            }
            _handlers[eventType].Add(asyncHandler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct
        {
            var eventType = typeof(TEvent);
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(handler);
            }
        }

        public void Unsubscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent : struct
        {
            var eventType = typeof(TEvent);
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(asyncHandler);
            }
        }

        public void Publish<TEvent>(TEvent eventData) where TEvent : struct
        {
            PublishedEvents.Add(eventData);
        }

        public UniTask PublishAsync<TEvent>(TEvent eventData) where TEvent : struct
        {
            PublishedEvents.Add(eventData);
            return UniTask.CompletedTask;
        }

        public void Clear<TEvent>() where TEvent : struct
        {
            var eventType = typeof(TEvent);
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Clear();
            }
        }

        public void ClearAll()
        {
            _handlers.Clear();
            PublishedEvents.Clear();
        }

        public bool WasEventPublished<TEvent>() where TEvent : struct
        {
            return PublishedEvents.Exists(e => e is TEvent);
        }

        public TEvent GetPublishedEvent<TEvent>() where TEvent : struct
        {
            foreach (var evt in PublishedEvents)
            {
                if (evt is TEvent typedEvent)
                {
                    return typedEvent;
                }
            }
            throw new InvalidOperationException($"No event of type {typeof(TEvent).Name} was published");
        }

        public int GetPublishedEventCount<TEvent>() where TEvent : struct
        {
            int count = 0;
            foreach (var evt in PublishedEvents)
            {
                if (evt is TEvent)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
