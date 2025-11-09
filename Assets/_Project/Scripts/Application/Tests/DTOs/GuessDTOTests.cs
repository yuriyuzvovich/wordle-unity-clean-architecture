using NUnit.Framework;
using System;
using Wordle.Application.DTOs;
using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;

namespace Wordle.Application.Tests.DTOs
{
    [TestFixture]
    public class GuessDTOTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesDTO()
        {
            var evaluations = new LetterEvaluationDTO[5];
            var timestamp = DateTime.UtcNow;

            var dto = new GuessDTO("HELLO", evaluations, timestamp);

            Assert.AreEqual("HELLO", dto.Word);
            Assert.AreEqual(5, dto.Evaluations.Length);
            Assert.AreEqual(timestamp, dto.Timestamp);
        }

        [Test]
        public void FromEntity_ValidGuess_CreatesDTO()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[5];
            for (int i = 0; i < 5; i++)
            {
                evaluations[i] = new LetterPosition(word[i], i, LetterEvaluation.Correct);
            }
            var guess = new Guess(word, evaluations);

            var dto = GuessDTO.FromEntity(guess);

            Assert.AreEqual("HELLO", dto.Word);
            Assert.AreEqual(5, dto.Evaluations.Length);
            Assert.That(dto.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void FromEntity_GuessWithMixedEvaluations_CreatesCorrectDTO()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[5];
            evaluations[0] = new LetterPosition('h', 0, LetterEvaluation.Correct);
            evaluations[1] = new LetterPosition('e', 1, LetterEvaluation.Present);
            evaluations[2] = new LetterPosition('l', 2, LetterEvaluation.Absent);
            evaluations[3] = new LetterPosition('l', 3, LetterEvaluation.Correct);
            evaluations[4] = new LetterPosition('o', 4, LetterEvaluation.Present);

            var guess = new Guess(word, evaluations);
            var dto = GuessDTO.FromEntity(guess);

            Assert.AreEqual("HELLO", dto.Word);
            Assert.AreEqual(5, dto.Evaluations.Length);
            Assert.AreEqual('H', dto.Evaluations[0].Letter);
            Assert.AreEqual("Correct", dto.Evaluations[0].Evaluation);
            Assert.AreEqual("Present", dto.Evaluations[1].Evaluation);
            Assert.AreEqual("Absent", dto.Evaluations[2].Evaluation);
        }
    }
}
