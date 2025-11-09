using System;
using UnityEngine;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Common.Lifecycle
{
    public class UnityLifecycleProvider : MonoBehaviour, IEngineLifecycle
    {
        public event Action OnFrameTick;
        public event Action OnFixedFrameTick;
        public event Action OnLateFrameTick;
        public event Action<bool> OnApplicationPaused;
        public event Action<bool> OnApplicationFocused;
        public event Action OnApplicationQuitted;

        public bool IsRunning { get; private set; } = true;
        public float DeltaTime => Time.deltaTime;
        public float FixedDeltaTime => Time.fixedDeltaTime;
        public int FrameCount => Time.frameCount;

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                OnFrameTick?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in OnFrameTick: {ex}");
            }
        }

        private void FixedUpdate()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                OnFixedFrameTick?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in OnFixedFrameTick: {ex}");
            }
        }

        private void LateUpdate()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                OnLateFrameTick?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in OnLateFrameTick: {ex}");
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            try
            {
                IsRunning = !pauseStatus;
                OnApplicationPaused?.Invoke(pauseStatus);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in OnApplicationPaused: {ex}");
            }
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            try
            {
                OnApplicationFocused?.Invoke(focusStatus);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in OnApplicationFocused: {ex}");
            }
        }

        private void OnApplicationQuit()
        {
            try
            {
                IsRunning = false;
                OnApplicationQuitted?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in OnApplicationQuitted: {ex}");
            }
        }

        private void OnDestroy()
        {
            // Clean up all event subscriptions
            OnFrameTick = null;
            OnFixedFrameTick = null;
            OnLateFrameTick = null;
            OnApplicationPaused = null;
            OnApplicationFocused = null;
            OnApplicationQuitted = null;
        }
    }
}