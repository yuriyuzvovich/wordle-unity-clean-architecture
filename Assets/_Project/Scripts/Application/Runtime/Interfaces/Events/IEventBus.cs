using System;
using Cysharp.Threading.Tasks;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Event bus for decoupled communication between systems.
    /// Supports both synchronous and asynchronous event handlers.
    /// All events must inherit from BaseEvent struct.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribe to an event with a synchronous handler.
        /// </summary>
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct;

        /// <summary>
        /// Subscribe to an event with an asynchronous handler.
        /// </summary>
        void Subscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent : struct;

        /// <summary>
        /// Unsubscribe a synchronous handler from an event.
        /// </summary>
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct;

        /// <summary>
        /// Unsubscribe an asynchronous handler from an event.
        /// </summary>
        void Unsubscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent : struct;

        /// <summary>
        /// Publish an event synchronously.
        /// All sync handlers execute immediately, async handlers are fire-and-forget.
        /// </summary>
        void Publish<TEvent>(TEvent eventData) where TEvent : struct;

        /// <summary>
        /// Publish an event asynchronously.
        /// Waits for all handlers (sync and async) to complete.
        /// </summary>
        UniTask PublishAsync<TEvent>(TEvent eventData) where TEvent : struct;

        /// <summary>
        /// Clear all subscriptions for a specific event type.
        /// </summary>
        void Clear<TEvent>() where TEvent : struct;

        /// <summary>
        /// Clear all subscriptions for all events.
        /// </summary>
        void ClearAll();
    }
}
