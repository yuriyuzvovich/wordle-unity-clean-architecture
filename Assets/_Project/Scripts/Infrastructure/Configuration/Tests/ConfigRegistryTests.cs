using System;
using NUnit.Framework;
using Wordle.Infrastructure.Configuration;

namespace Tests
{
    [TestFixture]
    public class ConfigRegistryTests
    {
        private ConfigRegistry _registry;
        private MockLogService _logService;

        [SetUp]
        public void SetUp()
        {
            _logService = new MockLogService();
            _registry = new ConfigRegistry(_logService);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithLogService_CreatesInstance()
        {
            var registry = new ConfigRegistry(_logService);
            Assert.IsNotNull(registry);
        }

        [Test]
        public void Constructor_NullLogService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ConfigRegistry(null));
        }

        #endregion

        #region Register Tests

        [Test]
        public void Register_WithType_RegistersConfig()
        {
            var config = new TestConfig(42, "test");
            _registry.Register(config);

            var retrieved = _registry.Get<TestConfig>();
            Assert.AreEqual(config, retrieved);
        }

        [Test]
        public void Register_WithKey_RegistersConfig()
        {
            var config = new TestConfig(42, "test");
            _registry.Register("custom-key", config);

            var retrieved = _registry.Get<TestConfig>("custom-key");
            Assert.AreEqual(config, retrieved);
        }

