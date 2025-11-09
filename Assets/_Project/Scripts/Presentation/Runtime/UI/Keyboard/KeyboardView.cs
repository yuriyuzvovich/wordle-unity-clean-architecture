using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Core.ValueObjects;
using Wordle.Core.Interfaces;
using Wordle.Infrastructure.Common.DI;
using Wordle.Infrastructure.Common.Pooling;
using Wordle.Infrastructure.AssetManagement;
using Wordle.Infrastructure.Pooling;
using Wordle.Presentation.Controllers;
using Wordle.Presentation.Inputs;

namespace Wordle.Presentation.UI.Keyboard
{
    /// <summary>
    /// Visual component for the virtual keyboard.
    /// Implements MVP pattern - delegates logic to KeyboardPresenter.
    /// </summary>
    public class KeyboardView : InjectableMonoBehaviour, IKeyboardView
    {
        private const string KEY_PREFAB_ADDRESS = "keyview";
        private const int MAX_KEYS_ACROSS_ALL_LAYOUTS = 35;

        [Header("Key Configuration")]
        [SerializeField] private Transform _row1Container;
        [SerializeField] private Transform _row2Container;
        [SerializeField] private Transform _row3Container;

        [Inject] private IEventBus _eventBus;
        [Inject] private IGameStateRepository _gameStateRepository;
        [Inject] private AddressablesAssetService _assetService;
        [Inject] private InputController _inputController;
        [Inject] private IObjectPoolService _poolService;
        [Inject] private ILocalizationService _localizationService;
        [Inject] private IKeyboardLayoutProvider _layoutProvider;
        [Inject] private ILogService _logsService;

        private Dictionary<string, KeyView> _keys;
        private TouchInputService _touchInputService;
        private bool _poolInitialized = false;
        private KeyView _enterKey;
        private KeyView _backspaceKey;
        private KeyboardPresenter _presenter;

        protected override void Awake()
        {
            base.Awake();

            _keys = new Dictionary<string, KeyView>();

            InitializeKeyboardAsync().Forget();
        }

        private void Start()
        {
            // TouchInputService initialization is now handled in InitializeKeyboardAsync
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }

        private async UniTaskVoid InitializeKeyboardAsync()
        {
            if (!_row1Container) throw new System.Exception("KeyboardView: Row1 container is not assigned in Inspector and could not be found as child.");
            if (!_row2Container) throw new System.Exception("KeyboardView: Row2 container is not assigned in Inspector and could not be found as child.");
            if (!_row3Container) throw new System.Exception("KeyboardView: Row3 container is not assigned in Inspector and could not be found as child.");

            await InitializePoolAsync();

            _touchInputService = _inputController.GetTouchInputService();

            if (_touchInputService == null)
            {
                Debug.LogError("KeyboardView: TouchInputService is null! InputController may not be initialized properly.");
                return;
            }

            _presenter = new KeyboardPresenter(
                this,
                _eventBus,
                _gameStateRepository,
                _localizationService,
                _layoutProvider,
                _touchInputService,
                _logsService
            );

            _presenter.Initialize();
            _presenter.RestoreKeyStatesAsync().Forget();
        }

        private async UniTask InitializePoolAsync()
        {
            if (_poolInitialized)
            {
                return;
            }

            var keyPrefab = await _assetService.LoadAssetAsync<GameObject>(KEY_PREFAB_ADDRESS);
            var keyView = keyPrefab.GetComponent<KeyView>();
            if (!keyView) throw new System.Exception("KeyboardView: Loaded key prefab does not contain a KeyView component.");

            _poolService.CreatePool(keyView, transform, initialSize : MAX_KEYS_ACROSS_ALL_LAYOUTS, maxSize : MAX_KEYS_ACROSS_ALL_LAYOUTS);
            _poolInitialized = true;
        }

        public void CreateKey(string label, KeyType type, int rowIndex, Action<string> onKeyPressedCallback)
        {
            var keyView = _poolService.Get<KeyView>();
            if (!keyView) throw new System.Exception("KeyboardView: Failed to retrieve KeyView from pool.");

            Transform parent = rowIndex switch {
                0 => _row1Container,
                1 => _row2Container,
                2 => _row3Container,
                _ => throw new System.ArgumentException($"Invalid row index: {rowIndex}")
            };

            keyView.gameObject.name = $"Key_{label}";
            keyView.transform.SetParent(parent);
            keyView.Initialize(label, type, onKeyPressedCallback);

            if (type == KeyType.Letter)
            {
                _keys[label] = keyView;
            }
            else if (type == KeyType.Enter)
            {
                _enterKey = keyView;
            }
            else if (type == KeyType.Backspace)
            {
                _backspaceKey = keyView;
            }
        }

        public void UpdateKeyState(string label, LetterEvaluation evaluation)
        {
            if (_keys.TryGetValue(label, out var keyView))
            {
                keyView.SetKeyState(evaluation);
            }
        }

        public void UpdateKeyLocalization(string enterText, string backspaceText)
        {
            if (_enterKey)
            {
                _enterKey.SetLocalizedTexts(enterText, backspaceText);
            }

            if (_backspaceKey)
            {
                _backspaceKey.SetLocalizedTexts(enterText, backspaceText);
            }

            foreach (var kvp in _keys)
            {
                if (kvp.Value)
                {
                    kvp.Value.SetLocalizedTexts(enterText, backspaceText);
                }
            }
        }

        public void ClearAllKeys()
        {
            foreach (var kvp in _keys)
            {
                if (kvp.Value)
                {
                    _poolService.Return(kvp.Value);
                }
            }
            _keys.Clear();

            if (_enterKey)
            {
                _poolService.Return(_enterKey);
                _enterKey = null;
            }

            if (_backspaceKey)
            {
                _poolService.Return(_backspaceKey);
                _backspaceKey = null;
            }
        }

        public void ResetAllKeys()
        {
            foreach (var kvp in _keys)
            {
                kvp.Value.ResetKeyState();
            }
        }

        public KeyView GetKey(string keyLabel)
        {
            if (_keys.TryGetValue(keyLabel, out var keyView))
            {
                return keyView;
            }

            Debug.LogWarning($"KeyboardView: Key '{keyLabel}' not found");
            return null;
        }
    }
}