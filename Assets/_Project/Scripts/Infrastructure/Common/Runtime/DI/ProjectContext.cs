using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Common.DI
{
    /// <summary>
    /// Global access point for the dependency injection container.
    /// Initialized by the Bootstrapper at application startup.
    /// </summary>
    public static class ProjectContext
    {
        private static IDependencyContainer _container;

        /// <summary>
        /// The global dependency container.
        /// </summary>
        public static IDependencyContainer Container
        {
            get
            {
                if (_container == null)
                {
                    UnityEngine.Debug.LogWarning("ProjectContext.Container accessed before initialization. " +
                                                "Make sure the Bootstrapper runs first.");
                }
                return _container;
            }
        }

        /// <summary>
        /// Initialize the project context with a container.
        /// Should only be called once by the Bootstrapper.
        /// </summary>
        public static void Initialize(IDependencyContainer container)
        {
            if (_container != null)
            {
                UnityEngine.Debug.LogWarning("ProjectContext already initialized. " +
                                            "This should only happen once during application startup.");
            }

            _container = container;
            UnityEngine.Debug.Log("ProjectContext initialized with dependency container.");
        }

        /// <summary>
        /// Clear the container (for testing or scene transitions).
        /// </summary>
        public static void Clear()
        {
            _container = null;
            UnityEngine.Debug.Log("ProjectContext cleared.");
        }
    }
}
