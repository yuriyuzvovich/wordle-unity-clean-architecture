using System;
using Cysharp.Threading.Tasks;
using Wordle.Core.Entities;
using Wordle.Core.Interfaces;

namespace Wordle.Application.Tests.Mocks
{
    public class MockGameStateRepository : IGameStateRepository
    {
        private GameState _savedState;
        public bool ThrowOnSave;
        public bool ThrowOnLoad;
        public bool ThrowOnClear;

        public UniTask<bool> HasSavedStateAsync()
        {
            return UniTask.FromResult(_savedState != null);
        }

        public UniTask<GameState> LoadAsync()
        {
            if (ThrowOnLoad)
            {
                throw new InvalidOperationException("Failed to load game state");
            }
            return UniTask.FromResult(_savedState);
        }

        public UniTask SaveAsync(GameState gameState)
        {
            if (ThrowOnSave)
            {
                throw new InvalidOperationException("Failed to save game state");
            }
            _savedState = gameState;
            return UniTask.CompletedTask;
        }

        public UniTask ClearAsync()
        {
            if (ThrowOnClear)
            {
                throw new InvalidOperationException("Failed to clear game state");
            }
            _savedState = null;
            return UniTask.CompletedTask;
        }

        public GameState GetSavedState()
        {
            return _savedState;
        }
    }
}
