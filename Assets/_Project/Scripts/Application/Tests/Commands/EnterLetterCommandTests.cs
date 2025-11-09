using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wordle.Application.Commands;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Application.Tests.Mocks;

namespace Wordle.Application.Tests.Commands
{
    [TestFixture]
    public class EnterLetterCommandTests
    {
        private MockEventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new MockEventBus();
        }

        [Test]
        public void Constructor_NullEventBus_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EnterLetterCommand(null, 'a', 0, 0));
        }

        [Test]
        public void CommandName_ReturnsCorrectName()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 0);

            Assert.AreEqual("EnterLetter", command.CommandName);
        }

        [Test]
        public void Priority_ReturnsNormal()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 0);

            Assert.AreEqual(CommandPriority.Normal, command.Priority);
        }

        [Test]
        public void CanExecute_ValidLetterAndPosition_ReturnsTrue()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 2);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_UpperCaseLetter_ReturnsTrue()
        {
            var command = new EnterLetterCommand(_eventBus, 'Z', 0, 2);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_NonLetter_ReturnsFalse()
        {
            var command = new EnterLetterCommand(_eventBus, '1', 0, 2);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_NegativePosition_ReturnsFalse()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, -1);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_PositionTooLarge_ReturnsFalse()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 5);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_PositionZero_ReturnsTrue()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 0);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_PositionFour_ReturnsTrue()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 4);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public async Task ExecuteAsync_PublishesLetterEnteredEvent()
        {
            var command = new EnterLetterCommand(_eventBus, 'h', 1, 2);

            await command.ExecuteAsync(CancellationToken.None);

            Assert.IsTrue(_eventBus.WasEventPublished<LetterEnteredEvent>());
            var evt = _eventBus.GetPublishedEvent<LetterEnteredEvent>();
            Assert.AreEqual('H', evt.Letter); // Event normalizes to uppercase
            Assert.AreEqual(1, evt.Row);
            Assert.AreEqual(2, evt.Position);
        }

        [Test]
        public void OnComplete_DoesNotThrow()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 0);

            Assert.DoesNotThrow(() => command.OnComplete());
        }

        [Test]
        public void OnFailed_DoesNotThrow()
        {
            var command = new EnterLetterCommand(_eventBus, 'a', 0, 0);

            Assert.DoesNotThrow(() => command.OnFailed(new Exception("Test exception")));
        }
    }
}
