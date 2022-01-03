using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using DependencyInjectionContainerLib.Reflection;
using DependencyInjectionContainerLib.Implementation;

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
            return (T)Resolve(typeof(T), false);
        }

        private object Resolve(Type type, bool createAllImplementations)
        {
            object instance = null;
            List<Type> implementations = null;
            if (!_dependencies.TryGetValue(type, out implementations))
            {
                if (type.GetInterface("IEnumerable") != null && type.IsGenericType)
                {
                    instance = Resolve(type.GetGenericArguments().First(), true);
                }
            }
            else
            {
                IDependencyLife dependencyLifeObject = null;
                if (createAllImplementations)
                {
                    instance = Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                    for (int i = 0; i < implementations.Count; i++)
                    {
                        dependencyLifeObject = (IDependencyLife)ObjectCreator.CreateInstance(implementations[i], type.GetGenericArguments());
                        (instance as IList).Add(dependencyLifeObject.GetInstance(GetConstructorParams(implementations[i].GenericTypeArguments[0])));
                    }
                }
                else
                {
                    dependencyLifeObject = (IDependencyLife)ObjectCreator.CreateInstance(implementations[0], type.GetGenericArguments());
                    instance = dependencyLifeObject.GetInstance(GetConstructorParams(implementations[0].GenericTypeArguments[0]));
                }
            }
            return instance;
        }

        private object[] GetConstructorParams(Type type)
        {
            List<object> constructorParams = new List<object>();
            ConstructorInfo constructor = type.GetConstructors().OrderByDescending(con => con.GetParameters().Length).First();

            ParameterInfo[] parameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                constructorParams.Add(Resolve(parameters[i].ParameterType, false));
            }

            return constructorParams.ToArray();
        }
    }
}
