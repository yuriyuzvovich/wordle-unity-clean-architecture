namespace Wordle.Core.Events
{
    /// <summary>
    /// Domain event: A new game has started.
    /// </summary>
    public struct GameStartedEvent
    {
        public readonly string EventId;
        public readonly System.DateTime Timestamp;
        public readonly string TargetWord;
        public readonly int MaxAttempts;

        public GameStartedEvent(string targetWord, int maxAttempts)
        {
            EventId = nameof(GameStartedEvent);
            Timestamp = System.DateTime.UtcNow;
            TargetWord = targetWord;
            MaxAttempts = maxAttempts;
        }

        public override string ToString()
        {
            return $"[{EventId}] Game started with {MaxAttempts} attempts at {Timestamp:HH:mm:ss}";
        }
    }
}
