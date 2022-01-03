using System;
using System.Linq;
using System.Collections.Generic;
using DependencyInjectionContainerLib.Implementation;

namespace DependencyInjectionContainerLib
{
    public enum DependencyLifeTime
    {
        InstancePerDependency,
        Singletone
    }

    public class DependenciesConfiguration
    {
        private static Dictionary<DependencyLifeTime, Type> _lifeTypes;
        private Dictionary<int, List<Type>> _dependencies;

        public DependenciesConfiguration()
        {
            _lifeTypes = new Dictionary<DependencyLifeTime, Type>()
            {
                { DependencyLifeTime.InstancePerDependency, typeof(InstancePerDependency<>) },
                { DependencyLifeTime.Singletone, typeof(Singletone<>) }
            };
            _dependencies = new Dictionary<int, List<Type>>();
        }

        public void Register<T, F>(DependencyLifeTime dependencyLifeTime)
        {
            Register(typeof(T), typeof(F), dependencyLifeTime);
        }

        public void Register(Type dependencyType, Type implementationType, DependencyLifeTime dependencyLifeTime)
        {
            int dependencyMetadataToken = dependencyType.MetadataToken;
            int implementationMetadataToken = implementationType.MetadataToken;
            if (_dependencies.ContainsKey(dependencyMetadataToken))
            {
                if (!_dependencies[dependencyMetadataToken].Where(impl => impl.GenericTypeArguments.First().MetadataToken == implementationMetadataToken).Any())
                {
                    _dependencies[dependencyMetadataToken].Add(_lifeTypes[dependencyLifeTime].MakeGenericType(implementationType));
                }
            }
            else
            {
                _dependencies.Add(dependencyMetadataToken, new List<Type> { _lifeTypes[dependencyLifeTime].MakeGenericType(implementationType) });
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
