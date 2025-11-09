using System;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Presentation.UI.Theme;

namespace Wordle.Presentation.UI.Background
{
    public class BackgroundPresenter : IDisposable
    {
        private readonly IBackgroundView _view;
        private readonly IEventBus _eventBus;
        private readonly IThemeService _themeService;

        private Color _backgroundColor;

        public BackgroundPresenter(IBackgroundView view, IEventBus eventBus, IThemeService themeService)
        {
            _view = view;
            _eventBus = eventBus;
            _themeService = themeService;

            SubscribeToEvents();
            LoadThemeColors();
            UpdateVisuals();
        }

        private void LoadThemeColors()
        {
            var colorScheme = _themeService.CurrentColorScheme;
            _backgroundColor = colorScheme.BackgroundColor.ToUnityColor();
        }

        private void UpdateVisuals()
        {
            _view.SetBackgroundColor(_backgroundColor);
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<ThemeChangedEvent>(OnThemeChanged);
        }

        private void OnThemeChanged(ThemeChangedEvent evt)
        {
            LoadThemeColors();
            UpdateVisuals();
        }

        public void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ThemeChangedEvent>(OnThemeChanged);
            }
        }
    }
}
