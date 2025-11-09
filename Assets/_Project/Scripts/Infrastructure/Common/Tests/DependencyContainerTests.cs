using System;
using NUnit.Framework;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.DI;

namespace Tests
{
    [TestFixture]
    public class DependencyContainerTests
    {
        private DependencyContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new DependencyContainer();
        }

        #region Singleton Registration Tests

        [Test]
        public void RegisterSingleton_WithInstance_ResolvesCorrectInstance()
        {
            var instance = new TestServiceA();
            _container.RegisterSingleton<ITestServiceA>(instance);

            var resolved = _container.Resolve<ITestServiceA>();

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void RegisterSingleton_WithInstance_ResolvesSameInstanceMultipleTimes()
        {
            var instance = new TestServiceA();
            _container.RegisterSingleton<ITestServiceA>(instance);

            var resolved1 = _container.Resolve<ITestServiceA>();
            var resolved2 = _container.Resolve<ITestServiceA>();

            Assert.AreSame(resolved1, resolved2);
        }

        [Test]
        public void RegisterSingleton_WithType_CreatesSingleInstance()
        {
            _container.RegisterSingleton<ITestServiceA, TestServiceA>();

            var resolved1 = _container.Resolve<ITestServiceA>();
            var resolved2 = _container.Resolve<ITestServiceA>();

            Assert.IsNotNull(resolved1);
            Assert.AreSame(resolved1, resolved2);
        }

        [Test]
        public void RegisterSingleton_WithType_PerformsConstructorInjection()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);
            _container.RegisterSingleton<ITestServiceB, TestServiceB>();

            var resolved = _container.Resolve<ITestServiceB>();

