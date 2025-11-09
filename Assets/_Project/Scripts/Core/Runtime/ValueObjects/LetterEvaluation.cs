namespace Wordle.Core.ValueObjects
{
    /// <summary>
    /// Represents the evaluation result of a letter in a guess.
    /// </summary>
    public enum LetterEvaluation
    {
        Absent = 0,
        Present = 1,
        Correct = 2
    }
}
