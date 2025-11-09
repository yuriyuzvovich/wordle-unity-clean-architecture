using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Wordle.Application.Commands;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Core.Theme;

namespace Wordle.Presentation.UI.Theme
{
    /// <summary>
    /// Presenter for theme toggle logic.
    /// Handles theme switching, command execution, and theme coordination.
    /// </summary>
    public class ThemeTogglePresenter : IDisposable
    {
        private readonly IThemeToggleView _view;
        private readonly IEventBus _eventBus;
        private readonly IThemeService _themeService;
        private readonly SetThemeUseCase _setThemeUseCase;
        private readonly GetCurrentThemeUseCase _getCurrentThemeUseCase;

        private Color _textColor;
        private Color _backgroundColor;
        private CancellationTokenSource _cts;

        public ThemeTogglePresenter(
            IThemeToggleView view,
            IEventBus eventBus,
            IThemeService themeService,
            SetThemeUseCase setThemeUseCase,
            GetCurrentThemeUseCase getCurrentThemeUseCase)
        {
            _view = view;
            _eventBus = eventBus;
            _themeService = themeService;
            _setThemeUseCase = setThemeUseCase;
            _getCurrentThemeUseCase = getCurrentThemeUseCase;

            _cts = new CancellationTokenSource();

            SubscribeToEvents();
            LoadThemeColors();
        }

        public void Initialize()
        {
            ApplyThemeColors();
            InitializeToggle();
        }

        public void OnToggleChanged(bool isOn)
        {
            var newTheme = isOn ? ThemeType.Dark : ThemeType.Light;
            ExecuteSetThemeCommandAsync(newTheme).Forget();
        }

        private void InitializeToggle()
        {
            var currentTheme = _getCurrentThemeUseCase.Execute();
            var isDarkTheme = currentTheme.ThemeType == ThemeType.Dark;
            _view.SetToggleState(isDarkTheme);
            UpdateLabel(currentTheme.ThemeType);
        }

        private async UniTaskVoid ExecuteSetThemeCommandAsync(ThemeType themeType)
        {
            var command = new SetThemeCommand(_setThemeUseCase, themeType);

            if (!command.CanExecute())
            {
                Debug.LogWarning("ThemeTogglePresenter: SetThemeCommand cannot execute");
                return;
            }

            try
            {
                await command.ExecuteAsync(_cts.Token);
                command.OnComplete();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.LogError($"ThemeTogglePresenter: SetThemeCommand execution failed - {ex}");
                command.OnFailed(ex);
            }
        }

        private void LoadThemeColors()
        {
            var colorScheme = _themeService.CurrentColorScheme;
            _textColor = colorScheme.TextPrimaryColor.ToUnityColor();
            _backgroundColor = colorScheme.PanelColor.ToUnityColor();
        }

        private void ApplyThemeColors()
        {
            _view.SetTextColor(_textColor);
            _view.SetBackgroundColor(_backgroundColor);
        }

        private void UpdateLabel(ThemeType themeType)
        {
            var labelText = themeType == ThemeType.Dark ? "Dark" : "Light";
            _view.SetLabelText(labelText);
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<ThemeChangedEvent>(OnThemeChanged);
        }

        private void OnThemeChanged(ThemeChangedEvent evt)
        {
            var isDarkTheme = evt.ThemeType == ThemeType.Dark;
            _view.SetToggleState(isDarkTheme);
            UpdateLabel(evt.ThemeType);
            LoadThemeColors();
            ApplyThemeColors();
        }

        public void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ThemeChangedEvent>(OnThemeChanged);
            }

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}
