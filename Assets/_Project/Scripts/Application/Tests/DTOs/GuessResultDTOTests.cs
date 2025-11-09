using NUnit.Framework;
using Wordle.Application.DTOs;
using Wordle.Core.Entities;

namespace Wordle.Application.Tests.DTOs
{
    [TestFixture]
    public class GuessResultDTOTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesDTO()
        {
            var guessDTO = CreateGuessDTO();

            var dto = new GuessResultDTO(
                true,
                "Success",
                guessDTO,
                true,
                GameStatus.Won,
                5);

            Assert.IsTrue(dto.Success);
            Assert.AreEqual("Success", dto.Message);
            Assert.AreEqual("HELLO", dto.Guess.Word);
            Assert.IsTrue(dto.IsCorrect);
            Assert.AreEqual(GameStatus.Won, dto.GameStatus);
            Assert.AreEqual(5, dto.AttemptsRemaining);
        }

        [Test]
        public void CreateSuccess_ValidParameters_CreatesSuccessDTO()
        {
            var guessDTO = CreateGuessDTO();

            var dto = GuessResultDTO.CreateSuccess(guessDTO, true, GameStatus.Won, 5);

            Assert.IsTrue(dto.Success);
            Assert.AreEqual("Guess submitted successfully", dto.Message);
            Assert.IsTrue(dto.IsCorrect);
            Assert.AreEqual(GameStatus.Won, dto.GameStatus);
            Assert.AreEqual(5, dto.AttemptsRemaining);
        }

        [Test]
        public void CreateSuccess_IncorrectGuess_CreatesSuccessDTOWithIsCorrectFalse()
        {
            var guessDTO = CreateGuessDTO();

            var dto = GuessResultDTO.CreateSuccess(guessDTO, false, GameStatus.Playing, 4);

            Assert.IsTrue(dto.Success);
            Assert.IsFalse(dto.IsCorrect);
            Assert.AreEqual(GameStatus.Playing, dto.GameStatus);
            Assert.AreEqual(4, dto.AttemptsRemaining);
        }

        [Test]
        public void CreateFailure_ValidMessage_CreatesFailureDTO()
        {
            var dto = GuessResultDTO.CreateFailure("Invalid word");

            Assert.IsFalse(dto.Success);
            Assert.AreEqual("Invalid word", dto.Message);
            Assert.IsFalse(dto.IsCorrect);
            Assert.AreEqual(GameStatus.Playing, dto.GameStatus);
            Assert.AreEqual(0, dto.AttemptsRemaining);
        }

        [Test]
        public void CreateFailure_EmptyMessage_CreatesFailureDTOWithMessage()
        {
            var dto = GuessResultDTO.CreateFailure("");

            Assert.IsFalse(dto.Success);
            Assert.AreEqual("", dto.Message);
        }

        private GuessDTO CreateGuessDTO()
        {
            var evaluations = new LetterEvaluationDTO[5];
            for (int i = 0; i < 5; i++)
            {
                evaluations[i] = new LetterEvaluationDTO('h', i, "Correct");
            }
            return new GuessDTO("HELLO", evaluations, System.DateTime.UtcNow);
        }
    }
}
