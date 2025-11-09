using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Common.Commands
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ICommandQueue _queue;
        private readonly IEngineLifecycle _lifecycle;
        private readonly IEventBus _eventBus;

        private bool _isRunning;
        private ICommand _currentCommand;
        private CancellationTokenSource _commandCts;
        private readonly ILogService _logService;

        public bool IsRunning => _isRunning;
        public int MaxCommandsPerFrame { get; set; } = 1;

        public CommandProcessor(
            ICommandQueue queue,
            IEngineLifecycle lifecycle,
            IEventBus eventBus,
            ILogService logService
        )
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public void Start()
        {
            if (_isRunning)
            {
                _logService?.LogWarning("CommandProcessor is already running.");
                return;
            }

            _isRunning = true;
            _lifecycle.OnFrameTick += ProcessCommands;
            _logService?.LogInfo("CommandProcessor started.");
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                _logService?.LogWarning("CommandProcessor is not running.");
                return;
            }

            _isRunning = false;
            _lifecycle.OnFrameTick -= ProcessCommands;

            // Cancel current command if running
            if (_currentCommand != null && _commandCts != null)
            {
                _commandCts.Cancel();
                _commandCts.Dispose();
                _commandCts = null;
                _currentCommand = null;
            }

            _logService?.LogInfo("CommandProcessor stopped.");
        }

        private void ProcessCommands()
        {
            if (!_isRunning || _currentCommand != null)
            {
                return;
            }

            int processedCount = 0;

            while (processedCount < MaxCommandsPerFrame && _queue.HasCommands)
            {
                var command = _queue.Dequeue();
                if (command == null)
                {
                    break;
                }

                // Check if command can execute
                if (!command.CanExecute())
                {
                    _logService?.LogWarning($"Command {command.CommandName} cannot execute (CanExecute returned false).");
                    continue;
                }

                // Execute command asynchronously
                ExecuteCommandAsync(command).Forget();
                processedCount++;
            }
        }

        private async UniTaskVoid ExecuteCommandAsync(ICommand command)
        {
            _currentCommand = command;
            _commandCts = new CancellationTokenSource();

            try
            {
                _logService?.LogInfo($"Executing command: {command.CommandName} (Priority: {command.Priority})");

                await command.ExecuteAsync(_commandCts.Token);

                command.OnComplete();
                _logService?.LogInfo($"Command completed: {command.CommandName}");
            }
            catch (OperationCanceledException)
            {
                _logService?.LogWarning($"Command cancelled: {command.CommandName}");
            }
            catch (Exception ex)
            {
                _logService?.LogError($"Command failed: {command.CommandName} - {ex}");
                command.OnFailed(ex);
            }
            finally
            {
                _commandCts?.Dispose();
                _commandCts = null;
                _currentCommand = null;
            }
        }
    }
}