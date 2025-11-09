using NUnit.Framework;
using Wordle.Application.Interfaces.Pooling;
using Wordle.Infrastructure.Common.Pooling;
using Wordle.Infrastructure.Pooling;

namespace Tests
{
    [TestFixture]
    public class ObjectPoolTests
    {
        private class TestPoolable : IPoolable
        {
            public bool SpawnCalled { get; private set; }
            public bool ReturnCalled { get; private set; }

            public void OnSpawnFromPool()
            {
                SpawnCalled = true;
                ReturnCalled = false;
            }

            public void OnReturnToPool()
            {
                ReturnCalled = true;
                SpawnCalled = false;
            }

            public void Reset()
            {
                SpawnCalled = false;
                ReturnCalled = false;
            }
        }

        [Test]
        public void Get_EmptyPool_CreatesNewObject()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable());

            var item = pool.Get();

            Assert.IsNotNull(item);
            Assert.AreEqual(1, pool.TotalCount);
        }

        [Test]
        public void Get_CallsOnSpawnFromPool()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable());

            var item = pool.Get();

            Assert.IsTrue(item.SpawnCalled);
        }

        [Test]
        public void Return_AddsObjectToPool()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable());
            var item = pool.Get();

            pool.Return(item);

            Assert.AreEqual(1, pool.AvailableCount);
        }

        [Test]
        public void Return_CallsOnReturnToPool()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable());
            var item = pool.Get();

            pool.Return(item);

            Assert.IsTrue(item.ReturnCalled);
        }

        [Test]
        public void Get_ReusesReturnedObject()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable());
            var item1 = pool.Get();
            pool.Return(item1);

            var item2 = pool.Get();

            Assert.AreSame(item1, item2);
            Assert.AreEqual(1, pool.TotalCount);
        }

        [Test]
        public void InitialSize_CreatesObjectsUpfront()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable(), initialSize: 5);

            Assert.AreEqual(5, pool.AvailableCount);
            Assert.AreEqual(5, pool.TotalCount);
        }

        [Test]
        public void MaxSize_LimitsPoolSize()
        {
            var destroyCount = 0;
            var pool = new ObjectPool<TestPoolable>(
                () => new TestPoolable(),
                onDestroy: _ => destroyCount++,
                maxSize: 2
            );

            var item1 = pool.Get();
            var item2 = pool.Get();
            var item3 = pool.Get();

            pool.Return(item1);
            pool.Return(item2);
            pool.Return(item3);

            Assert.AreEqual(2, pool.AvailableCount);
            Assert.AreEqual(2, pool.TotalCount);
            Assert.AreEqual(1, destroyCount);
        }

        [Test]
        public void Clear_RemovesAllObjects()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable(), initialSize: 5);

            pool.Clear();

            Assert.AreEqual(0, pool.AvailableCount);
            Assert.AreEqual(0, pool.TotalCount);
        }

        [Test]
        public void OnGet_CallbackIsInvoked()
        {
            bool getCalled = false;
            var pool = new ObjectPool<TestPoolable>(
                () => new TestPoolable(),
                onGet: _ => getCalled = true
            );

            pool.Get();

            Assert.IsTrue(getCalled);
        }

        [Test]
        public void OnReturn_CallbackIsInvoked()
        {
            bool returnCalled = false;
            var pool = new ObjectPool<TestPoolable>(
                () => new TestPoolable(),
                onReturn: _ => returnCalled = true
            );

            var item = pool.Get();
            pool.Return(item);

            Assert.IsTrue(returnCalled);
        }

        [Test]
        public void Return_NullObject_DoesNotThrow()
        {
            var pool = new ObjectPool<TestPoolable>(() => new TestPoolable());

            Assert.DoesNotThrow(() => pool.Return(null));
            Assert.AreEqual(0, pool.AvailableCount);
        }
    }
}
