using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionContainerLib.Reflection
{
    internal static class ObjectCreator
    {
        internal static object CreateInstance(Type type, object[] constructorParams)
        {
            return CreateInstance(type, Array.Empty<Type>(), constructorParams);
        }

        internal static object CreateInstance(Type type, Type[] genericArguments)
        {
            return CreateInstance(type, genericArguments, Array.Empty<object>());
        }

        internal static object CreateInstance(Type type, Type[] genericArguments, object[] constructorParams)
        {
            return Activator.CreateInstance(CreateType(type, genericArguments), constructorParams);
        }

        private static Type CreateType(Type baseType, Type[] geneticArguments)
        {
            Type instance = baseType;
            if (baseType.IsGenericTypeDefinition)
            {
                instance = baseType.MakeGenericType(geneticArguments);
            }
            else
            {
                if (baseType.IsGenericType)
                {
                    Type[] currentStepGenericArgs = baseType.GetGenericArguments().Select(arg => CreateType(arg, geneticArguments)).ToArray();
                    instance = baseType.GetGenericTypeDefinition().MakeGenericType(currentStepGenericArgs);
                }
            }
            return instance;
        }
    }
}
