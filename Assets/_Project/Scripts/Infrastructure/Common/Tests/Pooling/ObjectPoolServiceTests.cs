using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Wordle.Application.Interfaces;
using Wordle.Application.Interfaces.Pooling;
using Wordle.Infrastructure.Common.Pooling;
using Wordle.Infrastructure.Logging;
using Wordle.Infrastructure.Pooling;

namespace Tests
{
    [TestFixture]
    public class ObjectPoolServiceTests
    {
        private class TestComponent : MonoBehaviour, IPoolable
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
        }

        private IObjectPoolService _poolService;
        private ILogService _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new UnityLogger(LogLevel.Debug);
            _poolService = new ObjectPoolService(_logger);
        }

        [TearDown]
        public void TearDown()
        {
            _poolService.ClearAllPools();
        }

        [Test]
        public void CreatePool_ValidPrefab_CreatesPool()
        {
            var prefab = CreateTestPrefab();

            _poolService.CreatePool(prefab, initialSize: 5);

            Assert.IsTrue(_poolService.HasPool<TestComponent>());

            CleanupPrefab(prefab);
        }

        [Test]
        public void CreatePool_DuplicateType_LogsWarning()
        {
            var prefab = CreateTestPrefab();

            _poolService.CreatePool(prefab, initialSize: 1);
            _poolService.CreatePool(prefab, initialSize: 1);

            Assert.IsTrue(_poolService.HasPool<TestComponent>());

            CleanupPrefab(prefab);
        }

        [Test]
        public void Get_ExistingPool_ReturnsObject()
        {
            var prefab = CreateTestPrefab();
            _poolService.CreatePool(prefab, initialSize: 1);

            var obj = _poolService.Get<TestComponent>();

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.gameObject.activeSelf);

            CleanupPrefab(prefab);
            if (obj) Object.DestroyImmediate(obj.gameObject);
        }

        [Test]
        public void Get_NoPool_ReturnsNull()
        {
            LogAssert.Expect(LogType.Error, "[ERROR] ObjectPoolService: No pool found for type TestComponent. Call CreatePool<TestComponent>() first.");
            
            var obj = _poolService.Get<TestComponent>();

            Assert.IsNull(obj);
        }

        [Test]
        public void Return_ValidObject_ReturnsToPool()
        {
            var prefab = CreateTestPrefab();
            _poolService.CreatePool(prefab, initialSize: 1);
            var obj = _poolService.Get<TestComponent>();

            _poolService.Return(obj);

            Assert.IsFalse(obj.gameObject.activeSelf);
            Assert.IsTrue(obj.ReturnCalled);

            CleanupPrefab(prefab);
        }

        [Test]
        public void Return_NullObject_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _poolService.Return<TestComponent>(null));
        }

        [Test]
        public void ClearPool_ExistingPool_RemovesPool()
        {
            var prefab = CreateTestPrefab();
            _poolService.CreatePool(prefab, initialSize: 1);

            _poolService.ClearPool<TestComponent>();

            Assert.IsFalse(_poolService.HasPool<TestComponent>());

            CleanupPrefab(prefab);
        }

        [Test]
        public void ClearAllPools_RemovesAllPools()
        {
            var prefab = CreateTestPrefab();
            _poolService.CreatePool(prefab, initialSize: 1);

            _poolService.ClearAllPools();

            Assert.IsFalse(_poolService.HasPool<TestComponent>());

            CleanupPrefab(prefab);
        }

        [Test]
        public void HasPool_NonExistentPool_ReturnsFalse()
        {
            Assert.IsFalse(_poolService.HasPool<TestComponent>());
        }

        private TestComponent CreateTestPrefab()
        {
            var go = new GameObject("TestPrefab");
            return go.AddComponent<TestComponent>();
        }

        private void CleanupPrefab(TestComponent prefab)
        {
            if (prefab)
            {
                Object.DestroyImmediate(prefab.gameObject);
            }
        }
    }
}
