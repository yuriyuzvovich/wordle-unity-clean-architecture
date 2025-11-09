using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wordle.Application.Commands;
using Wordle.Application.Interfaces;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;
using Wordle.Core.Theme;

namespace Wordle.Application.Tests.Commands
{
    [TestFixture]
    public class SetThemeCommandTests
    {
        private MockThemeService _themeService;
        private MockLogService _logService;
        private SetThemeUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _themeService = new MockThemeService();
            _logService = new MockLogService();
            _useCase = new SetThemeUseCase(_themeService, _logService);
        }

        [Test]
        public void Constructor_NullUseCase_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new SetThemeCommand(null, ThemeType.Dark));
        }

        [Test]
        public void CommandName_ReturnsCorrectName()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Dark);

            Assert.AreEqual("SetTheme", command.CommandName);
        }

        [Test]
        public void Priority_ReturnsNormal()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Dark);

            Assert.AreEqual(CommandPriority.Normal, command.Priority);
        }

        [Test]
        public void CanExecute_ValidThemeType_ReturnsTrue()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Dark);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_LightTheme_ReturnsTrue()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Light);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_InvalidThemeType_ReturnsFalse()
        {
            var command = new SetThemeCommand(_useCase, (ThemeType)999);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public async Task ExecuteAsync_DarkTheme_CompletesSuccessfully()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Dark);

            await command.ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(ThemeType.Dark, _themeService.CurrentTheme);
        }

        [Test]
        public async Task ExecuteAsync_LightTheme_CompletesSuccessfully()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Light);

            await command.ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(ThemeType.Light, _themeService.CurrentTheme);
        }

        [Test]
        public void OnComplete_DoesNotThrow()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Dark);

            Assert.DoesNotThrow(() => command.OnComplete());
        }

        [Test]
        public void OnFailed_DoesNotThrow()
        {
            var command = new SetThemeCommand(_useCase, ThemeType.Dark);

            Assert.DoesNotThrow(() => command.OnFailed(new Exception("Test exception")));
        }
    }
}
