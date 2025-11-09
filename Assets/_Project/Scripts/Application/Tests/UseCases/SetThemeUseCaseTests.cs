using NUnit.Framework;
using System;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;
using Wordle.Core.Theme;

namespace Wordle.Application.Tests.UseCases
{
    [TestFixture]
    public class SetThemeUseCaseTests
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
        public void Constructor_NullThemeService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SetThemeUseCase(null, _logService));
        }

        [Test]
        public void Constructor_NullLogService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SetThemeUseCase(_themeService, null));
        }

        [Test]
        public void Execute_SetsThemeToLight_UpdatesThemeService()
        {
            _useCase.Execute(ThemeType.Light);

            Assert.AreEqual(ThemeType.Light, _themeService.CurrentTheme);
        }

        [Test]
        public void Execute_SetsThemeToDark_UpdatesThemeService()
        {
            _useCase.Execute(ThemeType.Dark);

            Assert.AreEqual(ThemeType.Dark, _themeService.CurrentTheme);
        }

        [Test]
        public void Execute_ChangesTheme_LogsInfo()
        {
            _useCase.Execute(ThemeType.Dark);

            Assert.IsTrue(_logService.InfoLogs.Count > 0);
            Assert.IsTrue(_logService.InfoLogs[0].Contains("Dark"));
        }

        [Test]
        public void Execute_ChangesTheme_UpdatesColorScheme()
        {
            _useCase.Execute(ThemeType.Dark);

            Assert.AreEqual(ThemeColors.DarkTheme.TileEmptyColor, _themeService.CurrentColorScheme.TileEmptyColor);
            Assert.AreEqual(ThemeColors.DarkTheme.TileCorrectColor, _themeService.CurrentColorScheme.TileCorrectColor);
        }
    }
}
