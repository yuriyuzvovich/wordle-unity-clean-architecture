namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Processes commands from the command queue every frame.
    /// </summary>
    public interface ICommandProcessor
    {
        /// <summary>
        /// Start processing commands.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop processing commands.
        /// </summary>
        void Stop();

        /// <summary>
        /// Check if the processor is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Maximum number of commands to process per frame.
        /// Prevents frame rate drops from too many commands.
        /// </summary>
        int MaxCommandsPerFrame { get; set; }
    }
}
