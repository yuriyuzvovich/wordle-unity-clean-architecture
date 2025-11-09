using System;

namespace Wordle.Application.Events
{
    /// <summary>
    /// Application event: A letter has been deleted.
    /// Used for UI updates and animations.
    /// </summary>
    public struct LetterDeletedEvent
    {
        public readonly string EventId;
        public readonly DateTime Timestamp;
        public readonly int Row;
        public readonly int Position;

        public LetterDeletedEvent(int row, int position)
        {
            EventId = nameof(LetterDeletedEvent);
            Timestamp = DateTime.UtcNow;
            Row = row;
            Position = position;
        }

        public override string ToString()
        {
            return $"[{EventId}] Letter deleted from row {Row}, position {Position}";
        }
    }
}
