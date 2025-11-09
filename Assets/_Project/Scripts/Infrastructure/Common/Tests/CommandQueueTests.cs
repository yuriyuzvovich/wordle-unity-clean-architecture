using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.Commands;

namespace Tests
{
    [TestFixture]
    public class CommandQueueTests
    {
        private CommandQueue _queue;

        [SetUp]
        public void SetUp()
        {
            _queue = new CommandQueue();
        }

        #region Enqueue Tests

        [Test]
        public void Enqueue_NullCommand_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _queue.Enqueue(null));
        }

        [Test]
        public void Enqueue_AddsCommandToQueue()
        {
            var command = new TestCommand(CommandPriority.Normal);

            _queue.Enqueue(command);

            Assert.AreEqual(1, _queue.TotalCommandsCount);
            Assert.IsTrue(_queue.HasCommands);
        }

        [Test]
        public void Enqueue_MultipleCommands_IncreasesCount()
        {
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));

            Assert.AreEqual(3, _queue.TotalCommandsCount);
        }

        #endregion

        #region Dequeue Tests

        [Test]
        public void Dequeue_EmptyQueue_ReturnsNull()
        {
            var command = _queue.Dequeue();

            Assert.IsNull(command);
        }

        [Test]
        public void Dequeue_ReturnsCommandInPriorityOrder()
        {
            var highPriorityCommand = new TestCommand(CommandPriority.High, "High");
            var normalPriorityCommand = new TestCommand(CommandPriority.Normal, "Normal");
            var lowPriorityCommand = new TestCommand(CommandPriority.Low, "Low");

            _queue.Enqueue(normalPriorityCommand);
            _queue.Enqueue(lowPriorityCommand);
            _queue.Enqueue(highPriorityCommand);

            var first = _queue.Dequeue();
            var second = _queue.Dequeue();
            var third = _queue.Dequeue();

            Assert.AreSame(highPriorityCommand, first);
            Assert.AreSame(normalPriorityCommand, second);
            Assert.AreSame(lowPriorityCommand, third);
        }

        [Test]
        public void Dequeue_SamePriority_ReturnsInFIFOOrder()
        {
            var command1 = new TestCommand(CommandPriority.Normal, "First");
            var command2 = new TestCommand(CommandPriority.Normal, "Second");
            var command3 = new TestCommand(CommandPriority.Normal, "Third");

            _queue.Enqueue(command1);
            _queue.Enqueue(command2);
            _queue.Enqueue(command3);

            var first = _queue.Dequeue();
            var second = _queue.Dequeue();
            var third = _queue.Dequeue();

            Assert.AreSame(command1, first);
            Assert.AreSame(command2, second);
            Assert.AreSame(command3, third);
        }

        [Test]
        public void Dequeue_DecreasesCount()
        {
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));

            Assert.AreEqual(2, _queue.TotalCommandsCount);

            _queue.Dequeue();
            Assert.AreEqual(1, _queue.TotalCommandsCount);

            _queue.Dequeue();
            Assert.AreEqual(0, _queue.TotalCommandsCount);
            Assert.IsFalse(_queue.HasCommands);
        }

        #endregion

        #region PeekOrNull Tests

        [Test]
        public void Peek_EmptyQueue_ReturnsNull()
        {
            var command = _queue.PeekOrNull();

            Assert.IsNull(command);
        }

        [Test]
        public void Peek_ReturnsHighestPriorityCommand()
        {
            var highPriorityCommand = new TestCommand(CommandPriority.High);
            var normalPriorityCommand = new TestCommand(CommandPriority.Normal);

            _queue.Enqueue(normalPriorityCommand);
            _queue.Enqueue(highPriorityCommand);

            var peeked = _queue.PeekOrNull();

            Assert.AreSame(highPriorityCommand, peeked);
        }

        [Test]
        public void Peek_DoesNotRemoveCommand()
        {
            var command = new TestCommand(CommandPriority.Normal);
            _queue.Enqueue(command);

            _queue.PeekOrNull();

            Assert.AreEqual(1, _queue.TotalCommandsCount);
            Assert.IsTrue(_queue.HasCommands);
        }

        [Test]
        public void Peek_MultipleCalls_ReturnsSameCommand()
        {
            var command = new TestCommand(CommandPriority.Normal);
            _queue.Enqueue(command);

            var peek1 = _queue.PeekOrNull();
            var peek2 = _queue.PeekOrNull();

            Assert.AreSame(peek1, peek2);
        }

        #endregion

        #region Clear Tests

        [Test]
        public void Clear_RemovesAllCommands()
        {
            _queue.Enqueue(new TestCommand(CommandPriority.High));
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));
            _queue.Enqueue(new TestCommand(CommandPriority.Low));

            _queue.Clear();

            Assert.AreEqual(0, _queue.TotalCommandsCount);
            Assert.IsFalse(_queue.HasCommands);
        }

        [Test]
        public void Clear_EmptyQueue_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _queue.Clear());
        }

        #endregion

        #region GetAllCommands Tests

        [Test]
        public void GetAllCommands_EmptyQueue_ReturnsEmptyCollection()
        {
            var commands = _queue.GetAllCommands();

            Assert.IsNotNull(commands);
            Assert.AreEqual(0, commands.Count());
        }

        [Test]
        public void GetAllCommands_ReturnsCommandsInPriorityOrder()
        {
            var highPriorityCommand = new TestCommand(CommandPriority.High);
            var normalPriorityCommand = new TestCommand(CommandPriority.Normal);
            var lowPriorityCommand = new TestCommand(CommandPriority.Low);

            _queue.Enqueue(normalPriorityCommand);
            _queue.Enqueue(lowPriorityCommand);
            _queue.Enqueue(highPriorityCommand);

            var commands = _queue.GetAllCommands().ToList();

            Assert.AreEqual(3, commands.Count);
            Assert.AreSame(highPriorityCommand, commands[0]);
            Assert.AreSame(normalPriorityCommand, commands[1]);
            Assert.AreSame(lowPriorityCommand, commands[2]);
        }

        [Test]
        public void GetAllCommands_DoesNotModifyQueue()
        {
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));

            _queue.GetAllCommands();

            Assert.AreEqual(2, _queue.TotalCommandsCount);
        }

        #endregion

        #region HasCommands Tests

        [Test]
        public void HasCommands_EmptyQueue_ReturnsFalse()
        {
            Assert.IsFalse(_queue.HasCommands);
        }

        [Test]
        public void HasCommands_AfterEnqueue_ReturnsTrue()
        {
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));

            Assert.IsTrue(_queue.HasCommands);
        }

        [Test]
        public void HasCommands_AfterDequeueAll_ReturnsFalse()
        {
            _queue.Enqueue(new TestCommand(CommandPriority.Normal));
            _queue.Dequeue();

            Assert.IsFalse(_queue.HasCommands);
        }

        #endregion

        #region Test Helper Classes

        private class TestCommand : ICommand
        {
            public string CommandName { get; }
            public CommandPriority Priority { get; }

            public TestCommand(CommandPriority priority, string name = "TestCommand")
            {
                Priority = priority;
                CommandName = name;
            }

            public bool CanExecute() => true;

            public UniTask ExecuteAsync(CancellationToken cancellationToken)
            {
                return UniTask.CompletedTask;
            }

            public void OnComplete() { }

            public void OnFailed(Exception exception) { }
        }

        #endregion
    }
}
