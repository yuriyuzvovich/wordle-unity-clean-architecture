using UnityEngine;

namespace Wordle.Infrastructure.Common.DI
{
    /// <summary>
    /// Base class for MonoBehaviours that need dependency injection.
    /// Automatically injects dependencies in Awake().
    /// </summary>
    public abstract class InjectableMonoBehaviour : MonoBehaviour
    {
        private bool _injected = false;

        protected virtual void Awake()
        {
            if (!_injected)
            {
                InjectDependencies();
            }
        }

        /// <summary>
        /// Manually trigger dependency injection.
        /// Useful if you need to inject before Awake() or re-inject.
        /// </summary>
        public void InjectDependencies()
        {
            if (ProjectContext.Container == null)
            {
                Debug.LogError($"ProjectContext.Container is null. Cannot inject dependencies into {GetType().Name}. " +
                              "Make sure the Bootstrapper has initialized the container.");
                return;
            }

            ProjectContext.Container.Inject(this);
            _injected = true;
        }
    }
}
