using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Wordle.Infrastructure.Common.Events;

namespace Tests
{
    [TestFixture]
    public class EventBusTests
    {
        private EventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
        }

        #region Subscribe Tests

        [Test]
        public void Subscribe_SyncHandler_NullHandler_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _eventBus.Subscribe<TestEvent>(null));
        }

        [Test]
        public void Subscribe_AsyncHandler_NullHandler_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _eventBus.Subscribe<TestEvent>((Func<TestEvent, UniTask>)null));
        }

        [Test]
        public void Subscribe_SyncHandler_CanSubscribeMultipleHandlers()
        {
            var callCount = 0;
            Action<TestEvent> handler1 = e => callCount++;
            Action<TestEvent> handler2 = e => callCount++;

            _eventBus.Subscribe(handler1);
            _eventBus.Subscribe(handler2);

            _eventBus.Publish(new TestEvent());

            Assert.AreEqual(2, callCount);
        }

        [Test]
        public async Task Subscribe_AsyncHandler_CanSubscribeMultipleHandlers()
        {
            var callCount = 0;
            Func<TestEvent, UniTask> handler1 = async e => { callCount++; await UniTask.Yield(); };
            Func<TestEvent, UniTask> handler2 = async e => { callCount++; await UniTask.Yield(); };

            _eventBus.Subscribe(handler1);
            _eventBus.Subscribe(handler2);

            await _eventBus.PublishAsync(new TestEvent());

            Assert.AreEqual(2, callCount);
        }

        #endregion

        #region Unsubscribe Tests

        [Test]
        public void Unsubscribe_SyncHandler_NullHandler_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _eventBus.Unsubscribe<TestEvent>(null));
        }

        [Test]
        public void Unsubscribe_AsyncHandler_NullHandler_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _eventBus.Unsubscribe<TestEvent>((Func<TestEvent, UniTask>)null));
        }

        [Test]
        public void Unsubscribe_SyncHandler_RemovesHandler()
        {
            var callCount = 0;
            Action<TestEvent> handler = e => callCount++;

            _eventBus.Subscribe(handler);
            _eventBus.Publish(new TestEvent());
            Assert.AreEqual(1, callCount);

            _eventBus.Unsubscribe(handler);
            _eventBus.Publish(new TestEvent());
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public async Task Unsubscribe_AsyncHandler_RemovesHandler()
        {
            var callCount = 0;
            Func<TestEvent, UniTask> handler = async e => { callCount++; await UniTask.Yield(); };

            _eventBus.Subscribe(handler);
            await _eventBus.PublishAsync(new TestEvent());
            Assert.AreEqual(1, callCount);

            _eventBus.Unsubscribe(handler);
            await _eventBus.PublishAsync(new TestEvent());
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Unsubscribe_SyncHandler_UnsubscribedHandler_DoesNotThrow()
        {
            Action<TestEvent> handler = e => { };

            Assert.DoesNotThrow(() => _eventBus.Unsubscribe(handler));
        }

        [Test]
        public void Unsubscribe_AsyncHandler_UnsubscribedHandler_DoesNotThrow()
        {
            Func<TestEvent, UniTask> handler = async e => await UniTask.Yield();

            Assert.DoesNotThrow(() => _eventBus.Unsubscribe(handler));
        }

        #endregion

        #region Publish Tests

        [Test]
        public void Publish_SyncHandler_InvokesHandler()
        {
            var invoked = false;
            _eventBus.Subscribe<TestEvent>(e => invoked = true);

            _eventBus.Publish(new TestEvent());

            Assert.IsTrue(invoked);
        }

        [Test]
        public void Publish_SyncHandler_PassesEventData()
        {
            TestEvent receivedEvent = default;
            var testEvent = new TestEvent { Value = 42, Message = "test" };

            _eventBus.Subscribe<TestEvent>(e => receivedEvent = e);
            _eventBus.Publish(testEvent);

            Assert.AreEqual(42, receivedEvent.Value);
            Assert.AreEqual("test", receivedEvent.Message);
        }

        [Test]
        public void Publish_NoHandlers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _eventBus.Publish(new TestEvent()));
        }

        [Test]
        public void Publish_HandlerThrowsException_DoesNotStopOtherHandlers()
        {
            var callCount = 0;
            // Register explicitly as sync handlers to avoid overlap with the async overload
            _eventBus.Subscribe<TestEvent>((Action<TestEvent>)(e => { throw new Exception("Test exception"); }));
            _eventBus.Subscribe<TestEvent>((Action<TestEvent>)(e => callCount++));

             // Accept either sync or async handler error message; handler may be registered as either overload
             LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Error in (sync|async) event handler for TestEvent:.*Test exception"));
             _eventBus.Publish(new TestEvent());

             Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Publish_MultipleHandlers_InvokesAllHandlers()
        {
            var handler1Invoked = false;
            var handler2Invoked = false;
            var handler3Invoked = false;

            _eventBus.Subscribe<TestEvent>(e => handler1Invoked = true);
            _eventBus.Subscribe<TestEvent>(e => handler2Invoked = true);
            _eventBus.Subscribe<TestEvent>(e => handler3Invoked = true);

            _eventBus.Publish(new TestEvent());

            Assert.IsTrue(handler1Invoked);
            Assert.IsTrue(handler2Invoked);
            Assert.IsTrue(handler3Invoked);
        }

        #endregion

        #region PublishAsync Tests

        [Test]
        public async Task PublishAsync_SyncHandler_InvokesHandler()
        {
            var invoked = false;
            _eventBus.Subscribe<TestEvent>(e => invoked = true);

            await _eventBus.PublishAsync(new TestEvent());

            Assert.IsTrue(invoked);
        }

        [Test]
        public async Task PublishAsync_AsyncHandler_InvokesHandler()
        {
            var invoked = false;
            _eventBus.Subscribe<TestEvent>(async e => { invoked = true; await UniTask.Yield(); });

            await _eventBus.PublishAsync(new TestEvent());

            Assert.IsTrue(invoked);
        }

        [Test]
        public async Task PublishAsync_AsyncHandler_PassesEventData()
        {
            TestEvent receivedEvent = default;
            var testEvent = new TestEvent { Value = 42, Message = "test" };

            _eventBus.Subscribe<TestEvent>(async e => { receivedEvent = e; await UniTask.Yield(); });
            await _eventBus.PublishAsync(testEvent);

            Assert.AreEqual(42, receivedEvent.Value);
            Assert.AreEqual("test", receivedEvent.Message);
        }

        [Test]
        public async Task PublishAsync_NoHandlers_DoesNotThrow()
        {
            await _eventBus.PublishAsync(new TestEvent());
            Assert.Pass();
        }

        [Test]
        public async Task PublishAsync_AsyncHandler_WaitsForCompletion()
        {
            var executionOrder = new List<int>();

            _eventBus.Subscribe<TestEvent>(async e =>
            {
                executionOrder.Add(1);
                await UniTask.Delay(50);
                executionOrder.Add(2);
            });

            await _eventBus.PublishAsync(new TestEvent());
            executionOrder.Add(3);

            Assert.AreEqual(3, executionOrder.Count);
            Assert.AreEqual(1, executionOrder[0]);
            Assert.AreEqual(2, executionOrder[1]);
            Assert.AreEqual(3, executionOrder[2]);
        }

        [Test]
        public async Task PublishAsync_MultipleAsyncHandlers_WaitsForAllHandlers()
        {
            var handler1Completed = false;
            var handler2Completed = false;

            _eventBus.Subscribe<TestEvent>(async e =>
            {
                await UniTask.Delay(50);
                handler1Completed = true;
            });

            _eventBus.Subscribe<TestEvent>(async e =>
            {
                await UniTask.Delay(30);
                handler2Completed = true;
            });

            await _eventBus.PublishAsync(new TestEvent());

            Assert.IsTrue(handler1Completed);
            Assert.IsTrue(handler2Completed);
        }

        [Test]
        public async Task PublishAsync_HandlerThrowsException_DoesNotStopOtherHandlers()
        {
            var callCount = 0;
            _eventBus.Subscribe<TestEvent>(async e =>
            {
                await UniTask.Yield();
                throw new Exception("Test exception");
            });
            _eventBus.Subscribe<TestEvent>(async e =>
            {
                await UniTask.Yield();
                callCount++;
            });

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Error in async event handler for TestEvent:.*Test exception"));
            await _eventBus.PublishAsync(new TestEvent());

            Assert.AreEqual(1, callCount);
        }

        #endregion

        #region Clear Tests

        [Test]
        public void Clear_RemovesAllHandlersForEvent()
        {
            var callCount = 0;
            _eventBus.Subscribe<TestEvent>(e => callCount++);
            _eventBus.Subscribe<TestEvent>(e => callCount++);

            _eventBus.Clear<TestEvent>();
            _eventBus.Publish(new TestEvent());

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public async Task Clear_RemovesAsyncHandlers()
        {
            var callCount = 0;
            _eventBus.Subscribe<TestEvent>(async e => { callCount++; await UniTask.Yield(); });

            _eventBus.Clear<TestEvent>();
            await _eventBus.PublishAsync(new TestEvent());

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Clear_DoesNotAffectOtherEventTypes()
        {
            var testEventCallCount = 0;
            var otherEventCallCount = 0;

            _eventBus.Subscribe<TestEvent>(e => testEventCallCount++);
            _eventBus.Subscribe<OtherTestEvent>(e => otherEventCallCount++);

            _eventBus.Clear<TestEvent>();

            _eventBus.Publish(new TestEvent());
            _eventBus.Publish(new OtherTestEvent());

            Assert.AreEqual(0, testEventCallCount);
            Assert.AreEqual(1, otherEventCallCount);
        }

        [Test]
        public void ClearAll_RemovesAllHandlers()
        {
            var testEventCallCount = 0;
            var otherEventCallCount = 0;

            _eventBus.Subscribe<TestEvent>(e => testEventCallCount++);
            _eventBus.Subscribe<OtherTestEvent>(e => otherEventCallCount++);

            _eventBus.ClearAll();

            _eventBus.Publish(new TestEvent());
            _eventBus.Publish(new OtherTestEvent());

            Assert.AreEqual(0, testEventCallCount);
            Assert.AreEqual(0, otherEventCallCount);
        }

        #endregion

        #region Test Helper Structs

        public struct TestEvent
        {
            public int Value;
            public string Message;
        }

        public struct OtherTestEvent
        {
            public string Data;
        }

        #endregion
    }
}
