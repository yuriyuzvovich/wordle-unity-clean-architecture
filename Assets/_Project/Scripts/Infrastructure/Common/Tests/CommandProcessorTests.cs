using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.Commands;
using Wordle.Infrastructure.Common.Events;

namespace Tests
{
    [TestFixture]
    public class CommandProcessorTests
    {
        private CommandProcessor _processor;
        private CommandQueue _queue;
        private MockEngineLifecycle _lifecycle;
        private EventBus _eventBus;
        private MockLogService _logService;

        [SetUp]
        public void SetUp()
        {
            _queue = new CommandQueue();
            _lifecycle = new MockEngineLifecycle();
            _eventBus = new EventBus();
            _logService = new MockLogService();
            _processor = new CommandProcessor(_queue, _lifecycle, _eventBus, _logService);
        }

        [TearDown]
        public void TearDown()
        {
            if (_processor.IsRunning)
            {
                _processor.Stop();
            }
        }

        #region Constructor Tests

        [Test]
        public void Constructor_NullQueue_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CommandProcessor(null, _lifecycle, _eventBus, _logService));
        }

        [Test]
        public void Constructor_NullLifecycle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CommandProcessor(_queue, null, _eventBus, _logService));
        }

        [Test]
        public void Constructor_NullEventBus_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new CommandProcessor(_queue, _lifecycle, null, _logService));
        }

        #endregion

        #region Start/Stop Tests

        [Test]
        public void Start_SetsIsRunningToTrue()
        {
            _processor.Start();

            Assert.IsTrue(_processor.IsRunning);
        }

        [Test]
        public void Stop_SetsIsRunningToFalse()
        {
            _processor.Start();
            _processor.Stop();

            Assert.IsFalse(_processor.IsRunning);
        }

        [Test]
        public void Start_WhenAlreadyRunning_DoesNotThrow()
        {
            _processor.Start();

            Assert.DoesNotThrow(() => _processor.Start());
        }

        [Test]
        public void Stop_WhenNotRunning_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _processor.Stop());
        }

        #endregion

        #region Command Processing Tests

        [Test]
        public async Task ProcessCommands_ExecutesQueuedCommand()
        {
            var command = new MockCommand();
            _queue.Enqueue(command);

            _processor.Start();
            _lifecycle.TriggerFrameTick();

            await UniTask.Delay(50);

            Assert.IsTrue(command.WasExecuted);
        }

        [Test]
        public async Task ProcessCommands_CallsOnComplete()
        {
            var command = new MockCommand();
            _queue.Enqueue(command);

            _processor.Start();
            _lifecycle.TriggerFrameTick();

            await UniTask.Delay(50);

            Assert.IsTrue(command.OnCompleteWasCalled);
        }

        [Test]
        public async Task ProcessCommands_CommandThrowsException_CallsOnFailed()
        {
            var exception = new Exception("Test exception");
            var command = new MockCommand { ThrowException = exception };
            _queue.Enqueue(command);

            // Expect the error log from CommandProcessor
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Command failed: MockCommand.*Test exception"));

            _processor.Start();
            _lifecycle.TriggerFrameTick();

            await UniTask.Delay(50);

            Assert.IsTrue(command.OnFailedWasCalled);
            Assert.AreSame(exception, command.ReceivedExeption);
        }

        [Test]
        public async Task ProcessCommands_CanExecuteReturnsFalse_SkipsCommand()
        {
            var command = new MockCommand { CanExecuteResult = false };
            _queue.Enqueue(command);

            _processor.Start();
            _lifecycle.TriggerFrameTick();

            await UniTask.Delay(50);

            Assert.IsFalse(command.WasExecuted);
        }

        [Test]
        public async Task ProcessCommands_MultipleCommands_ProcessesInOrder()
        {
            var command1 = new MockCommand { Name = "Command1" };
            var command2 = new MockCommand { Name = "Command2" };
            var command3 = new MockCommand { Name = "Command3" };

            _queue.Enqueue(command1);
            _queue.Enqueue(command2);
            _queue.Enqueue(command3);

            _processor.Start();
            _lifecycle.TriggerFrameTick();
            await UniTask.Delay(50);

            _lifecycle.TriggerFrameTick();
            await UniTask.Delay(50);

            _lifecycle.TriggerFrameTick();
            await UniTask.Delay(50);

            Assert.IsTrue(command1.WasExecuted);
            Assert.IsTrue(command2.WasExecuted);
            Assert.IsTrue(command3.WasExecuted);
        }

        [Test]
        public async Task ProcessCommands_NotRunning_DoesNotProcessCommands()
        {
            var command = new MockCommand();
            _queue.Enqueue(command);

            _lifecycle.TriggerFrameTick();

            await UniTask.Delay(50);

            Assert.IsFalse(command.WasExecuted);
        }

        #endregion

        #region MaxCommandsPerFrame Tests

        [Test]
        public void MaxCommandsPerFrame_DefaultValue_IsOne()
        {
            Assert.AreEqual(1, _processor.MaxCommandsPerFrame);
        }

        [Test]
        public void MaxCommandsPerFrame_CanBeSet()
        {
            _processor.MaxCommandsPerFrame = 5;

            Assert.AreEqual(5, _processor.MaxCommandsPerFrame);
        }

        [Test]
        public async Task ProcessCommands_RespectsMaxCommandsPerFrame()
        {
            _processor.MaxCommandsPerFrame = 2;

            var command1 = new MockCommand();
            var command2 = new MockCommand();
            var command3 = new MockCommand();

            _queue.Enqueue(command1);
            _queue.Enqueue(command2);
            _queue.Enqueue(command3);

            _processor.Start();
            _lifecycle.TriggerFrameTick();

            await UniTask.Delay(50);

            Assert.IsTrue(command1.WasExecuted || command2.WasExecuted);
            Assert.IsFalse(command3.WasExecuted);
        }

        #endregion

        #region Stop Behavior Tests

        [Test]
        public async Task Stop_CancelsRunningCommand()
        {
            var command = new MockCommand { ExecutionDelay = 200 };
            _queue.Enqueue(command);

            _processor.Start();
            _lifecycle.TriggerFrameTick();

            await UniTask.Delay(50);

            _processor.Stop();

            await UniTask.Delay(200);

            Assert.IsTrue(command.WasCancelled);
        }

        #endregion

        #region Test Helper Classes

        private class MockLogService : ILogService
        {
            public void LogDebug(string message) { }
            public void LogInfo(string message) { }
            public void LogWarning(string message) { }
            public void LogError(string message)
            {
                // Forward to Unity's logging so LogAssert in tests can observe the error
                UnityEngine.Debug.LogError(message);
            }

            public void LogException(string message, Exception exception)
            {
                // Include the message and exception in Unity's log for the tests
                if (exception != null)
                {
                    UnityEngine.Debug.LogError($"{message} - {exception}");
                    UnityEngine.Debug.LogException(exception);
                }
                else
                {
                    UnityEngine.Debug.LogError(message);
                }
            }
            public void SetLogLevel(LogLevel level) { }
        }

        private class MockEngineLifecycle : IEngineLifecycle
        {
            public event Action OnFrameTick;
            public event Action OnFixedFrameTick;
            public event Action OnLateFrameTick;
            public event Action<bool> OnApplicationPaused;
            public event Action<bool> OnApplicationFocused;
            public event Action OnApplicationQuitted;

            public bool IsRunning => true;
            public float DeltaTime => 0.016f;
            public float FixedDeltaTime => 0.02f;
            public int FrameCount => 0;

            public void TriggerFrameTick()
            {
                OnFrameTick?.Invoke();
            }

            public void TriggerFixedFrameTick()
            {
                OnFixedFrameTick?.Invoke();
            }

            public void TriggerLateFrameTick()
            {
                OnLateFrameTick?.Invoke();
            }
        }

        private class MockCommand : ICommand
        {
            public string Name { get; set; } = "MockCommand";
            public string CommandName => Name;
            public CommandPriority Priority { get; set; } = CommandPriority.Normal;

            public bool CanExecuteResult { get; set; } = true;
            public int ExecutionDelay { get; set; } = 10;
            public Exception ThrowException { get; set; }

            public bool WasExecuted { get; private set; }
            public bool OnCompleteWasCalled { get; private set; }
            public bool OnFailedWasCalled { get; private set; }
            public bool WasCancelled { get; private set; }
            public Exception ReceivedExeption { get; private set; }

            public bool CanExecute() => CanExecuteResult;

            public async UniTask ExecuteAsync(CancellationToken cancellationToken)
            {
                WasExecuted = true;

                try
                {
                    await UniTask.Delay(ExecutionDelay, cancellationToken: cancellationToken);

                    if (ThrowException != null)
                    {
                        throw ThrowException;
                    }
                }
                catch (OperationCanceledException)
                {
                    WasCancelled = true;
                    throw;
                }
            }

            public void OnComplete()
            {
                OnCompleteWasCalled = true;
            }

            public void OnFailed(Exception exception)
            {
                OnFailedWasCalled = true;
                ReceivedExeption = exception;
            }
        }

        #endregion
    }
}
