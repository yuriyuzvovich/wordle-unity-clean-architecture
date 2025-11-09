namespace Wordle.Core.Events
{
    /// <summary>
    /// Domain event: The player has won the game.
    /// </summary>
    public struct GameWonEvent
    {
        public readonly string EventId;
        public readonly System.DateTime Timestamp;
        public readonly string TargetWord;
        public readonly int AttemptsUsed;

        public GameWonEvent(string targetWord, int attemptsUsed)
        {
            EventId = nameof(GameWonEvent);
            Timestamp = System.DateTime.UtcNow;
            TargetWord = targetWord;
            AttemptsUsed = attemptsUsed;
        }

        public override string ToString()
        {
            return $"[{EventId}] Game won! Word: {TargetWord}, Attempts: {AttemptsUsed}";
        }
    }
}
