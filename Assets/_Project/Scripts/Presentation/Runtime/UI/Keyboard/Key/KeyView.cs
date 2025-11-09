using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Application.Interfaces.Pooling;
using Wordle.Core.ValueObjects;
using Wordle.Infrastructure.Common.DI;

namespace Wordle.Presentation.UI.Keyboard
{
    /// <summary>
    /// Visual component for a single keyboard key.
    /// Implements MVP pattern - delegates logic to KeyPresenter.
    /// </summary>
    public class KeyView : InjectableMonoBehaviour, IKeyView, IPoolable
    {
        [Header("Key Configuration")]
        [SerializeField] private string _keyLabel;
        [SerializeField] private KeyType _keyType = KeyType.Letter;

        [Header("Visual Components")]
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _button;
        [SerializeField] private LayoutElement _layoutElement;

        [Header("Animation Settings")]
        [SerializeField] private float _pressDuration = 0.1f;
        [SerializeField] private float _pressScale = 0.95f;

        [Inject] private IEventBus _eventBus;
        [Inject] private IThemeService _themeService;

        private KeyPresenter _presenter;
        private Sequence _currentAnimation;
        private Transform _transformCache;

        protected override void Awake()
        {
            base.Awake();

            if (!_backgroundImage) throw new System.Exception($"KeyView [{_keyLabel}]: Background Image component not assigned");
            if (!_button) throw new System.Exception($"KeyView [{_keyLabel}]: Button component not assigned");
            if (!_keyText) throw new System.Exception($"KeyView [{_keyLabel}]: Key Text component not assigned");
            if (!_layoutElement) throw new System.Exception($"KeyView [{_keyLabel}]: LayoutElement component not assigned");

            _transformCache = transform;

            _presenter = new KeyPresenter(this, _eventBus, _themeService);
            SetupButton();
        }

        public void Initialize(string keyLabel, KeyType keyType, System.Action<string> onKeyPressed)
        {
            _keyLabel = keyLabel;
            _keyType = keyType;
            _presenter.Initialize(keyLabel, keyType, onKeyPressed);

            if (keyType == KeyType.Enter)
            {
                _layoutElement.minWidth = Screen.width * 0.15f;
            }
        }

        private void OnDestroy()
        {
            KillCurrentAnimation();

            if (_button)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }

            _presenter?.Dispose();
        }

        public void SetLocalizedTexts(string enterText, string backspaceText)
        {
            _presenter.SetLocalizedTexts(enterText, backspaceText);
        }

        private void SetupButton()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _presenter.OnButtonClicked();
        }

        public void SetKeyState(LetterEvaluation evaluation)
        {
            _presenter.SetKeyState(evaluation);
        }

        public void ResetKeyState()
        {
            _presenter.ResetKeyState();
        }

        public void SetText(string text)
        {
            _keyText.text = text;
        }

        public void SetBackgroundColor(Color color)
        {
            _backgroundImage.DOColor(color, 0.2f).SetEase(Ease.OutQuad);
        }

        public void SetTextColor(Color color)
        {
            _keyText.color = color;
        }

        public void PlayPressAnimation()
        {
            KillCurrentAnimation();

            if (!_transformCache) return;

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_transformCache.DOScale(_pressScale, _pressDuration / 2f).SetEase(Ease.OutQuad));
            _currentAnimation.Append(_transformCache.DOScale(1f, _pressDuration / 2f).SetEase(Ease.InQuad));
            _currentAnimation.OnComplete(() => _currentAnimation = null);
        }

        public void ResetVisuals()
        {
            KillCurrentAnimation();
            if (_transformCache)
            {
                _transformCache.localScale = Vector3.one;
            }
        }

        private void KillCurrentAnimation()
        {
            if (_currentAnimation != null && _currentAnimation.IsActive())
            {
                _currentAnimation.Kill();
                _currentAnimation = null;
            }
        }

        public string KeyLabel => _presenter?.KeyLabel ?? _keyLabel;
        public KeyType Type => _presenter?.Type ?? _keyType;
        public KeyState CurrentState => _presenter?.CurrentState ?? KeyState.Default;

        public void OnSpawnFromPool()
        {
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            ResetKeyState();
            _presenter?.ClearCallback();

            var rectTransform = GetComponent<RectTransform>();
            if (rectTransform)
            {
                var sizeDelta = rectTransform.sizeDelta;
                sizeDelta.x = 60f;
                rectTransform.sizeDelta = sizeDelta;
            }

            _layoutElement.minWidth = -1f;

            gameObject.SetActive(false);
        }
    }

    public enum KeyType
    {
        Letter,
        Enter,
        Backspace
    }

    public enum KeyState
    {
        Default = 0,
        Absent = 1,
        Present = 2,
        Correct = 3
    }
}