using System;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Abstraction over Unity's lifecycle events.
    /// Allows pure C# classes to subscribe to frame updates without being MonoBehaviours.
    /// </summary>
    public interface IEngineLifecycle
    {
        /// <summary>
        /// Invoked every frame (Update).
        /// </summary>
        event Action OnFrameTick;

        /// <summary>
        /// Invoked at fixed time intervals (FixedUpdate).
        /// </summary>
        event Action OnFixedFrameTick;

        /// <summary>
        /// Invoked after all Update calls (LateUpdate).
        /// </summary>
        event Action OnLateFrameTick;

        /// <summary>
        /// Invoked when application is paused/resumed.
        /// </summary>
        event Action<bool> OnApplicationPaused;

        /// <summary>
        /// Invoked when application gains/loses focus.
        /// </summary>
        event Action<bool> OnApplicationFocused;

        /// <summary>
        /// Invoked when application is quitting.
        /// </summary>
        event Action OnApplicationQuitted;

        /// <summary>
        /// True if the engine is running (not paused, not quitting).
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Time since last frame (Time.deltaTime).
        /// </summary>
        float DeltaTime { get; }

        /// <summary>
        /// Fixed timestep interval (Time.fixedDeltaTime).
        /// </summary>
        float FixedDeltaTime { get; }

        /// <summary>
        /// Current frame count.
        /// </summary>
        int FrameCount { get; }
    }
}