        [Test]
        public void Register_NullConfig_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _registry.Register<TestConfig>(null));
        }

        [Test]
        public void Register_NullConfigWithKey_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _registry.Register<TestConfig>("key", null));
        }

        [Test]
        public void Register_NullKey_ThrowsArgumentException()
        {
            var config = new TestConfig(42, "test");
            Assert.Throws<ArgumentException>(() => _registry.Register<TestConfig>(null, config));
        }

        [Test]
        public void Register_EmptyKey_ThrowsArgumentException()
        {
            var config = new TestConfig(42, "test");
            Assert.Throws<ArgumentException>(() => _registry.Register<TestConfig>("", config));
        }

        [Test]
        public void Register_SameTypeTwice_OverwritesConfig()
        {
            var config1 = new TestConfig(42, "first");
            var config2 = new TestConfig(99, "second");

            _registry.Register(config1);
            _registry.Register(config2);

            var retrieved = _registry.Get<TestConfig>();
            Assert.AreEqual(config2, retrieved);
            Assert.AreEqual(99, retrieved.Value);
        }

        [Test]
        public void Register_DifferentTypes_RegistersBothConfigs()
        {
            var testConfig = new TestConfig(42, "test");
            var anotherConfig = new AnotherTestConfig(true);

            _registry.Register(testConfig);
            _registry.Register(anotherConfig);

            Assert.AreEqual(testConfig, _registry.Get<TestConfig>());
            Assert.AreEqual(anotherConfig, _registry.Get<AnotherTestConfig>());
        }

        #endregion

        #region Get Tests

        [Test]
        public void Get_RegisteredConfig_ReturnsConfig()
        {
            var config = new TestConfig(42, "test");
            _registry.Register(config);

            var retrieved = _registry.Get<TestConfig>();
            Assert.AreEqual(config, retrieved);
        }

        [Test]
        public void Get_WithKey_RegisteredConfig_ReturnsConfig()
        {
            var config = new TestConfig(42, "test");
            _registry.Register("my-key", config);

            var retrieved = _registry.Get<TestConfig>("my-key");
            Assert.AreEqual(config, retrieved);
        }

        [Test]
        public void Get_NotRegistered_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _registry.Get<TestConfig>());
        }

        [Test]
        public void Get_WithKey_NotRegistered_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _registry.Get<TestConfig>("non-existent"));
        }

        [Test]
        public void Get_NullKey_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _registry.Get<TestConfig>(null));
        }

        [Test]
        public void Get_EmptyKey_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _registry.Get<TestConfig>(""));
        }

        [Test]
        public void Get_WrongType_ThrowsInvalidOperationException()
        {
            var config = new TestConfig(42, "test");
            _registry.Register("key", config);

            Assert.Throws<InvalidOperationException>(() => _registry.Get<AnotherTestConfig>("key"));
        }

        #endregion

        #region TryGet Tests

        [Test]
        public void TryGet_RegisteredConfig_ReturnsTrueAndConfig()
        {
            var config = new TestConfig(42, "test");
            _registry.Register(config);

            var success = _registry.TryGet<TestConfig>(out var retrieved);
            Assert.IsTrue(success);
            Assert.AreEqual(config, retrieved);
        }

        [Test]
        public void TryGet_WithKey_RegisteredConfig_ReturnsTrueAndConfig()
        {
            var config = new TestConfig(42, "test");
            _registry.Register("my-key", config);

            var success = _registry.TryGet<TestConfig>("my-key", out var retrieved);
            Assert.IsTrue(success);
            Assert.AreEqual(config, retrieved);
        }

        [Test]
        public void TryGet_NotRegistered_ReturnsFalseAndNull()
        {
            var success = _registry.TryGet<TestConfig>(out var retrieved);
            Assert.IsFalse(success);
            Assert.IsNull(retrieved);
        }

        [Test]
        public void TryGet_WithKey_NotRegistered_ReturnsFalseAndNull()
        {
            var success = _registry.TryGet<TestConfig>("non-existent", out var retrieved);
            Assert.IsFalse(success);
            Assert.IsNull(retrieved);
        }

        [Test]
        public void TryGet_NullKey_ReturnsFalse()
        {
            var success = _registry.TryGet<TestConfig>(null, out var retrieved);
            Assert.IsFalse(success);
            Assert.IsNull(retrieved);
        }

        [Test]
        public void TryGet_EmptyKey_ReturnsFalse()
        {
            var success = _registry.TryGet<TestConfig>("", out var retrieved);
            Assert.IsFalse(success);
            Assert.IsNull(retrieved);
        }

        [Test]
        public void TryGet_WrongType_ReturnsFalse()
        {
            var config = new TestConfig(42, "test");
            _registry.Register("key", config);

            var success = _registry.TryGet<AnotherTestConfig>("key", out var retrieved);
            Assert.IsFalse(success);
            Assert.IsNull(retrieved);
        }

        #endregion

        #region IsRegistered Tests

        [Test]
        public void IsRegistered_RegisteredConfig_ReturnsTrue()
        {
            var config = new TestConfig(42, "test");
            _registry.Register(config);

            Assert.IsTrue(_registry.IsRegistered<TestConfig>());
        }

        [Test]
        public void IsRegistered_WithKey_RegisteredConfig_ReturnsTrue()
        {
            var config = new TestConfig(42, "test");
            _registry.Register("my-key", config);

            Assert.IsTrue(_registry.IsRegistered<TestConfig>("my-key"));
        }

        [Test]
        public void IsRegistered_NotRegistered_ReturnsFalse()
        {
            Assert.IsFalse(_registry.IsRegistered<TestConfig>());
        }

        [Test]
        public void IsRegistered_WithKey_NotRegistered_ReturnsFalse()
        {
            Assert.IsFalse(_registry.IsRegistered<TestConfig>("non-existent"));
        }

        [Test]
        public void IsRegistered_NullKey_ReturnsFalse()
        {
            Assert.IsFalse(_registry.IsRegistered<TestConfig>(null));
        }

        [Test]
        public void IsRegistered_EmptyKey_ReturnsFalse()
        {
            Assert.IsFalse(_registry.IsRegistered<TestConfig>(""));
        }

        [Test]
        public void IsRegistered_WrongType_ReturnsFalse()
        {
            var config = new TestConfig(42, "test");
            _registry.Register("key", config);

            Assert.IsFalse(_registry.IsRegistered<AnotherTestConfig>("key"));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void IntegrationTest_MultipleConfigsWithMixedKeys()
        {
            var testConfig1 = new TestConfig(42, "first");
            var testConfig2 = new TestConfig(99, "second");
            var anotherConfig = new AnotherTestConfig(true);
            var projectConfig = ProjectConfigFactory.CreateDefault();

            _registry.Register(testConfig1);
            _registry.Register("custom", testConfig2);
            _registry.Register(anotherConfig);
            _registry.Register(projectConfig);

            Assert.AreEqual(testConfig1, _registry.Get<TestConfig>());
            Assert.AreEqual(testConfig2, _registry.Get<TestConfig>("custom"));
            Assert.AreEqual(anotherConfig, _registry.Get<AnotherTestConfig>());
            Assert.AreEqual(projectConfig, _registry.Get<ProjectConfig>());
        }

        #endregion

        #region Test Helper Classes

        private class TestConfig
        {
            public int Value { get; }
            public string Name { get; }

            public TestConfig(int value, string name)
            {
                Value = value;
                Name = name;
            }
        }

        private class AnotherTestConfig
        {
            public bool Enabled { get; }

            public AnotherTestConfig(bool enabled)
            {
                Enabled = enabled;
            }
        }

        #endregion
    }
}
