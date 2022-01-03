using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace DependencyInjectionContainerLib
{
    public class DependencyProvider
    {
        private DependenciesConfiguration _dependencies;
        public DependencyProvider(DependenciesConfiguration dependencies)
        {
            _dependencies = dependencies;
        }

        public T Resolve<T>()
        {
            return default;
        }
    }
}
