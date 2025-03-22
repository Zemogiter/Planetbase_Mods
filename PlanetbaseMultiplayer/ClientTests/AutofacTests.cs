﻿using NUnit.Framework;
using PlanetbaseMultiplayer.Client.Autofac;
using PlanetbaseMultiplayer.Client.GameStates;
using PlanetbaseMultiplayer.Model;
using PlanetbaseMultiplayer.Model.Autofac;
using System.Runtime.Serialization;
using System;
using UnityEngine.Assertions;

namespace PlanetbaseMultiplayer.Client.Tests
{
    public class AutofacTests
    {
        [Test]
        public void ResolveClientTest()
        {
            GameStateMultiplayer? gameStateMultiplayer = (GameStateMultiplayer)FormatterServices.GetUninitializedObject(typeof(GameStateMultiplayer));
            ServiceLocator serviceLocator = new ServiceLocator();

            ClientAutoFacRegistrar clientAutoFacRegistrar = new(serviceLocator, gameStateMultiplayer);

            serviceLocator.Initialize(clientAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            try
            {
                Client client = serviceLocator.LocateService<Client>();
            }
            catch (Exception ex)
            {
                NUnit.Framework.Assert.Fail($"Failed to resolve Client service: {ex}");
            }
        }

        [Test]
        public void ResolveManagersTest()
        {
            GameStateMultiplayer gameStateMultiplayer = (GameStateMultiplayer)FormatterServices.GetUninitializedObject(typeof(GameStateMultiplayer));
            ServiceLocator serviceLocator = new ServiceLocator();

            ClientAutoFacRegistrar clientAutoFacRegistrar = new ClientAutoFacRegistrar(serviceLocator, gameStateMultiplayer);

            serviceLocator.Initialize(clientAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            Type currentType = null;

            try
            {
                foreach (Type managerType in serviceLocator.GetDerivedServiceTypes<IManager>())
                {
                    currentType = managerType;
                    object service = serviceLocator.LocateService(managerType);
                }
            }
            catch (Exception ex)
            {
                NUnit.Framework.Assert.Fail($"Failed to resolve {currentType.FullName}: {ex}");
            }
        }

        [Test]
        public void ManagersCountNotEmpty()
        {
            GameStateMultiplayer? gameStateMultiplayer = FormatterServices.GetUninitializedObject(typeof(GameStateMultiplayer)) as GameStateMultiplayer;
            ServiceLocator serviceLocator = new ServiceLocator();

            ClientAutoFacRegistrar clientAutoFacRegistrar = new ClientAutoFacRegistrar(serviceLocator, gameStateMultiplayer);

            serviceLocator.Initialize(clientAutoFacRegistrar);
            serviceLocator.BeginLifetimeScope();

            if (serviceLocator.GetDerivedServiceTypes<IManager>().Count == 0)
                NUnit.Framework.Assert.Fail("Current ServiceLocator does not contain any registered managers");
        }
    }
}