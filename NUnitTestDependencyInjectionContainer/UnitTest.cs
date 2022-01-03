using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using DependencyInjectionContainerLib;

namespace NUnitTestDependencyInjectionContainer
{
    public interface ITest1 { }

    public class Test1Impl1 : ITest1 { }

    public class Test1Impl2 : ITest1 { }

    public interface ITest2 { }

    public class Test2Impl1 : ITest2
    {
        public ITest1 test1;

        public Test2Impl1(ITest1 test1)
        {
            this.test1 = test1;
        }
    }

    public interface ITest3<T> { }

    public class Test3Impl1<T> : ITest3<T> { }

    public class Test3Impl2<T> : ITest3<T>
        where T : ITest1
    {
        public T test1;
        public Test3Impl2(T test1)
        {
            this.test1 = test1;
        }
    }

    public class Tests
    {
        [Test]
        public void CheckDependecny()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<ITest1, Test1Impl1>(DependencyLifeTime.InstancePerDependency);
            var provider = new DependencyProvider(dependencies);
            var result = provider.Resolve<ITest1>();

            Assert.NotNull(result);
        }

        [Test]
        public void CheckDependencies()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<ITest1, Test1Impl1>(DependencyLifeTime.InstancePerDependency);
            dependencies.Register<ITest1, Test1Impl2>(DependencyLifeTime.InstancePerDependency);
            var provider = new DependencyProvider(dependencies);
            ITest1[] result = provider.Resolve<IEnumerable<ITest1>>().ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.Where(obj => obj.GetType().Equals(typeof(Test1Impl1))).Any());
            Assert.IsTrue(result.Where(obj => obj.GetType().Equals(typeof(Test1Impl2))).Any());
        }

        [Test]
        public void CheckConstructorParamWithoutImplemetation()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<ITest2, Test2Impl1>(DependencyLifeTime.InstancePerDependency);
            var provider = new DependencyProvider(dependencies);
            var obj = provider.Resolve<ITest2>();

            Assert.NotNull(obj);
            Assert.Null((obj as Test2Impl1).test1);
        }

        [Test]
        public void CheckConstructorParamWithImplementation()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<ITest2, Test2Impl1>(DependencyLifeTime.InstancePerDependency);
            dependencies.Register<ITest1, Test1Impl1>(DependencyLifeTime.InstancePerDependency);
            var provider = new DependencyProvider(dependencies);
            var obj = provider.Resolve<ITest2>();

            Assert.NotNull(obj);
            Assert.NotNull((obj as Test2Impl1).test1);
        }

        [Test]
        public void CheckGenericDependency()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register(typeof(ITest3<>), typeof(Test3Impl1<>), DependencyLifeTime.InstancePerDependency);
            var provider = new DependencyProvider(dependencies);
            var obj = provider.Resolve<ITest3<int>>();

            Assert.NotNull(obj);
            Assert.AreEqual(typeof(int), obj.GetType().GenericTypeArguments.First());
        }
    }
}