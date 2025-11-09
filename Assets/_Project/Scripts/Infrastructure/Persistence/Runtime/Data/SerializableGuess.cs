using System;
using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;

namespace Wordle.Infrastructure.Persistence
{
    [Serializable]
    public class SerializableGuess
    {
        public string guessedWord;
        public SerializableLetterPosition[] evaluations;

        public static SerializableGuess FromGuess(Guess guess)
        {
            var data = new SerializableGuess();
            data.guessedWord = guess.GuessedWord.Value;
            data.evaluations = new SerializableLetterPosition[guess.Evaluations.Length];

            for (int i = 0; i < guess.Evaluations.Length; i++)
            {
                data.evaluations[i] = SerializableLetterPosition.FromLetterPosition(guess.Evaluations[i]);
            }

            return data;
        }

        public Guess ToGuess()
        {
            var word = new Word(guessedWord);
            var letterPositions = new LetterPosition[evaluations.Length];

            for (int i = 0; i < evaluations.Length; i++)
            {
                letterPositions[i] = evaluations[i].ToLetterPosition();
            }

            return new Guess(word, letterPositions);
        }
    }
}