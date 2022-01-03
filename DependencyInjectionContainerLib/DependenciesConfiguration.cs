using System;
using System.Linq;
using System.Collections.Generic;
using DependencyInjectionContainerLib.Implementation;

namespace DependencyInjectionContainerLib
{
    public class DependenciesConfiguration
    {
        private Dictionary<int, List<Type>> _dependencies;

        public DependenciesConfiguration()
        {
            _dependencies = new Dictionary<int, List<Type>>();
        }

        public void Register<T, F>()
        {
            Register(typeof(T), typeof(F));
        }

        public void Register(Type dependencyType, Type implementationType)
        {
            int dependencyMetadataToken = dependencyType.MetadataToken;
            int implementationMetadataToken = implementationType.MetadataToken;
            if (_dependencies.ContainsKey(dependencyMetadataToken))
            {
                if (!_dependencies[dependencyMetadataToken].Where(impl => impl.GenericTypeArguments.First().MetadataToken == implementationMetadataToken).Any())
                {
                    _dependencies[dependencyMetadataToken].Add(implementationType);
                }
            }
            else
            {
                _dependencies.Add(dependencyMetadataToken, new List<Type> { implementationType });
            }
        }

        internal bool TryGetValue(Type type, out List<Type> implementations)
        {
            implementations = null;
            int typeMetadataToken = type.MetadataToken;
            if (_dependencies.ContainsKey(typeMetadataToken))
            {
                implementations = _dependencies[typeMetadataToken];
            }
            return implementations != null;
        }
    }
}
