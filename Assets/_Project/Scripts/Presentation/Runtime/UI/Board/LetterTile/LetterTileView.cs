using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Application.Interfaces.Pooling;
using Wordle.Core.ValueObjects;
using Wordle.Infrastructure.Common.DI;
using Wordle.Presentation.UI.Theme;

namespace Wordle.Presentation.UI.Board
{
    /// <summary>
    /// Visual component for a single letter tile.
    /// Implements MVP pattern - delegates logic to LetterTilePresenter.
    /// </summary>
    public class LetterTileView : InjectableMonoBehaviour, ILetterTileView, IPoolable
    {
        [Header("Tile Configuration")]
        [SerializeField] private int _row;
        [SerializeField] private int _column;

        [Header("Visual Components")]
        [SerializeField] private TextMeshProUGUI _letterText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private RectTransform _rectTransform;

        [Header("Animation Settings")]
        [SerializeField] private float _popDuration = 0.1f;
        [SerializeField] private float _popScale = 1.1f;
        [SerializeField] private float _flipDuration = 0.6f;
        [SerializeField] private float _shakeDuration = 0.5f;
        [SerializeField] private float _shakeStrength = 10f;

        [Inject] private IEventBus _eventBus;
        [Inject] private IThemeService _themeService;

        private LetterTilePresenter _presenter;
        private Sequence _currentAnimation;

        protected override void Awake()
        {
            base.Awake();

            if (!_letterText)
            {
                Debug.LogError($"LetterTileView [{_row},{_column}]: TextMeshProUGUI component not assigned");
            }

            if (!_backgroundImage)
            {
                _backgroundImage = GetComponent<Image>();
            }

            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            _presenter = new LetterTilePresenter(this, _eventBus, _themeService);
            _presenter.Initialize(_row, _column);
        }

        public void Initialize(int row, int column)
        {
            _row = row;
            _column = column;
            _presenter?.Initialize(row, column);
        }

        public void SetLetter(char letter)
        {
            _presenter?.SetLetter(letter);
        }

        public void ClearLetter()
        {
            _presenter?.ClearLetter();
        }

        public void SetEvaluation(LetterEvaluation evaluation)
        {
            _presenter?.SetEvaluation(evaluation);
        }

        public void ResetTile()
        {
            _presenter?.ResetTile();
        }

        public void PlayShakeAnimation()
        {
            _presenter?.PlayShakeAnimation();
        }

        public void SetText(string text)
        {
            if (_letterText)
            {
                _letterText.text = text;
            }
        }

        public void SetTextColor(Color color)
        {
            if (_letterText)
            {
                _letterText.color = color;
            }
        }

        public void SetBackgroundColor(Color color)
        {
            if (_backgroundImage)
            {
                _backgroundImage.DOColor(color, 0.2f);
            }
        }

        public void PlayPopAnimation()
        {
            KillCurrentAnimation();

            if (!_rectTransform) return;

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_rectTransform.DOScale(_popScale, _popDuration / 2f).SetEase(Ease.OutQuad));
            _currentAnimation.Append(_rectTransform.DOScale(1f, _popDuration / 2f).SetEase(Ease.InQuad));
            _currentAnimation.OnComplete(() => _currentAnimation = null);
        }

        public void PlayFlipAnimation(System.Action onFlipHalf)
        {
            KillCurrentAnimation();

            if (!_rectTransform) return;

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_rectTransform.DORotate(new Vector3(90f, 0f, 0f), _flipDuration / 2f).SetEase(Ease.InQuad));
            _currentAnimation.AppendCallback(() => onFlipHalf?.Invoke());
            _currentAnimation.Append(_rectTransform.DORotate(Vector3.zero, _flipDuration / 2f).SetEase(Ease.OutQuad));
            _currentAnimation.OnComplete(() => _currentAnimation = null);
        }

        void ILetterTileView.PlayShakeAnimation()
        {
            KillCurrentAnimation();

            if (!_rectTransform) return;

            _currentAnimation = DOTween.Sequence();
            _currentAnimation.Append(_rectTransform.DOShakePosition(_shakeDuration, _shakeStrength, 10, 90, false, true));
            _currentAnimation.OnComplete(() => _currentAnimation = null);
        }

        public void ResetVisuals()
        {
            KillCurrentAnimation();

            if (_rectTransform)
            {
                _rectTransform.localScale = Vector3.one;
                _rectTransform.localRotation = Quaternion.identity;
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

        public int Row => _presenter?.Row ?? _row;
        public int Column => _presenter?.Column ?? _column;
        public char CurrentLetter => _presenter?.CurrentLetter ?? '\0';
        public TileState CurrentState => _presenter?.CurrentState ?? TileState.Empty;

        public void OnSpawnFromPool()
        {
            gameObject.SetActive(true);
        }

        public void OnReturnToPool()
        {
            ResetTile();
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            KillCurrentAnimation();
            _presenter?.Dispose();
        }
    }

    public enum TileState
    {
        Empty,
        Filled,
        Absent,
        Present,
        Correct
    }
}
