using NUnit.Framework;
using System;
using Wordle.Application.DTOs;
using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;

namespace Wordle.Application.Tests.DTOs
{
    [TestFixture]
    public class GameStateDTOTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesDTO()
        {
            var guesses = new GuessDTO[0];
            var startTime = DateTime.UtcNow;

            var dto = new GameStateDTO("HELLO", 6, 0, 6, GameStatus.Playing, guesses, startTime);

            Assert.AreEqual("HELLO", dto.TargetWord);
            Assert.AreEqual(6, dto.MaxAttempts);
            Assert.AreEqual(0, dto.CurrentAttempt);
            Assert.AreEqual(6, dto.AttemptsRemaining);
            Assert.AreEqual(GameStatus.Playing, dto.Status);
            Assert.AreEqual(0, dto.Guesses.Length);
            Assert.AreEqual(startTime, dto.StartTime);
        }

        [Test]
        public void FromEntity_NullGameState_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => GameStateDTO.FromEntity(null));
        }

        [Test]
        public void FromEntity_ValidGameState_CreatesDTO()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            var dto = GameStateDTO.FromEntity(gameState);

            Assert.AreEqual("HELLO", dto.TargetWord);
            Assert.AreEqual(6, dto.MaxAttempts);
            Assert.AreEqual(0, dto.CurrentAttempt);
            Assert.AreEqual(6, dto.AttemptsRemaining);
            Assert.AreEqual(GameStatus.Playing, dto.Status);
            Assert.AreEqual(0, dto.Guesses.Length);
        }

        [Test]
        public void FromEntity_GameStateWithGuesses_IncludesGuesses()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            var guessWord = new Word("world");
            var evaluations = new LetterPosition[5];
            for (int i = 0; i < 5; i++)
            {
                evaluations[i] = new LetterPosition(guessWord[i], i, LetterEvaluation.Absent);
            }
            var guess = new Guess(guessWord, evaluations);
            gameState.AddGuess(guess);

            var dto = GameStateDTO.FromEntity(gameState);

            Assert.AreEqual(1, dto.Guesses.Length);
            Assert.AreEqual("WORLD", dto.Guesses[0].Word);
            Assert.AreEqual(1, dto.CurrentAttempt);
            Assert.AreEqual(5, dto.AttemptsRemaining);
        }

        [Test]
        public void FromEntity_WonGameState_HasCorrectStatus()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            var evaluations = new LetterPosition[5];
            for (int i = 0; i < 5; i++)
            {
                evaluations[i] = new LetterPosition(targetWord[i], i, LetterEvaluation.Correct);
            }
            var guess = new Guess(targetWord, evaluations);
            gameState.AddGuess(guess);

            var dto = GameStateDTO.FromEntity(gameState);

            Assert.AreEqual(GameStatus.Won, dto.Status);
        }

        [Test]
        public void FromEntity_LostGameState_HasCorrectStatus()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            for (int i = 0; i < 6; i++)
            {
                var guessWord = new Word("world");
                var evaluations = new LetterPosition[5];
                for (int j = 0; j < 5; j++)
                {
                    evaluations[j] = new LetterPosition(guessWord[j], j, LetterEvaluation.Absent);
                }
                gameState.AddGuess(new Guess(guessWord, evaluations));
            }

            var dto = GameStateDTO.FromEntity(gameState);

            Assert.AreEqual(GameStatus.Lost, dto.Status);
            Assert.AreEqual(0, dto.AttemptsRemaining);
        }
    }
}
