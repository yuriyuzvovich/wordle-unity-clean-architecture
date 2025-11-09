using System;

namespace Wordle.Core.Events
{
    /// <summary>
    /// Base event struct for all events in the system.
    /// Events are immutable value types with metadata.
    /// </summary>
    public struct BaseEvent
    {
        /// <summary>
        /// Unique identifier for this event type.
        /// </summary>
        public readonly string EventId;

        /// <summary>
        /// When this event was created.
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// Constructor for derived events.
        /// </summary>
        public BaseEvent(string eventId)
        {
            EventId = eventId;
            Timestamp = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[{EventId}] at {Timestamp:HH:mm:ss.fff}";
        }
    }
}
