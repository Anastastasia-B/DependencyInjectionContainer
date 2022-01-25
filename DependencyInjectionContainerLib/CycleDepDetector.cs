using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionContainerLib
{
    class CycleDepDetector
    {
        private readonly Stack<Type> typeStack = new Stack<Type>();
        private int depCount = 0;

        public bool IsCycleDependencyDetected(Type type)
        {
            if (typeStack.Contains(type))
            {
                depCount++;
                if (depCount > 2)
                {
                    depCount = 0;
                    return true;
                }
            }

            return false;
        }

        public void PushType(Type type)
        {
            typeStack.Push(type);
        }

        public void PopType()
        {
            if (typeStack.Count > 0)
            {
                typeStack.Pop();
            }
        }
    }
}
