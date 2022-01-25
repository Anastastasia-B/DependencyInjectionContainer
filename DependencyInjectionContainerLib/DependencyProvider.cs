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
        class FindCycleResult
        {
            public object instance;
            public FieldInfo field;

            public FindCycleResult(object instance, FieldInfo field)
            {
                this.instance = instance;
                this.field = field;
            }
        }

        private DependenciesConfiguration _dependencies;
        private CycleDepDetector cycleDepDetector;
        private bool resolveCycleDep = false;

        public DependencyProvider(DependenciesConfiguration dependencies)
        {
            _dependencies = dependencies;
            cycleDepDetector = new CycleDepDetector();
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T), false);
        }

        private object Resolve(Type type, bool createAllImplementations)
        {
            object instance = null;
            if (cycleDepDetector.IsCycleDependencyDetected(type))
            {
                return null;
            }

            cycleDepDetector.PushType(type);

            List<Type> implementations = null;
            IDependencyLife dependencyLifeObject = null;
            if (!_dependencies.TryGetValue(type, out implementations))
            {
                if (type.GetInterface("IEnumerable") != null && type.IsGenericType)
                {
                    instance = Resolve(type.GetGenericArguments().First(), true);
                }
                else
                {
                    if(type.GetConstructors().Count() == 0)
                    {
                        instance = Activator.CreateInstance(type);
                    }
                    else
                    {
                        dependencyLifeObject = (IDependencyLife)ObjectCreator.CreateInstance(type, type.GetGenericArguments());
                        var instType = type.GenericTypeArguments[0];
                        var parameters = GetConstructorParams(instType, type.GetGenericTypeDefinition());
                        instance = dependencyLifeObject.GetInstance(parameters);
                    }
                }
            }
            else
            {
                if (createAllImplementations)
                {
                    instance = Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
                    for (int i = 0; i < implementations.Count; i++)
                    {
                        dependencyLifeObject = (IDependencyLife)ObjectCreator.CreateInstance(implementations[i], type.GetGenericArguments());
                        (instance as IList).Add(dependencyLifeObject.GetInstance(GetConstructorParams(implementations[i].GenericTypeArguments[0], implementations[i])));
                    }
                }
                else
                {
                    dependencyLifeObject = (IDependencyLife)ObjectCreator.CreateInstance(implementations[0], type.GetGenericArguments());
                    var parameters = GetConstructorParams(implementations[0].GenericTypeArguments[0], implementations[0].GetGenericTypeDefinition());
                    instance = dependencyLifeObject.GetInstance(parameters);
                }
            }
            cycleDepDetector.PopType();
            return instance;
        }

        private bool TryResolveCycleDep(List<object> constructorParams)
        {
            foreach(object parameter in constructorParams)
            {
                FindCycleResult result = FindCycle(parameter, parameter.GetType());
                if(result != null)
                {
                    result.field.SetValue(result.instance, parameter);
                    return true;
                }
            }
            return false;
        }

        private FindCycleResult FindCycle(object inst, Type type)
        {
            FieldInfo[] fields = inst.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.Equals(type))
                {
                    return new FindCycleResult(inst, field);
                }
                if(field.GetValue(inst) != null)
                {
                    FindCycleResult result = FindCycle(field.GetValue(inst), type);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private object[] GetConstructorParams(Type type, Type lifeType)
        {
            List<object> constructorParams = new List<object>();
            ConstructorInfo constructor = type.GetConstructors().OrderByDescending(con => con.GetParameters().Length).First();

            ParameterInfo[] parameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                Type paramType = parameters[i].ParameterType;
                var param = Resolve(lifeType.MakeGenericType(paramType), false);
                constructorParams.Add(param);

                if (lifeType.Equals(typeof(Singletone<>).GetGenericTypeDefinition()))
                {
                    if (resolveCycleDep && TryResolveCycleDep(constructorParams))
                    {
                        resolveCycleDep = false;
                    }
                    if (param == null)
                    {
                        resolveCycleDep = true;
                    }
                }
            }

            return constructorParams.ToArray();
        }
    }
}
