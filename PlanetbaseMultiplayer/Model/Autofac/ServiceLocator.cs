﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseMultiplayer.Model.Autofac
{
    public class ServiceLocator
    {
        private IContainer container;
        private ILifetimeScope lifetimeScope;

        public ServiceLocator(params IAutoFacRegistrar[] registrars)
        {
            Initialize(registrars);
        }

        public ServiceLocator()
        { }

        public void Initialize(params IAutoFacRegistrar[] registrars)
        {
            if (container != null)
                throw new InvalidOperationException("Service locator is already initialized.");
            
            ContainerBuilder containerBuilder = new ContainerBuilder();

            foreach (var registrar in registrars)
                registrar.RegisterComponents(containerBuilder);

            container = containerBuilder.Build();
        }

        public void BeginLifetimeScope()
        {
            if (container == null)
                throw new InvalidOperationException("You must initialize the service locator before starting a new lifetime scope.");

            EndLifetimeScope();
            lifetimeScope = container.BeginLifetimeScope();
        }

        public void EndLifetimeScope()
        {
            lifetimeScope?.Dispose();
        }

        public bool LifetimeScopeExists()
        {
            return lifetimeScope != null;
        }

        private void AssertLifetimeScopeExists()
        {
            if (!LifetimeScopeExists())
                throw new InvalidOperationException("You must create a new lifetime scope before resolving services.");
        }

        public T LocateService<T>() where T : class
        {
            AssertLifetimeScopeExists();
            return lifetimeScope.Resolve<T>();
        }

        public object LocateService(Type serviceType)
        {
            AssertLifetimeScopeExists();
            return lifetimeScope.Resolve(serviceType);
        }

        public List<T> LocateServicesOfType<T>() where T : class
        {
            AssertLifetimeScopeExists();
            return lifetimeScope.ComponentRegistry.Registrations
                .Where(r => typeof(T).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType)
                .Select(t => lifetimeScope.Resolve(t) as T).ToList();
        }

        public List<Type> GetDerivedServiceTypes<T>() where T : class
        {
            AssertLifetimeScopeExists();
            return lifetimeScope.ComponentRegistry.Registrations
                .Where(r => typeof(T).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType).ToList();
        }
    }
}
