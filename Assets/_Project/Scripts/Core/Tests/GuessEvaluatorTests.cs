using NUnit.Framework;
using Wordle.Core.Entities;
using Wordle.Core.Services;
using Wordle.Core.ValueObjects;

namespace Tests
{
    [TestFixture]
    public class GuessEvaluatorTests
    {
        private GuessEvaluator _evaluator;

        [SetUp]
        public void SetUp()
        {
            _evaluator = new GuessEvaluator(new GameRules());
        }

        [Test]
        public void Evaluate_AllLettersCorrect_ReturnsAllCorrect()
        {
            var guess = new Word("hello");
            var target = new Word("hello");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(Word.WORD_LENGTH, result.Length);
            for (int i = 0; i < Word.WORD_LENGTH; i++)
            {
                Assert.AreEqual(LetterEvaluation.Correct, result[i].Evaluation);
                Assert.AreEqual(guess[i], result[i].Letter);
                Assert.AreEqual(i, result[i].Position);
            }
        }

        [Test]
        public void Evaluate_AllLettersAbsent_ReturnsAllAbsent()
        {
            var guess = new Word("world");
            var target = new Word("hexes");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(Word.WORD_LENGTH, result.Length);
            for (int i = 0; i < Word.WORD_LENGTH; i++)
            {
                Assert.AreEqual(LetterEvaluation.Absent, result[i].Evaluation);
            }
        }

        [Test]
        public void Evaluate_MixedEvaluations_ReturnsCorrectResults()
        {
            var guess = new Word("world");
            var target = new Word("wager");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Correct, result[0].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[1].Evaluation);
            Assert.AreEqual(LetterEvaluation.Present, result[2].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[3].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[4].Evaluation);
        }

        [Test]
        public void Evaluate_DuplicateLetters_SingleOccurrenceInTarget_MarksOnlyOne()
        {
            var guess = new Word("spell");
            var target = new Word("while");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Absent, result[0].Evaluation);   // s not in target
            Assert.AreEqual(LetterEvaluation.Absent, result[1].Evaluation);   // p not in target
            Assert.AreEqual(LetterEvaluation.Present, result[2].Evaluation);  // e exists at position 4
            Assert.AreEqual(LetterEvaluation.Correct, result[3].Evaluation);  // l matches l at position 3
            Assert.AreEqual(LetterEvaluation.Absent, result[4].Evaluation);   // l already used (only 1 l in target)
        }

        [Test]
        public void Evaluate_DuplicateLetters_BothInTarget_MarksCorrectly()
        {
            var guess = new Word("speed");
            var target = new Word("erase");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Present, result[0].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[1].Evaluation);
            Assert.AreEqual(LetterEvaluation.Present, result[2].Evaluation);
            Assert.AreEqual(LetterEvaluation.Present, result[3].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[4].Evaluation);
        }

        [Test]
        public void Evaluate_DuplicateLetters_PrioritizesCorrectPosition()
        {
            var guess = new Word("geese");
            var target = new Word("speed");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Absent, result[0].Evaluation);   // g not in target
            Assert.AreEqual(LetterEvaluation.Present, result[1].Evaluation);  // e exists at positions 2,3
            Assert.AreEqual(LetterEvaluation.Correct, result[2].Evaluation);  // e matches e at position 2
            Assert.AreEqual(LetterEvaluation.Present, result[3].Evaluation);  // s exists at position 0
            Assert.AreEqual(LetterEvaluation.Absent, result[4].Evaluation);   // e already used (2 e's in target, all consumed)
        }

        [Test]
        public void Evaluate_TripleLetters_OnlyOneInTarget_MarksOnlyOne()
        {
            var guess = new Word("seems");
            var target = new Word("white");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Absent, result[0].Evaluation);
            Assert.AreEqual(LetterEvaluation.Present, result[1].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[2].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[3].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[4].Evaluation);
        }

        [Test]
        public void Evaluate_TripleLetters_TwoInTarget_MarksTwo()
        {
            var guess = new Word("eeeee");
            var target = new Word("geese");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Absent, result[0].Evaluation);   // e doesn't match g, and all 3 target e's are used
            Assert.AreEqual(LetterEvaluation.Correct, result[1].Evaluation);  // e matches e at position 1
            Assert.AreEqual(LetterEvaluation.Correct, result[2].Evaluation);  // e matches e at position 2
            Assert.AreEqual(LetterEvaluation.Absent, result[3].Evaluation);   // e doesn't match s, and all 3 target e's are used
            Assert.AreEqual(LetterEvaluation.Correct, result[4].Evaluation);  // e matches e at position 4
        }

        [Test]
        public void Evaluate_ComplexDuplicateScenario_MarksCorrectly()
        {
            var guess = new Word("llama");
            var target = new Word("label");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Correct, result[0].Evaluation);  // l matches l
            Assert.AreEqual(LetterEvaluation.Present, result[1].Evaluation);  // l exists at position 4
            Assert.AreEqual(LetterEvaluation.Present, result[2].Evaluation);  // a exists at position 1
            Assert.AreEqual(LetterEvaluation.Absent, result[3].Evaluation);   // m not in target
            Assert.AreEqual(LetterEvaluation.Absent, result[4].Evaluation);   // a already used
        }

        [Test]
        public void Evaluate_SameLetterMultipleTimes_TargetHasOne_MarksCorrectOnly()
        {
            var guess = new Word("robot");
            var target = new Word("floor");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Present, result[0].Evaluation);  // r exists at position 4
            Assert.AreEqual(LetterEvaluation.Present, result[1].Evaluation);  // o exists at positions 2,3
            Assert.AreEqual(LetterEvaluation.Absent, result[2].Evaluation);   // b not in target
            Assert.AreEqual(LetterEvaluation.Correct, result[3].Evaluation);  // o matches o at position 3
            Assert.AreEqual(LetterEvaluation.Absent, result[4].Evaluation);   // t not in target
        }

        [Test]
        public void Evaluate_AllSameLetter_TargetHasDifferentLetters_MarksCorrectly()
        {
            var guess = new Word("eeeee");
            var target = new Word("taste");

            var result = _evaluator.Evaluate(guess, target);

            Assert.AreEqual(LetterEvaluation.Absent, result[0].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[1].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[2].Evaluation);
            Assert.AreEqual(LetterEvaluation.Absent, result[3].Evaluation);
            Assert.AreEqual(LetterEvaluation.Correct, result[4].Evaluation);
        }
    }
}