            Assert.IsNotNull(resolved);
            Assert.AreSame(logService, ((TestServiceB)resolved).LogService);
        }

        #endregion

        #region Transient Registration Tests

        [Test]
        public void RegisterTransient_CreatesNewInstanceEachTime()
        {
            _container.RegisterTransient<ITestServiceA, TestServiceA>();

            var resolved1 = _container.Resolve<ITestServiceA>();
            var resolved2 = _container.Resolve<ITestServiceA>();

            Assert.IsNotNull(resolved1);
            Assert.IsNotNull(resolved2);
            Assert.AreNotSame(resolved1, resolved2);
        }

        [Test]
        public void RegisterTransient_PerformsConstructorInjection()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);
            _container.RegisterTransient<ITestServiceB, TestServiceB>();

            var resolved = _container.Resolve<ITestServiceB>();

            Assert.IsNotNull(resolved);
            Assert.AreSame(logService, ((TestServiceB)resolved).LogService);
        }

        #endregion

        #region Factory Registration Tests

        [Test]
        public void RegisterFactory_UsesFactoryFunction()
        {
            var instance = new TestServiceA();
            _container.RegisterFactory<ITestServiceA>(c => instance);

            var resolved = _container.Resolve<ITestServiceA>();

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void RegisterFactory_PassesContainerToFactory()
        {
            IDependencyContainer capturedContainer = null;
            _container.RegisterFactory<ITestServiceA>(c =>
            {
                capturedContainer = c;
                return new TestServiceA();
            });

            _container.Resolve<ITestServiceA>();

            Assert.AreSame(_container, capturedContainer);
        }

        [Test]
        public void RegisterFactory_CanResolveDependenciesFromContainer()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);
            _container.RegisterFactory<ITestServiceB>(c => new TestServiceB(c.Resolve<ITestLogService>()));

            var resolved = _container.Resolve<ITestServiceB>();

            Assert.IsNotNull(resolved);
            Assert.AreSame(logService, ((TestServiceB)resolved).LogService);
        }

        #endregion

        #region Resolution Tests

        [Test]
        public void Resolve_UnregisteredType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _container.Resolve<ITestServiceA>());
        }

        [Test]
        public void Resolve_NonGeneric_ReturnsCorrectType()
        {
            var instance = new TestServiceA();
            _container.RegisterSingleton<ITestServiceA>(instance);

            var resolved = _container.Resolve(typeof(ITestServiceA));

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void Resolve_NonGeneric_UnregisteredType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _container.Resolve(typeof(ITestServiceA)));
        }

        #endregion

        #region Build Tests

        [Test]
        public void Build_CreatesInstanceWithConstructorInjection()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);

            var instance = _container.Build<TestServiceB>();

            Assert.IsNotNull(instance);
            Assert.AreSame(logService, instance.LogService);
        }

        [Test]
        public void Build_NonGeneric_CreatesInstanceWithConstructorInjection()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);

            var instance = _container.Build(typeof(TestServiceB));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<TestServiceB>(instance);
            Assert.AreSame(logService, ((TestServiceB)instance).LogService);
        }

        [Test]
        public void Build_TypeWithoutPublicConstructor_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _container.Build<TestServiceWithoutConstructor>());
        }

        [Test]
        public void Build_SelectsConstructorWithMostParameters()
        {
            var logService = new TestLogService();
            var serviceA = new TestServiceA();
            _container.RegisterSingleton<ITestLogService>(logService);
            _container.RegisterSingleton<ITestServiceA>(serviceA);

            var instance = _container.Build<TestServiceWithMultipleConstructors>();

            Assert.IsNotNull(instance);
            Assert.AreSame(logService, instance.LogService);
            Assert.AreSame(serviceA, instance.ServiceA);
        }

        #endregion

        #region Injection Tests

        [Test]
        public void Inject_InjectsPublicFields()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);

            var target = new TestClassWithFieldInjection();
            _container.Inject(target);

            Assert.AreSame(logService, target.PublicLogService);
        }

        [Test]
        public void Inject_InjectsPrivateFields()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);

            var target = new TestClassWithFieldInjection();
            _container.Inject(target);

            Assert.AreSame(logService, target.GetPrivateLogService());
        }

        [Test]
        public void Inject_InjectsPublicProperties()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);

            var target = new TestClassWithPropertyInjection();
            _container.Inject(target);

            Assert.AreSame(logService, target.PublicLogService);
        }

        [Test]
        public void Inject_InjectsPrivateProperties()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);

            var target = new TestClassWithPropertyInjection();
            _container.Inject(target);

            Assert.AreSame(logService, target.GetPrivateLogService());
        }

        [Test]
        public void Inject_OptionalField_UnregisteredDependency_DoesNotThrow()
        {
            var target = new TestClassWithOptionalInjection();

            Assert.DoesNotThrow(() => _container.Inject(target));
            Assert.IsNull(target.OptionalService);
        }

        [Test]
        public void Inject_RequiredField_UnregisteredDependency_ThrowsInvalidOperationException()
        {
            var target = new TestClassWithRequiredInjection();

            Assert.Throws<InvalidOperationException>(() => _container.Inject(target));
        }

        [Test]
        public void Inject_NullTarget_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _container.Inject(null));
        }

        [Test]
        public void Inject_ReadOnlyProperty_IsNotInjected()
        {
            var logService = new TestLogService();
            _container.RegisterSingleton<ITestLogService>(logService);

            var target = new TestClassWithReadOnlyProperty();
            _container.Inject(target);

            Assert.IsNull(target.ReadOnlyLogService);
        }

        #endregion

        #region Scope Tests

        [Test]
        public void CreateScope_CreatesChildContainer()
        {
            var scope = _container.CreateScope();

            Assert.IsNotNull(scope);
            Assert.IsInstanceOf<IDependencyContainer>(scope);
        }

        [Test]
        public void CreateScope_ChildCanResolveParentRegistrations()
        {
            var instance = new TestServiceA();
            _container.RegisterSingleton<ITestServiceA>(instance);

            var scope = _container.CreateScope();
            var resolved = scope.Resolve<ITestServiceA>();

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void CreateScope_ParentCannotResolveChildRegistrations()
        {
            var scope = _container.CreateScope();
            var instance = new TestServiceA();
            scope.RegisterSingleton<ITestServiceA>(instance);

            Assert.Throws<InvalidOperationException>(() => _container.Resolve<ITestServiceA>());
        }

        [Test]
        public void CreateScope_ChildRegistrationOverridesParent()
        {
            var parentInstance = new TestServiceA();
            var childInstance = new TestServiceA();

            _container.RegisterSingleton<ITestServiceA>(parentInstance);

            var scope = _container.CreateScope();
            scope.RegisterSingleton<ITestServiceA>(childInstance);

            var resolved = scope.Resolve<ITestServiceA>();

            Assert.AreSame(childInstance, resolved);
        }

        #endregion

        #region IsRegistered Tests

        [Test]
        public void IsRegistered_Generic_RegisteredType_ReturnsTrue()
        {
            _container.RegisterSingleton<ITestServiceA>(new TestServiceA());

            Assert.IsTrue(_container.IsRegistered<ITestServiceA>());
        }

        [Test]
        public void IsRegistered_Generic_UnregisteredType_ReturnsFalse()
        {
            Assert.IsFalse(_container.IsRegistered<ITestServiceA>());
        }

        [Test]
        public void IsRegistered_NonGeneric_RegisteredType_ReturnsTrue()
        {
            _container.RegisterSingleton<ITestServiceA>(new TestServiceA());

            Assert.IsTrue(_container.IsRegistered(typeof(ITestServiceA)));
        }

        [Test]
        public void IsRegistered_NonGeneric_UnregisteredType_ReturnsFalse()
        {
            Assert.IsFalse(_container.IsRegistered(typeof(ITestServiceA)));
        }

        [Test]
        public void IsRegistered_ChecksParentContainer()
        {
            _container.RegisterSingleton<ITestServiceA>(new TestServiceA());

            var scope = _container.CreateScope();

            Assert.IsTrue(scope.IsRegistered<ITestServiceA>());
        }

        #endregion

        #region Test Helper Classes

        public interface ITestServiceA { }
        public interface ITestServiceB { }
        public interface ITestLogService { }

        public class TestServiceA : ITestServiceA { }

        public class TestServiceB : ITestServiceB
        {
            public ITestLogService LogService { get; }

            public TestServiceB(ITestLogService logService)
            {
                LogService = logService;
            }
        }

        public class TestLogService : ITestLogService { }

        private class TestServiceWithoutConstructor
        {
            private TestServiceWithoutConstructor() { }
        }

        public class TestServiceWithMultipleConstructors
        {
            public ITestLogService LogService { get; }
            public ITestServiceA ServiceA { get; }

            public TestServiceWithMultipleConstructors(ITestLogService logService)
            {
                LogService = logService;
            }

            public TestServiceWithMultipleConstructors(ITestLogService logService, ITestServiceA serviceA)
            {
                LogService = logService;
                ServiceA = serviceA;
            }
        }

        public class TestClassWithFieldInjection
        {
            [Inject] public ITestLogService PublicLogService;
            [Inject] private ITestLogService _privateLogService;

            public ITestLogService GetPrivateLogService() => _privateLogService;
        }

        public class TestClassWithPropertyInjection
        {
            [Inject] public ITestLogService PublicLogService { get; set; }
            [Inject] private ITestLogService PrivateLogService { get; set; }

            public ITestLogService GetPrivateLogService() => PrivateLogService;
        }

        public class TestClassWithOptionalInjection
        {
            [Inject(Optional = true)] public ITestServiceA OptionalService;
        }

        public class TestClassWithRequiredInjection
        {
            [Inject] public ITestServiceA RequiredService;
        }

        public class TestClassWithReadOnlyProperty
        {
            [Inject] public ITestLogService ReadOnlyLogService { get; }
        }

        #endregion
    }
}
