using System;
using Wordle.Core.Entities;

namespace Wordle.Infrastructure.Persistence
{
    [Serializable]
    public class SerializableGameState
    {
        public string targetWord;
        public int maxAttempts;
        public string startTime;
        public int status;
        public SerializableGuess[] guesses;

        public static SerializableGameState FromGameState(GameState gameState)
        {
            var data = new SerializableGameState();
            data.targetWord = gameState.TargetWord.Value;
            data.maxAttempts = gameState.MaxAttempts;
            data.startTime = gameState.StartTime.ToString("o");
            data.status = (int)gameState.Status;
            data.guesses = new SerializableGuess[gameState.Guesses.Count];

            for (int i = 0; i < gameState.Guesses.Count; i++)
            {
                data.guesses[i] = SerializableGuess.FromGuess(gameState.Guesses[i]);
            }

            return data;
        }

        public GameState ToGameState()
        {
            var newTargetWord = new Word(this.targetWord);
            var newStartTime = DateTime.Parse(this.startTime);

            var gameState = new GameState(newTargetWord, maxAttempts);

            for (int i = 0; i < guesses.Length; i++)
            {
                var guess = guesses[i].ToGuess();
                if (gameState.CanMakeGuess())
                {
                    gameState.AddGuess(guess);
                }
            }

            return gameState;
        }
    }
}