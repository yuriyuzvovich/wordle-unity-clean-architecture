using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.DI;

namespace Wordle.Presentation.UI.Background
{
    /// <summary>
    /// Visual component for the game background.
    /// Implements MVP pattern - delegates logic to BackgroundPresenter.
    /// </summary>
    public class BackgroundView : InjectableMonoBehaviour, IBackgroundView
    {
        [Header("Visual Components")]
        [SerializeField] private Image _backgroundImage;

        [Header("Animation Settings")]
        [SerializeField] private float _transitionDuration = 0.3f;

        [Inject] private IEventBus _eventBus;
        [Inject] private IThemeService _themeService;

        private BackgroundPresenter _presenter;

        protected override void Awake()
        {
            base.Awake();

            if (!_backgroundImage) throw new System.Exception("BackgroundView: Background Image component not assigned");

            _presenter = new BackgroundPresenter(this, _eventBus, _themeService);
        }

        public void SetBackgroundColor(Color color)
        {
            if (_backgroundImage)
            {
                _backgroundImage.DOColor(color, _transitionDuration).SetEase(Ease.OutQuad);
            }
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}
