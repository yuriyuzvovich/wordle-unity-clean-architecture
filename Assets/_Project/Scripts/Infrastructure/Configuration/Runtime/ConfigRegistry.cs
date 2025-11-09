using System;
using System.Collections.Generic;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Configuration
{
    public class ConfigRegistry : IConfigRegistry
    {
        private readonly Dictionary<string, object> _configs;
        private readonly ILogService _logService;

        public ConfigRegistry(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _configs = new Dictionary<string, object>();
        }

        public void Register<TConfig>(TConfig config) where TConfig: class
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var key = GetTypeKey<TConfig>();
            Register(key, config);
        }

        public void Register<TConfig>(string key, TConfig config) where TConfig: class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (_configs.ContainsKey(key))
            {
                _logService.LogWarning($"Config with key '{key}' already registered. Overwriting.");
            }

            _configs[key] = config;
            _logService.LogInfo($"Registered config: {key} (Type: {typeof(TConfig).Name})");
        }

        public TConfig Get<TConfig>() where TConfig: class
        {
            var key = GetTypeKey<TConfig>();
            return Get<TConfig>(key);
        }

        public TConfig Get<TConfig>(string key) where TConfig: class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            if (!_configs.TryGetValue(key, out var config))
            {
                throw new InvalidOperationException($"Config with key '{key}' is not registered");
            }

            if (config is TConfig typedConfig)
            {
                return typedConfig;
            }

            throw new InvalidOperationException($"Config with key '{key}' is not of type {typeof(TConfig).Name}");
        }

        public bool TryGet<TConfig>(out TConfig config) where TConfig: class
        {
            var key = GetTypeKey<TConfig>();
            return TryGet(key, out config);
        }

        public bool TryGet<TConfig>(string key, out TConfig config) where TConfig: class
        {
            config = null;

            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (!_configs.TryGetValue(key, out var obj))
            {
                return false;
            }

            if (obj is TConfig typedConfig)
            {
                config = typedConfig;
                return true;
            }

            return false;
        }

        public bool IsRegistered<TConfig>() where TConfig: class
        {
            var key = GetTypeKey<TConfig>();
            return IsRegistered<TConfig>(key);
        }

        public bool IsRegistered<TConfig>(string key) where TConfig: class
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (!_configs.TryGetValue(key, out var config))
            {
                return false;
            }

            return config is TConfig;
        }

        private string GetTypeKey<TConfig>()
        {
            return typeof(TConfig).FullName;
        }
    }
}