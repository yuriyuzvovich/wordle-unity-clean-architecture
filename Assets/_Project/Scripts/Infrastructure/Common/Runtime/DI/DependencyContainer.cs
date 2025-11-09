using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wordle.Application.Interfaces;
using Wordle.Application.Attributes;

namespace Wordle.Infrastructure.Common.DI
{
    /// <summary>
    /// Dependency injection container implementation.
    /// Supports singleton, transient, and factory registrations with constructor and field injection.
    /// </summary>
    public class DependencyContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, Registration> _registrations = new ();
        private readonly Dictionary<Type, object> _singletonInstances = new ();
        private readonly DependencyContainer _parent;

        public DependencyContainer() : this(null) {}

        private DependencyContainer(DependencyContainer parent)
        {
            _parent = parent;
        }

        public void RegisterSingleton<TInterface>(TInterface instance)
        {
            var type = typeof(TInterface);
            _registrations[type] = new Registration(RegistrationType.Singleton, null, null);
            _singletonInstances[type] = instance;
        }

        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation: TInterface
        {
            var interfaceType = typeof(TInterface);
            var implementationType = typeof(TImplementation);
            _registrations[interfaceType] = new Registration(RegistrationType.Singleton, implementationType, null);
        }

        public void RegisterTransient<TInterface, TImplementation>() where TImplementation: TInterface
        {
            var interfaceType = typeof(TInterface);
            var implementationType = typeof(TImplementation);
            _registrations[interfaceType] = new Registration(RegistrationType.Transient, implementationType, null);
        }

        public void RegisterFactory<TInterface>(Func<IDependencyContainer, TInterface> factory)
        {
            var type = typeof(TInterface);
            _registrations[type] = new Registration(RegistrationType.Factory, null, c => factory(c));
        }

        public TInterface Resolve<TInterface>()
        {
            return (TInterface) Resolve(typeof(TInterface));
        }

        public object Resolve(Type type)
        {
            if (_registrations.TryGetValue(type, out var registration))
            {
                return ResolveRegistration(type, registration);
            }

            if (_parent != null)
            {
                return _parent.Resolve(type);
            }

            throw new InvalidOperationException($"Type {type.Name} is not registered in the container.");
        }

        public TImplementation Build<TImplementation>()
        {
            return (TImplementation) Build(typeof(TImplementation));
        }

        public object Build(Type type)
        {
            // Get the constructor with the most parameters (greedy algorithm)
            var constructor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {type.Name} has no public constructors.");
            }

            // Resolve constructor parameters
            var parameters = constructor.GetParameters();
            var resolvedParameters = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                resolvedParameters[i] = Resolve(parameters[i].ParameterType);
            }

            var instance = constructor.Invoke(resolvedParameters);

            // Inject fields/properties
            Inject(instance);

            return instance;
        }

        public void Inject(object target)
        {
            // Allow null targets: injecting into a null object is a no-op and should not throw.
            if (target == null)
            {
                return;
            }

            var type = target.GetType();

            // Inject fields
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<InjectAttribute>() != null);

            foreach (var field in fields)
            {
                var injectAttr = field.GetCustomAttribute<InjectAttribute>();

                try
                {
                    var value = Resolve(field.FieldType);
                    field.SetValue(target, value);
                }
                catch (Exception ex)
                {
                    if (!injectAttr.Optional)
                    {
                        throw new InvalidOperationException(
                            $"Failed to inject field {field.Name} in {type.Name}: {ex.Message}", ex
                        );
                    }
                }
            }

            // Inject properties
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<InjectAttribute>() != null && p.CanWrite);

            foreach (var property in properties)
            {
                var injectAttr = property.GetCustomAttribute<InjectAttribute>();

                try
                {
                    var value = Resolve(property.PropertyType);
                    property.SetValue(target, value);
                }
                catch (Exception ex)
                {
                    if (!injectAttr.Optional)
                    {
                        throw new InvalidOperationException(
                            $"Failed to inject property {property.Name} in {type.Name}: {ex.Message}", ex
                        );
                    }
                }
            }
        }

        public IDependencyContainer CreateScope()
        {
            return new DependencyContainer(this);
        }

        public bool IsRegistered<TInterface>()
        {
            return IsRegistered(typeof(TInterface));
        }

        public bool IsRegistered(Type type)
        {
            if (_registrations.ContainsKey(type))
                return true;

            return _parent?.IsRegistered(type) ?? false;
        }

        private object ResolveRegistration(Type type, Registration registration)
        {
            switch (registration.Type)
            {
                case RegistrationType.Singleton:
                    if (_singletonInstances.TryGetValue(type, out var instance))
                    {
                        return instance;
                    }

                    var newInstance = Build(registration.ImplementationType);
                    _singletonInstances[type] = newInstance;
                    return newInstance;

                case RegistrationType.Transient:
                    return Build(registration.ImplementationType);

                case RegistrationType.Factory:
                    return registration.Factory(this);

                default:
                    throw new InvalidOperationException($"Unknown registration type: {registration.Type}");
            }
        }

        private class Registration
        {
            public RegistrationType Type { get; }
            public Type ImplementationType { get; }
            public Func<IDependencyContainer, object> Factory { get; }

            public Registration(RegistrationType type, Type implementationType, Func<IDependencyContainer, object> factory)
            {
                Type = type;
                ImplementationType = implementationType;
                Factory = factory;
            }
        }

        private enum RegistrationType
        {
            Singleton,
            Transient,
            Factory
        }
    }
}