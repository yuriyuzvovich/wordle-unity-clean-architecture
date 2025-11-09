using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.AssetManagement
{
    /// <summary>
    /// Asset service implementation using Unity Addressables system.
    /// Handles async loading, instantiation, and release of game assets.
    /// Implements IAssetService interface plus Unity-specific convenience methods.
    /// </summary>
    public class AddressablesAssetService : IAssetService
    {
        private readonly ILogService _logService;

        public AddressablesAssetService(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public async UniTask<T> LoadAssetAsync<T>(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("Address cannot be null or empty.", nameof(address));
            }

            var handle = Addressables.LoadAssetAsync<T>(address);

            try
            {
                var result = await handle.ToUniTask();
                if (result == null)
                {
                    throw new InvalidOperationException($"Failed to load asset at address: {address}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logService.LogError($"Error loading asset '{address}': {ex.Message}");
                throw;
            }
        }

        public async UniTask<IList<T>> LoadAssetsAsync<T>(
            IEnumerable<string> addresses,
            Action<T> callback = null
        )
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            var loadedAssets = new List<T>();
            foreach (var address in addresses)
            {
                if (string.IsNullOrEmpty(address))
                {
                    _logService.LogWarning($"Skipping null or empty address in batch load.");
                    continue;
                }

                try
                {
                    var asset = await LoadAssetAsync<T>(address);
                    loadedAssets.Add(asset);
                    callback?.Invoke(asset);
                }
                catch (Exception ex)
                {
                    _logService.LogError($"Error loading asset '{address}' in batch: {ex.Message}");
                }
            }

            return loadedAssets;
        }

        public void ReleaseAsset<T>(T asset)
        {
            if (asset == null)
            {
                _logService.LogWarning("Attempted to release null asset.");
                return;
            }

            try
            {
                Addressables.Release(asset);
            }
            catch (Exception ex)
            {
                _logService.LogError($"Error releasing asset: {ex.Message}");
            }
        }

        /// <summary>
        /// Unity-specific convenience method for instantiating GameObjects from Addressables.
        /// </summary>
        public async UniTask<GameObject> InstantiateAsync(
            string address,
            Transform parent = null,
            bool instantiateInWorldSpace = false
        )
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("Address cannot be null or empty.", nameof(address));
            }

            AsyncOperationHandle<GameObject> handle;

            if (parent)
            {
                handle = Addressables.InstantiateAsync(address, parent, instantiateInWorldSpace);
            }
            else
            {
                handle = Addressables.InstantiateAsync(address);
            }

            try
            {
                var result = await handle.ToUniTask();
                if (!result)
                {
                    throw new InvalidOperationException($"Failed to instantiate prefab at address: {address}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logService.LogError($"Error instantiating prefab '{address}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Unity-specific convenience method for releasing instantiated GameObjects.
        /// </summary>
        public bool ReleaseInstance(GameObject instance)
        {
            if (!instance)
            {
                _logService.LogWarning("Attempted to release null GameObject instance.");
                return false;
            }

            try
            {
                return Addressables.ReleaseInstance(instance);
            }
            catch (Exception ex)
            {
                _logService.LogError($"Error releasing GameObject instance: {ex.Message}");
                return false;
            }
        }
    }
}