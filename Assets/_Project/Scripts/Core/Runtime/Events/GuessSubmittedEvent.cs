namespace Wordle.Core.Events
{
    /// <summary>
    /// Domain event: A guess has been submitted and evaluated.
    /// </summary>
    public struct GuessSubmittedEvent
    {
        public readonly string EventId;
        public readonly System.DateTime Timestamp;
        public readonly string GuessWord;
        public readonly int AttemptsRemaining;
        public readonly bool IsCorrect;

        public GuessSubmittedEvent(string guessWord, int attemptsRemaining, bool isCorrect)
        {
            EventId = nameof(GuessSubmittedEvent);
            Timestamp = System.DateTime.UtcNow;
            GuessWord = guessWord;
            AttemptsRemaining = attemptsRemaining;
            IsCorrect = isCorrect;
        }

        public override string ToString()
        {
            return $"[{EventId}] Guess '{GuessWord}' - {AttemptsRemaining} attempts remaining (Correct: {IsCorrect})";
        }
    }
}
