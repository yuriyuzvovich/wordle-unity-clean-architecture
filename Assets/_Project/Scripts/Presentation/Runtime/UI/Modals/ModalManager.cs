using UnityEngine;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.AssetManagement;

namespace Wordle.Presentation.UI.Modals
{
    public class ModalManager : IModalManager
    {
        private const string WIN_MODAL_ADDRESS = "winmodal";
        private const string LOSE_MODAL_ADDRESS = "losemodal";

        private readonly AddressablesAssetService _assetService;
        private readonly IEngineLifecycle _engineLifecycle;
        private readonly ILogService _logService;

        private GameObject _winModalInstance;
        private GameObject _loseModalInstance;

        public ModalManager(
            AddressablesAssetService assetService,
            IEngineLifecycle engineLifecycle,
            ILogService logService
        )
        {
            _assetService = assetService;
            _engineLifecycle = engineLifecycle;
            _logService = logService;

            _engineLifecycle.OnApplicationQuitted += Cleanup;
        }

        public async UniTask InitializeAsync()
        {
            _winModalInstance = await _assetService.InstantiateAsync(WIN_MODAL_ADDRESS);

            if (!_winModalInstance)
            {
                _logService.LogError("ModalManager: Failed to instantiate WinModal prefab");
            }
            else
            {
                _winModalInstance.name = "WinModal";
                _logService.LogInfo("ModalManager: WinModal loaded successfully");
            }

            _loseModalInstance = await _assetService.InstantiateAsync(LOSE_MODAL_ADDRESS);

            if (!_loseModalInstance)
            {
                _logService.LogError("ModalManager: Failed to instantiate LoseModal prefab");
            }
            else
            {
                _loseModalInstance.name = "LoseModal";
                _logService.LogInfo("ModalManager: LoseModal loaded successfully");
            }
        }

        private void Cleanup()
        {
            if (_winModalInstance && _assetService != null)
            {
                _assetService.ReleaseInstance(_winModalInstance);
                _logService.LogInfo("ModalManager: WinModal instance released");
            }

            if (_loseModalInstance && _assetService != null)
            {
                _assetService.ReleaseInstance(_loseModalInstance);
                _logService.LogInfo("ModalManager: LoseModal instance released");
            }

            _engineLifecycle.OnApplicationQuitted -= Cleanup;
        }
    }
}