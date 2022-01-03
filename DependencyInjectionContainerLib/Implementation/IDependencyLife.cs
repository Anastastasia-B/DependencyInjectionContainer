using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionContainerLib.Implementation
{
    internal interface IDependencyLife
    {
        internal object GetInstance(object[] constructorParams);
    }
}
