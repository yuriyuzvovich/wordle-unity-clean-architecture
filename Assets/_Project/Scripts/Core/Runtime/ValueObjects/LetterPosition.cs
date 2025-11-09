namespace Wordle.Core.ValueObjects
{
    /// <summary>
    /// Represents a letter at a specific position with its evaluation.
    /// Immutable value object.
    /// </summary>
    public readonly struct LetterPosition
    {
        public readonly char Letter;
        public readonly int Position;
        public readonly LetterEvaluation Evaluation;

        public LetterPosition(char letter, int position, LetterEvaluation evaluation)
        {
            Letter = char.ToUpper(letter);
            Position = position;
            Evaluation = evaluation;
        }

        public override string ToString()
        {
            return $"{Letter}[{Position}]={Evaluation}";
        }
    }
}
