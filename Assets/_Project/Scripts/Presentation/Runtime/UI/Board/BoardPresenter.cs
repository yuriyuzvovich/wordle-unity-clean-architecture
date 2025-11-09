using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;
using Wordle.Core.ValueObjects;

namespace Wordle.Presentation.UI.Board
{
    /// <summary>
    /// Presenter for board logic.
    /// Handles game state loading, event coordination, and reveal animations.
    /// </summary>
    public class BoardPresenter : IDisposable
    {
        private readonly IBoardView _view;
        private readonly IEventBus _eventBus;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IGameRules _gameRules;
        private readonly ILogService _logService;

        private int _currentRow = 0;
        private bool _enableStaggeredFlip = true;
        private float _flipDelayBetweenTiles = 0.15f;
        private float _tileFlipDuration = 0.6f;

        public int CurrentRow => _currentRow;
        
        public BoardPresenter(
            IBoardView view,
            IEventBus eventBus,
            IGameStateRepository gameStateRepository,
            IGameRules gameRules,
            ILogService logService
        )
        {
            _view = view;
            _eventBus = eventBus;
            _gameStateRepository = gameStateRepository;
            _gameRules = gameRules;
            _logService = logService;

            SubscribeToEvents();
        }

        public void Initialize(bool enableStaggeredFlip = true, float flipDelayBetweenTiles = 0.15f, float tileFlipDuration = 0.6f)
        {
            _enableStaggeredFlip = enableStaggeredFlip;
            _flipDelayBetweenTiles = flipDelayBetweenTiles;
            _tileFlipDuration = tileFlipDuration;
        }

        public async UniTask LoadGameStateAsync()
        {
            var gameState = await _gameStateRepository.LoadAsync();

            if (gameState == null)
            {
                _currentRow = 0;
                return;
            }

            _currentRow = gameState.CurrentAttempt;

            for (int i = 0; i < gameState.Guesses.Count; i++)
            {
                var guess = gameState.Guesses[i];
                DisplayGuess(i, guess);
            }
        }

        private void DisplayGuess(int row, Guess guess)
        {
            if (row < 0 || row >= _view.RowCount) return;

            for (int col = 0; col < _view.ColumnCount && col < guess.Evaluations.Length; col++)
            {
                var letterPos = guess.Evaluations[col];
                _view.DisplayLetterAtTile(row, col, letterPos.Letter);
                _view.SetTileEvaluation(row, col, letterPos.Evaluation);
            }
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<GuessSubmittedEvent>(OnGuessSubmitted);
            _eventBus.Subscribe<InvalidWordSubmittedEvent>(OnInvalidWordSubmitted);
            _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            _eventBus.Subscribe<RowRevealCompleteEvent>(OnRowRevealComplete);
            _eventBus.Subscribe<GameLostEvent>(OnGameLost);
        }

        private void OnGuessSubmitted(GuessSubmittedEvent evt)
        {
            EvaluateCurrentRowAsync().Forget();
        }

        private void OnInvalidWordSubmitted(InvalidWordSubmittedEvent evt)
        {
            if (_currentRow < 0 || _currentRow >= _view.RowCount)
            {
                return;
            }
            _view.PlayRowShakeAnimation(_currentRow);
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            _view.ResetAllTiles();
            _currentRow = 0;
        }

        private void OnRowRevealComplete(RowRevealCompleteEvent evt)
        {
            if (evt.IsWinningRow)
            {
                PlayVictoryAnimationAsync().Forget();
            }
        }

        private void OnGameLost(GameLostEvent evt)
        {
        }

        private async UniTaskVoid EvaluateCurrentRowAsync()
        {
            var gameState = await _gameStateRepository.LoadAsync();

            if (gameState == null || !gameState.HasGuesses())
            {
                _logService.LogWarning("BoardPresenter: No game state or guesses found");
                return;
            }

            var lastGuess = gameState.GetLastGuess();
            int row = gameState.CurrentAttempt - 1;

            if (row < 0 || row >= _view.RowCount)
            {
                _logService.LogError($"BoardPresenter: Invalid row index {row}");
                return;
            }

            bool isWinningRow = gameState.Status == GameStatus.Won;

            if (_enableStaggeredFlip)
            {
                await RevealTilesWithStagger(row, lastGuess.Evaluations, isWinningRow);
            }
            else
            {
                RevealTilesImmediate(row, lastGuess.Evaluations);
                _eventBus.Publish(new RowRevealCompleteEvent(row, isWinningRow));
            }

            _currentRow = gameState.CurrentAttempt;
        }

        private async UniTask RevealTilesWithStagger(int row, LetterPosition[] evaluations, bool isWinningRow)
        {
            for (int col = 0; col < _view.ColumnCount && col < evaluations.Length; col++)
            {
                _view.PlayTileFlipAnimation(row, col, evaluations[col].Evaluation);

                if (col < _view.ColumnCount - 1)
                {
                    await UniTask.Delay((int) (_flipDelayBetweenTiles * 1000));
                }
            }

            await UniTask.Delay((int) (_tileFlipDuration * 1000));

            _eventBus.Publish(new RowRevealCompleteEvent(row, isWinningRow));
        }

        private void RevealTilesImmediate(int row, LetterPosition[] evaluations)
        {
            for (int col = 0; col < _view.ColumnCount && col < evaluations.Length; col++)
            {
                _view.SetTileEvaluation(row, col, evaluations[col].Evaluation);
            }
        }

        private async UniTaskVoid PlayVictoryAnimationAsync()
        {
            var gameState = await _gameStateRepository.LoadAsync();
            if (gameState == null)
            {
                return;
            }

            int winningRow = gameState.CurrentAttempt - 1;
            if (winningRow < 0 || winningRow >= _view.RowCount)
            {
                return;
            }

            await UniTask.Delay(300);

            _view.PlayRowVictoryAnimation(winningRow);
        }

        public void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GuessSubmittedEvent>(OnGuessSubmitted);
                _eventBus.Unsubscribe<InvalidWordSubmittedEvent>(OnInvalidWordSubmitted);
                _eventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
                _eventBus.Unsubscribe<RowRevealCompleteEvent>(OnRowRevealComplete);
                _eventBus.Unsubscribe<GameLostEvent>(OnGameLost);
            }
        }
    }
}