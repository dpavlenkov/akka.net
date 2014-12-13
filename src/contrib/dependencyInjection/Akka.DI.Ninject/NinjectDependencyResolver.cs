﻿using Akka.Actor;
using Akka.DI.Core;
using Ninject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akka.DI.Ninject
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
       IKernel container;

        private ConcurrentDictionary<string, Type> typeCache;

        public NinjectDependencyResolver(IKernel container)
        {
            if (container == null) throw new ArgumentNullException("container");
            this.container = container;
            typeCache = new ConcurrentDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
        }

        public Type GetType(string actorName)
        {
            typeCache.TryAdd(actorName, actorName.GetTypeValue());

            return typeCache[actorName];
        }

        public Func<ActorBase> CreateActorFactory(string actorName)
        {
            return () =>
            {
                Type actorType = this.GetType(actorName);

                return (ActorBase)container.GetService(actorType);
            };
        }   
    }
    internal static class Extensions
    {
        public static Type GetTypeValue(this string typeName)
        {
            var firstTry = Type.GetType(typeName);
            Func<Type> searchForType = () =>
            {
                return
                AppDomain.
                    CurrentDomain.
                    GetAssemblies().
                    SelectMany(x => x.GetTypes()).
                    Where(t => t.Name.Equals(typeName)).
                    FirstOrDefault();
            };
            return firstTry ?? searchForType();
        }

    }
}
