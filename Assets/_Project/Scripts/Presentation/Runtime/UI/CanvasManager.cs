using UnityEngine;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.AssetManagement;

namespace Wordle.Presentation.UI
{
    /// <summary>
    /// Manages the main game canvas.
    /// Dynamically loads the WordleCanvas prefab from Addressables.
    /// </summary>
    public class CanvasManager : ICanvasManager
    {
        private const string CANVAS_PREFAB_ADDRESS = "wordlecanvas";

        private readonly AddressablesAssetService _assetService;
        private readonly IEngineLifecycle _engineLifecycle;
        private readonly ILogService _logService;

        private GameObject _canvasInstance;

        public CanvasManager(AddressablesAssetService assetService, IEngineLifecycle engineLifecycle, ILogService logService)
        {
            _assetService = assetService;
            _engineLifecycle = engineLifecycle;
            _logService = logService;

            _engineLifecycle.OnApplicationQuitted += Cleanup;
        }

        public async UniTask InitializeAsync()
        {
            _canvasInstance = await _assetService.InstantiateAsync(CANVAS_PREFAB_ADDRESS);

            if (!_canvasInstance)
            {
                _logService.LogError("CanvasManager: Failed to instantiate WordleCanvas prefab");
                return;
            }

            _canvasInstance.name = "WordleCanvas";
            _logService.LogInfo("CanvasManager: WordleCanvas loaded successfully");
        }

        private void Cleanup()
        {
            if (_canvasInstance && _assetService != null)
            {
                _assetService.ReleaseInstance(_canvasInstance);
                _logService.LogInfo("CanvasManager: Canvas instance released");
            }

            _engineLifecycle.OnApplicationQuitted -= Cleanup;
        }
    }
}
