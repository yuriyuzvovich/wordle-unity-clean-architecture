using NUnit.Framework;
using System;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;
using Wordle.Core.Theme;

namespace Wordle.Application.Tests.UseCases
{
    [TestFixture]
    public class GetCurrentThemeUseCaseTests
    {
        private MockThemeService _themeService;
        private GetCurrentThemeUseCase _useCase;
        private MockLogService _logService;

        [SetUp]
        public void SetUp()
        {
            _themeService = new MockThemeService();
            _logService = new MockLogService();
            _useCase = new GetCurrentThemeUseCase(_themeService, _logService);
        }

        [Test]
        public void Constructor_NullThemeService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetCurrentThemeUseCase(null, _logService));
        }

        [Test]
        public void Execute_DefaultTheme_ReturnsLightTheme()
        {
            var result = _useCase.Execute();

            Assert.AreEqual(ThemeType.Light, result.ThemeType);
            Assert.IsNotNull(result.ColorScheme);
        }

        [Test]
        public void Execute_AfterThemeChange_ReturnsUpdatedTheme()
        {
            _themeService.SetTheme(ThemeType.Dark);

            var result = _useCase.Execute();

            Assert.AreEqual(ThemeType.Dark, result.ThemeType);
            Assert.IsNotNull(result.ColorScheme);
        }

        [Test]
        public void Execute_ReturnsCorrectColorScheme()
        {
            _themeService.SetTheme(ThemeType.Dark);

            var result = _useCase.Execute();

            Assert.AreEqual(ThemeType.Dark, result.ThemeType);
            Assert.AreEqual(ThemeColors.DarkTheme.TileEmptyColor, result.ColorScheme.TileEmptyColor);
            Assert.AreEqual(ThemeColors.DarkTheme.TileCorrectColor, result.ColorScheme.TileCorrectColor);
        }
    }
}