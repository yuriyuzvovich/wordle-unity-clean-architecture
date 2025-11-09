using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Infrastructure.Common.DI;

namespace Wordle.Presentation.UI.Theme
{
    /// <summary>
    /// UI component for theme toggle.
    /// Implements MVP pattern - delegates logic to ThemeTogglePresenter.
    /// </summary>
    public class ThemeToggleView : InjectableMonoBehaviour, IThemeToggleView
    {
        [Header("UI References")]
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _label;

        [Inject] private IEventBus _eventBus;
        [Inject] private IThemeService _themeService;
        [Inject] private SetThemeUseCase _setThemeUseCase;
        [Inject] private GetCurrentThemeUseCase _getCurrentThemeUseCase;

        private ThemeTogglePresenter _presenter;

        protected override void Awake()
        {
            base.Awake();

            if (!_toggle)
            {
                Debug.LogError("ThemeToggleView: Toggle component not assigned");
                return;
            }

            _presenter = new ThemeTogglePresenter(
                this,
                _eventBus,
                _themeService,
                _setThemeUseCase,
                _getCurrentThemeUseCase);
            _presenter.Initialize();

            _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool isOn)
        {
            _presenter?.OnToggleChanged(isOn);
        }

        public void SetToggleState(bool isOn)
        {
            if (_toggle)
            {
                _toggle.SetIsOnWithoutNotify(isOn);
            }
        }

        public void SetLabelText(string text)
        {
            if (_label)
            {
                _label.text = text;
            }
        }

        public void SetTextColor(Color color)
        {
            if (_label)
            {
                _label.color = color;
            }
        }

        public void SetBackgroundColor(Color color)
        {
            if (!_toggle) return;

            var toggleBackground = _toggle.targetGraphic;
            if (toggleBackground)
            {
                toggleBackground.color = color;
            }

            var checkmark = _toggle.graphic;
            if (checkmark)
            {
                checkmark.color = color;
            }
        }

        private void OnDestroy()
        {
            if (_toggle)
            {
                _toggle.onValueChanged.RemoveListener(OnToggleChanged);
            }

            _presenter?.Dispose();
        }
    }
}
