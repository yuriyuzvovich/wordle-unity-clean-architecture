using NUnit.Framework;
using UnityEngine;
using Wordle.Infrastructure.Persistence;

namespace Tests
{
    [TestFixture]
    public class PlayerPrefsLocalStorageServiceTests
    {
        private PlayerPrefsLocalStorageService _service;
        private const string TEST_KEY = "TestKey";

        [SetUp]
        public void SetUp()
        {
            _service = new PlayerPrefsLocalStorageService();
            PlayerPrefs.DeleteKey(TEST_KEY);
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(TEST_KEY);
            PlayerPrefs.Save();
        }

        #region HasKey Tests

        [Test]
        public void HasKey_KeyDoesNotExist_ReturnsFalse()
        {
            var result = _service.HasKey(TEST_KEY);

            Assert.IsFalse(result);
        }

        [Test]
        public void HasKey_KeyExists_ReturnsTrue()
        {
            PlayerPrefs.SetString(TEST_KEY, "test");

            var result = _service.HasKey(TEST_KEY);

            Assert.IsTrue(result);
        }

        #endregion

        #region GetString Tests

        [Test]
        public void GetString_KeyDoesNotExist_ReturnsEmptyString()
        {
            var result = _service.GetString(TEST_KEY);

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetString_KeyExists_ReturnsValue()
        {
            const string expectedValue = "TestValue";
            PlayerPrefs.SetString(TEST_KEY, expectedValue);

            var result = _service.GetString(TEST_KEY);

            Assert.AreEqual(expectedValue, result);
        }

        #endregion

        #region SetString Tests

        [Test]
        public void SetString_SetsValueCorrectly()
        {
            const string expectedValue = "NewValue";

            _service.SetString(TEST_KEY, expectedValue);

            var actualValue = PlayerPrefs.GetString(TEST_KEY);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void SetString_OverwritesExistingValue()
        {
            const string oldValue = "OldValue";
            const string newValue = "NewValue";
            PlayerPrefs.SetString(TEST_KEY, oldValue);

            _service.SetString(TEST_KEY, newValue);

            var actualValue = PlayerPrefs.GetString(TEST_KEY);
            Assert.AreEqual(newValue, actualValue);
        }

        #endregion

        #region DeleteKey Tests

        [Test]
        public void DeleteKey_RemovesKey()
        {
            PlayerPrefs.SetString(TEST_KEY, "test");

            _service.DeleteKey(TEST_KEY);

            Assert.IsFalse(PlayerPrefs.HasKey(TEST_KEY));
        }

        [Test]
        public void DeleteKey_KeyDoesNotExist_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.DeleteKey(TEST_KEY));
        }

        #endregion

        #region Save Tests

        [Test]
        public void Save_PersistsChanges()
        {
            const string testValue = "TestValue";

            _service.SetString(TEST_KEY, testValue);
            _service.Save();

            var result = PlayerPrefs.GetString(TEST_KEY);
            Assert.AreEqual(testValue, result);
        }

        #endregion
    }
}
