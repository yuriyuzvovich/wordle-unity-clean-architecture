using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.Commands;
using Wordle.Infrastructure.Common.DI;
using Wordle.Infrastructure.Common.Events;
using Wordle.Infrastructure.Common.Lifecycle;
using Wordle.Infrastructure.Common.Pooling;
using Wordle.Infrastructure.Integration;
using Wordle.Infrastructure.Logging;
using Wordle.Infrastructure.Pooling;

namespace Wordle
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private bool _dontDestroyOnLoad = true;
        [SerializeField] private int _maxCommandsPerFrame = 1;

        private IDependencyContainer _container;
        private ICommandProcessor _commandProcessor;
        private ILogService _logger;

        private void Awake()
        {
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Bootstrapper: Initializing Application ===");

            InitializeInfrastructure(sb);

            sb.AppendLine("[Level 3] Initializing Persistence (Async)...");
            await PersistenceInitializer.InitializeAsync(_container);
            sb.AppendLine("[Level 3] Persistence async initialization complete.");

            sb.AppendLine("=== Bootstrapper: Initialization Complete ===");
            sb.AppendLine("Note: Presentation layer will be initialized by MainSceneInitializer in Main scene.");

            if (_logger != null)
            {
                _logger.LogInfo(sb.ToString());
            }
            else
            {
                Debug.Log(sb.ToString());
            }

            LoadMainScene();
        }

        private void LoadMainScene()
        {
            if (SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().buildIndex == 0)
            {
                _logger?.LogInfo("Loading Main scene (build index 1)...");
                SceneManager.LoadScene(1);
            }
        }

        private void InitializeInfrastructure(StringBuilder sb)
        {
            sb.AppendLine("[Level 0] Creating Dependency Container...");
            _container = new DependencyContainer();
            ProjectContext.Initialize(_container);

            sb.AppendLine("[Level 0] Registering ILogService...");
            _logger = new UnityLogger(LogLevel.Debug);
            _container.RegisterSingleton<ILogService>(_logger);

            sb.AppendLine("[Level 0] Registering IEngineLifecycle...");
            var lifecycleProvider = gameObject.AddComponent<UnityLifecycleProvider>();
            _container.RegisterSingleton<IEngineLifecycle>(lifecycleProvider);

            sb.AppendLine("[Level 0] Registering IEventBus...");
            var eventBus = new EventBus();
            _container.RegisterSingleton<IEventBus>(eventBus);

            sb.AppendLine("[Level 0] Registering IObjectPoolService...");
            var poolService = new ObjectPoolService(_logger);
            _container.RegisterSingleton<IObjectPoolService>(poolService);

            sb.AppendLine("[Level 1] Registering Configuration...");
            ConfigurationInitializer.Initialize(_container);

            sb.AppendLine("[Level 1] Registering Command Queue...");
            var commandQueue = new CommandQueue();
            _container.RegisterSingleton<ICommandQueue>(commandQueue);

            sb.AppendLine("[Level 1] Registering Command Processor...");
            _commandProcessor = new CommandProcessor(commandQueue, lifecycleProvider, eventBus, _logger);
            _commandProcessor.MaxCommandsPerFrame = _maxCommandsPerFrame;
            _container.RegisterSingleton<ICommandProcessor>(_commandProcessor);

            _commandProcessor.Start();
            sb.AppendLine("[Level 1] Command Processor started.");

            sb.AppendLine("[Level 2] Registering Asset Management...");
            AssetManagementInitializer.Initialize(_container);

            sb.AppendLine("[Level 3] Registering Persistence...");
            PersistenceInitializer.Initialize(_container);

            sb.AppendLine("[Level 3] Registering Remote Services...");
            RemoteServicesInitializer.Initialize(_container);

            sb.AppendLine("Infrastructure initialization complete.");
        }

        private void OnDestroy()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Bootstrapper: Shutting Down ===");

            if (_commandProcessor != null && _commandProcessor.IsRunning)
            {
                _commandProcessor.Stop();
            }

            ProjectContext.Clear();

            sb.AppendLine("=== Bootstrapper: Shutdown Complete ===");

            if (_logger != null)
            {
                _logger.LogInfo(sb.ToString());
            }
            else
            {
                Debug.Log(sb.ToString());
            }
        }

        private void OnApplicationQuit()
        {
            if (_logger != null)
            {
                _logger.LogInfo("=== Application Quitting ===");
            }
            else
            {
                Debug.Log("=== Application Quitting ===");
            }
        }
    }
}
