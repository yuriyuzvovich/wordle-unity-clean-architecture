using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Service for loading and managing game assets asynchronously.
    /// Provides abstraction over the underlying asset loading system (e.g., Addressables).
    /// Note: Unity-specific methods (InstantiateAsync, ReleaseInstance) are available
    /// on the concrete implementation in the Infrastructure layer.
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Load a single asset asynchronously by its address.
        /// </summary>
        /// <typeparam name="T">The type of asset to load.</typeparam>
        /// <param name="address">The address or key of the asset.</param>
        /// <returns>The loaded asset.</returns>
        UniTask<T> LoadAssetAsync<T>(string address);

        /// <summary>
        /// Load multiple assets asynchronously by their addresses.
        /// </summary>
        /// <typeparam name="T">The type of assets to load.</typeparam>
        /// <param name="addresses">The addresses or keys of the assets.</param>
        /// <param name="callback">Optional callback invoked for each loaded asset.</param>
        /// <returns>List of loaded assets.</returns>
        UniTask<IList<T>> LoadAssetsAsync<T>(IEnumerable<string> addresses, Action<T> callback = null);

        /// <summary>
        /// Release a previously loaded asset to free memory.
        /// </summary>
        /// <typeparam name="T">The type of asset to release.</typeparam>
        /// <param name="asset">The asset to release.</param>
        void ReleaseAsset<T>(T asset);
    }
}
