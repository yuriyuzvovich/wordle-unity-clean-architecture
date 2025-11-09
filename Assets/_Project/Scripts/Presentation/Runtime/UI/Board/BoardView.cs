using UnityEngine;
using Cysharp.Threading.Tasks;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;
using Wordle.Core.Interfaces;
using Wordle.Infrastructure.Common.DI;
using Wordle.Infrastructure.Common.Pooling;
using Wordle.Infrastructure.AssetManagement;
using Wordle.Infrastructure.Pooling;

namespace Wordle.Presentation.UI.Board
{
    /// <summary>
    /// Visual component for the game board.
    /// Implements MVP pattern - delegates logic to BoardPresenter.
    /// </summary>
    public class BoardView : InjectableMonoBehaviour, IBoardView
    {
        private const int COLUMNS = Word.WORD_LENGTH;
        private const string TILE_PREFAB_ADDRESS = "lettertileview";

        private int RowsCount => _gameRules.MaxAttempts;

        [Header("Tile Configuration")]
        [SerializeField] private Transform _gridContainer;

        [Header("Layout Settings")]
        [SerializeField] private float _tileSpacing = 10f;
        [SerializeField] private float _rowSpacing = 10f;

        [Header("Animation Settings")]
        [SerializeField] private float _flipDelayBetweenTiles = 0.15f;
        [SerializeField] private float _tileFlipDuration = 0.6f;
        [SerializeField] private bool _enableStaggeredFlip = true;

        [Inject] private IEventBus _eventBus;
        [Inject] private IGameStateRepository _gameStateRepository;
        [Inject] private AddressablesAssetService _assetService;
        [Inject] private IObjectPoolService _poolService;
        [Inject] private IGameRules _gameRules;
        [Inject] private ILogService _logService;

        private LetterTileView[,] _tiles;
        private bool _poolInitialized = false;
        private BoardPresenter _presenter;

        protected override void Awake()
        {
            base.Awake();

            if (!_gridContainer)
            {
                _gridContainer = transform;
            }

            _presenter = new BoardPresenter(
                this, _eventBus, 
                _gameStateRepository,
                _gameRules, _logService
            );
            _presenter.Initialize(_enableStaggeredFlip, _flipDelayBetweenTiles, _tileFlipDuration);

            InitializeGridAsync().Forget();
        }

        private void Start()
        {
            _presenter.LoadGameStateAsync().Forget();
        }

        private async UniTaskVoid InitializeGridAsync()
        {
            _tiles = new LetterTileView[RowsCount, COLUMNS];

            await InitializePoolAsync();

            for (int row = 0; row < RowsCount; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    CreateTile(row, col);
                }
            }
        }

        private async UniTask InitializePoolAsync()
        {
            if (_poolInitialized)
            {
                return;
            }

            var tilePrefab = await _assetService.LoadAssetAsync<GameObject>(TILE_PREFAB_ADDRESS);
            var tileView = tilePrefab.GetComponent<LetterTileView>();

            if (!tileView)
            {
                Debug.LogError("BoardView: Tile prefab has no LetterTileView component");
                return;
            }

            int totalTiles = RowsCount * COLUMNS;
            _poolService.CreatePool(tileView, _gridContainer, initialSize : totalTiles, maxSize : totalTiles);
            _poolInitialized = true;
        }

        private void CreateTile(int row, int col)
        {
            var tileView = _poolService.Get<LetterTileView>();
            if (!tileView)
            {
                Debug.LogError($"BoardView: Failed to get tile from pool at [{row},{col}]");
                return;
            }

            tileView.gameObject.name = $"Tile_R{row}_C{col}";
            tileView.Initialize(row, col);
            _tiles[row, col] = tileView;
        }

        public void DisplayLetterAtTile(int row, int col, char letter)
        {
            var tile = GetTile(row, col);
            if (tile)
            {
                tile.SetLetter(letter);
            }
        }

        public void SetTileEvaluation(int row, int col, LetterEvaluation evaluation)
        {
            var tile = GetTile(row, col);
            if (tile)
            {
                tile.SetEvaluation(evaluation);
            }
        }

        public void PlayTileFlipAnimation(int row, int col, LetterEvaluation evaluation)
        {
            var tile = GetTile(row, col);
            if (tile)
            {
                tile.SetEvaluation(evaluation);
            }
        }

        public void PlayRowShakeAnimation(int row)
        {
            if (row < 0 || row >= RowsCount) return;

            for (int col = 0; col < COLUMNS; col++)
            {
                var tile = _tiles[row, col];
                if (tile)
                {
                    tile.PlayShakeAnimation();
                }
            }
        }

        public void PlayRowVictoryAnimation(int row)
        {
            if (row < 0 || row >= RowsCount) return;

            PlayRowVictoryAnimationAsync(row).Forget();
        }

        private async UniTaskVoid PlayRowVictoryAnimationAsync(int row)
        {
            for (int col = 0; col < COLUMNS; col++)
            {
                var tile = _tiles[row, col];
                if (tile)
                {
                    tile.PlayShakeAnimation();
                }

                await UniTask.Delay(100);
            }
        }

        public void ClearTile(int row, int col)
        {
            var tile = GetTile(row, col);
            if (tile)
            {
                tile.ClearLetter();
            }
        }

        public void ResetAllTiles()
        {
            for (int row = 0; row < RowsCount; row++)
            {
                for (int col = 0; col < COLUMNS; col++)
                {
                    var tile = _tiles[row, col];
                    if (tile)
                    {
                        tile.ResetTile();
                    }
                }
            }
        }

        public LetterTileView GetTile(int row, int col)
        {
            if (row < 0 || row >= RowsCount || col < 0 || col >= COLUMNS)
            {
                Debug.LogWarning($"BoardView: Tile index out of range [{row},{col}]");
                return null;
            }

            return _tiles[row, col];
        }

        public int CurrentRow => _presenter?.CurrentRow ?? 0;
        public int RowCount => RowsCount;
        public int ColumnCount => COLUMNS;

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}