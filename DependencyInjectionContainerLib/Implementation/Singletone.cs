using System;
using System.Collections.Generic;
using System.Text;
using DependencyInjectionContainerLib.Reflection;

namespace DependencyInjectionContainerLib.Implementation
{
    internal class Singletone<T> : IDependencyLife
    {
        private static T instance;
        private readonly object obj = new object();

        object IDependencyLife.GetInstance(object[] constructorParams)
        {
            if (instance == null)
            {
                lock (obj)
                {
                    if (instance == null)
                    {
                        instance = (T)ObjectCreator.CreateInstance(typeof(T), constructorParams);
                    }
                }
            }
            return instance;
        }
    }
}
