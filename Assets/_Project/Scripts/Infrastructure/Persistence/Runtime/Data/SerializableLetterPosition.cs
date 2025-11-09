using System;
using Wordle.Core.ValueObjects;

namespace Wordle.Infrastructure.Persistence
{
    [Serializable]
    public class SerializableLetterPosition
    {
        public char letter;
        public int position;
        public int evaluation;

        public static SerializableLetterPosition FromLetterPosition(LetterPosition letterPosition)
        {
            var data = new SerializableLetterPosition();
            data.letter = letterPosition.Letter;
            data.position = letterPosition.Position;
            data.evaluation = (int)letterPosition.Evaluation;
            return data;
        }

        public LetterPosition ToLetterPosition()
        {
            return new LetterPosition(letter, position, (LetterEvaluation)evaluation);
        }
    }
}