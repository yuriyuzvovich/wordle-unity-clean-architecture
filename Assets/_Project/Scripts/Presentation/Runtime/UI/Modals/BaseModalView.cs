using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wordle.Application.Attributes;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.DI;
using Wordle.Presentation.UI.Theme;

namespace Wordle.Presentation.UI.Modals
{
    public class BaseModalView : InjectableMonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _modalPanel;
        [SerializeField] private Image _panelBackground;
        [SerializeField] private Image _overlayBackground;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private float _scaleDuration = 0.4f;
        [SerializeField] private Ease _scaleEase = Ease.OutBack;

        [Inject] protected IEventBus _eventBus;
        [Inject] protected IThemeService _themeService;

        private Sequence _showSequence;
        private Sequence _hideSequence;

        protected override void Awake()
        {
            base.Awake();
            if (!_canvasGroup) throw new System.Exception("WinModalView: CanvasGroup reference is missing.");
            if (!_canvas) throw new System.Exception("WinModalView: Canvas reference is missing.");

            ApplyThemeColors();
            _eventBus.Subscribe<ThemeChangedEvent>(OnThemeChanged);

            AfterAwake();
        }

        protected virtual void AfterAwake()
        {

        }

        private void OnThemeChanged(ThemeChangedEvent evt)
        {
            ApplyThemeColors();
        }

        protected virtual void ApplyThemeColors()
        {
            var colorScheme = _themeService.CurrentColorScheme;

            if (_panelBackground)
            {
                _panelBackground.color = colorScheme.PanelColor.ToUnityColor();
            }

            if (_overlayBackground)
            {
                var overlayColor = colorScheme.BackgroundColor.ToUnityColor();
                overlayColor.a = 0.8f;
                _overlayBackground.color = overlayColor;
            }

            ApplyTextColors(colorScheme);
        }

        protected virtual void ApplyTextColors(Core.Theme.ColorScheme colorScheme)
        {
        }

        public void Show()
        {
            _canvas.enabled = true;

            _hideSequence?.Kill();

            _showSequence = DOTween.Sequence();

            if (_canvasGroup)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                _showSequence.Append(_canvasGroup.DOFade(1f, _fadeDuration));
            }

            if (_modalPanel)
            {
                _modalPanel.localScale = Vector3.zero;
                _showSequence.Join(_modalPanel.DOScale(Vector3.one, _scaleDuration).SetEase(_scaleEase));
            }
        }

        public void Hide(bool immediate = false)
        {
            if (immediate)
            {
                SetActive(false);
                return;
            }

            _showSequence?.Kill();

            _hideSequence = DOTween.Sequence();

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _hideSequence.Append(_canvasGroup.DOFade(0f, _fadeDuration));

            if (_modalPanel)
            {
                _hideSequence.Join(_modalPanel.DOScale(Vector3.zero, _scaleDuration).SetEase(Ease.InBack));
            }

            _hideSequence.OnComplete(() => SetActive(false));
        }

        private void SetActive(bool isActive)
        {
            _canvas.enabled = isActive;
            _canvasGroup.interactable = isActive;
            _canvasGroup.blocksRaycasts = isActive;
            _modalPanel.localScale = isActive ? Vector3.one : Vector3.zero;
        }

        protected void KillSequences()
        {
            _showSequence?.Kill();
            _hideSequence?.Kill();
        }

        protected virtual void OnDestroy()
        {
            KillSequences();

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ThemeChangedEvent>(OnThemeChanged);
            }
            
            AfterDestroy();
        }

        protected virtual void AfterDestroy()
        {
        }
    }
}