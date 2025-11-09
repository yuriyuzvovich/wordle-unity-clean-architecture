using Wordle.Application.Interfaces;
using Wordle.Infrastructure.AssetManagement;

namespace Wordle.Infrastructure.Integration
{
    /// <summary>
    /// Initializes asset management services.
    /// Placed outside assembly definitions to bridge Common and AssetManagement modules.
    /// </summary>
    public static class AssetManagementInitializer
    {
        public static void Initialize(IDependencyContainer container)
        {
            var assetService = new AddressablesAssetService(container.Resolve<ILogService>());
            container.RegisterSingleton<IAssetService>(assetService);
            container.RegisterSingleton<AddressablesAssetService>(assetService);
        }
    }
}