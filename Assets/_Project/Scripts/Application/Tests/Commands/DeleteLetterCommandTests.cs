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
    public class DeleteLetterCommandTests
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
            Assert.Throws<ArgumentNullException>(() => new DeleteLetterCommand(null, 0, 0));
        }

        [Test]
        public void CommandName_ReturnsCorrectName()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 0);

            Assert.AreEqual("DeleteLetter", command.CommandName);
        }

        [Test]
        public void Priority_ReturnsNormal()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 0);

            Assert.AreEqual(CommandPriority.Normal, command.Priority);
        }

        [Test]
        public void CanExecute_ValidPosition_ReturnsTrue()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 2);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_NegativePosition_ReturnsFalse()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, -1);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_PositionTooLarge_ReturnsFalse()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 5);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_PositionZero_ReturnsTrue()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 0);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_PositionFour_ReturnsTrue()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 4);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public async Task ExecuteAsync_PublishesLetterDeletedEvent()
        {
            var command = new DeleteLetterCommand(_eventBus, 1, 3);

            await command.ExecuteAsync(CancellationToken.None);

            Assert.IsTrue(_eventBus.WasEventPublished<LetterDeletedEvent>());
            var evt = _eventBus.GetPublishedEvent<LetterDeletedEvent>();
            Assert.AreEqual(1, evt.Row);
            Assert.AreEqual(3, evt.Position);
        }

        [Test]
        public void OnComplete_DoesNotThrow()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 0);

            Assert.DoesNotThrow(() => command.OnComplete());
        }

        [Test]
        public void OnFailed_DoesNotThrow()
        {
            var command = new DeleteLetterCommand(_eventBus, 0, 0);

            Assert.DoesNotThrow(() => command.OnFailed(new Exception("Test exception")));
        }
    }
}
