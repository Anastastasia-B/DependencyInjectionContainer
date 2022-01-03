using System;
using System.Collections.Generic;
using System.Text;
using DependencyInjectionContainerLib.Reflection;

namespace DependencyInjectionContainerLib.Implementation
{
    internal class InstancePerDependency<T> : IDependencyLife
    {
        object IDependencyLife.GetInstance(object[] constructorParams)
        {
            return ObjectCreator.CreateInstance(typeof(T), constructorParams);
        }
    }
}
