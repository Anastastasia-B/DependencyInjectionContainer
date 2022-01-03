using System;
using System.Linq;
using System.Collections.Generic;

namespace DependencyInjectionContainerLib
{
    public class DependenciesConfiguration
    {
        public DependenciesConfiguration()
        {
            
        }

        public void Register<T, F>()
        {
            Register(typeof(T), typeof(F));
        }

        public void Register(Type dependencyType, Type implementationType)
        {
        }
    }
}
