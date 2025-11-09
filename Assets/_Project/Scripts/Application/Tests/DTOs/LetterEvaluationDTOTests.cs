using NUnit.Framework;
using Wordle.Application.DTOs;
using Wordle.Core.ValueObjects;

namespace Wordle.Application.Tests.DTOs
{
    [TestFixture]
    public class LetterEvaluationDTOTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesDTO()
        {
            var dto = new LetterEvaluationDTO('a', 2, "Correct");

            Assert.AreEqual('a', dto.Letter);
            Assert.AreEqual(2, dto.Position);
            Assert.AreEqual("Correct", dto.Evaluation);
        }

        [Test]
        public void FromValueObject_CorrectEvaluation_CreatesCorrectDTO()
        {
            var letterPosition = new LetterPosition('h', 0, LetterEvaluation.Correct);

            var dto = LetterEvaluationDTO.FromValueObject(letterPosition);

            Assert.AreEqual('H', dto.Letter);
            Assert.AreEqual(0, dto.Position);
            Assert.AreEqual("Correct", dto.Evaluation);
        }

        [Test]
        public void FromValueObject_PresentEvaluation_CreatesCorrectDTO()
        {
            var letterPosition = new LetterPosition('e', 1, LetterEvaluation.Present);

            var dto = LetterEvaluationDTO.FromValueObject(letterPosition);

            Assert.AreEqual('E', dto.Letter);
            Assert.AreEqual(1, dto.Position);
            Assert.AreEqual("Present", dto.Evaluation);
        }

        [Test]
        public void FromValueObject_AbsentEvaluation_CreatesCorrectDTO()
        {
            var letterPosition = new LetterPosition('z', 4, LetterEvaluation.Absent);

            var dto = LetterEvaluationDTO.FromValueObject(letterPosition);

            Assert.AreEqual('Z', dto.Letter);
            Assert.AreEqual(4, dto.Position);
            Assert.AreEqual("Absent", dto.Evaluation);
        }
    }
}
