namespace Wordle.Core.Events
{
    /// <summary>
    /// Domain event: The player has lost the game (ran out of attempts).
    /// </summary>
    public struct GameLostEvent
    {
        public readonly string EventId;
        public readonly System.DateTime Timestamp;
        public readonly string TargetWord;
        public readonly int AttemptsUsed;

        public GameLostEvent(string targetWord, int attemptsUsed)
        {
            AttemptsUsed = attemptsUsed;
            EventId = nameof(GameLostEvent);
            Timestamp = System.DateTime.UtcNow;
            TargetWord = targetWord;
        }

        public override string ToString()
        {
            return $"[{EventId}] Game lost. Target word was: {TargetWord}";
        }
    }
}
