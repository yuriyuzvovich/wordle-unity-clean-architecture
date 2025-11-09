using System;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Dependency injection container interface.
    /// Supports constructor injection, field injection, and factory patterns.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// Register a singleton instance directly.
        /// </summary>
        void RegisterSingleton<TInterface>(TInterface instance);

        /// <summary>
        /// Register a singleton by type mapping.
        /// </summary>
        void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface;

        /// <summary>
        /// Register a transient type (new instance each resolve).
        /// </summary>
        void RegisterTransient<TInterface, TImplementation>() where TImplementation : TInterface;

        /// <summary>
        /// Register a factory function for creating instances.
        /// </summary>
        void RegisterFactory<TInterface>(Func<IDependencyContainer, TInterface> factory);

        /// <summary>
        /// Resolve a registered interface/type.
        /// </summary>
        TInterface Resolve<TInterface>();

        /// <summary>
        /// Resolve a registered type by Type.
        /// </summary>
        object Resolve(Type type);

        /// <summary>
        /// Build an instance with constructor injection (doesn't need to be registered).
        /// </summary>
        TImplementation Build<TImplementation>();

        /// <summary>
        /// Build an instance by Type with constructor injection.
        /// </summary>
        object Build(Type type);

        /// <summary>
        /// Inject dependencies into an existing object's fields/properties marked with [Inject].
        /// </summary>
        void Inject(object target);

        /// <summary>
        /// Create a child container (scoped container).
        /// </summary>
        IDependencyContainer CreateScope();

        /// <summary>
        /// Check if a type is registered.
        /// </summary>
        bool IsRegistered<TInterface>();

        /// <summary>
        /// Check if a type is registered by Type.
        /// </summary>
        bool IsRegistered(Type type);
    }
}
