using System;

namespace Wordle.Application.Events
{
    /// <summary>
    /// Application event: A letter has been entered.
    /// Used for UI updates and animations.
    /// </summary>
    public struct LetterEnteredEvent
    {
        public readonly string EventId;
        public readonly DateTime Timestamp;
        public readonly char Letter;
        public readonly int Row;
        public readonly int Position;

        public LetterEnteredEvent(char letter, int row, int position)
        {
            EventId = nameof(LetterEnteredEvent);
            Timestamp = DateTime.UtcNow;
            Letter = char.ToUpper(letter);
            Row = row;
            Position = position;
        }

        public override string ToString()
        {
            return $"[{EventId}] Letter '{Letter}' entered at row {Row}, position {Position}";
        }
    }
}
