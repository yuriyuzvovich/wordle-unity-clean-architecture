using UnityEngine;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.DI;

namespace Wordle.Presentation.UI.SafeArea
{
    /// <summary>
    /// Adjusts RectTransform to fit within device safe area.
    /// Protects UI elements from being obscured by notches, camera cutouts, and home indicators.
    /// </summary>
    public class SafeAreaView : InjectableMonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        [Header("Safe Area Configuration")]
        [SerializeField] private bool _applyTop = true;
        [SerializeField] private bool _applyBottom = true;
        [SerializeField] private bool _applyLeft = true;
        [SerializeField] private bool _applyRight = true;

        [Header("Optional Padding")]
        [SerializeField] private float _additionalTopPadding = 0f;
        [SerializeField] private float _additionalBottomPadding = 0f;
        [SerializeField] private float _additionalLeftPadding = 0f;
        [SerializeField] private float _additionalRightPadding = 0f;

        [Inject] private ILogService _logService;

        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

        protected override void Awake()
        {
            base.Awake();

            if (!_rectTransform) throw new System.ArgumentNullException(nameof(_rectTransform), "RectTransform reference is required.");

            ApplySafeArea();
        }

        private void Update()
        {
            if (HasSafeAreaChanged())
            {
                ApplySafeArea();
            }
        }

        private bool HasSafeAreaChanged()
        {
            return Screen.safeArea != _lastSafeArea ||
                new Vector2Int(Screen.width, Screen.height) != _lastScreenSize;
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            if (Screen.width == 0 || Screen.height == 0)
            {
                return;
            }

            var (anchorMin, anchorMax) = SafeAreaCalculator.Calculate(
                safeArea,
                Screen.width,
                Screen.height,
                _applyLeft,
                _applyRight,
                _applyBottom,
                _applyTop,
                _additionalLeftPadding,
                _additionalRightPadding,
                _additionalBottomPadding,
                _additionalTopPadding
            );

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;

            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;

            _logService.LogInfo($"SafeAreaView: Applied safe area - AnchorMin: {anchorMin}, AnchorMax: {anchorMax}");
        }
    }
}