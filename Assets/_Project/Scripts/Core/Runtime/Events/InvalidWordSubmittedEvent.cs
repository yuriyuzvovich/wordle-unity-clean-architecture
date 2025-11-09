namespace Wordle.Core.Events
{
    /// <summary>
    /// Domain event: An invalid word was submitted by the user.
    /// </summary>
    public struct InvalidWordSubmittedEvent
    {
        public readonly string EventId;
        public readonly System.DateTime Timestamp;
        public readonly string GuessWord;
        public readonly string Reason;

        public InvalidWordSubmittedEvent(string guessWord, string reason)
        {
            EventId = nameof(InvalidWordSubmittedEvent);
            Timestamp = System.DateTime.UtcNow;
            GuessWord = guessWord;
            Reason = reason;
        }

        public override string ToString()
        {
            return $"[{EventId}] Invalid word '{GuessWord}' - {Reason}";
        }
    }
}
