using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;

namespace IDT.TaskActions.Unity.Tests.TestCases
{
    public class UnityContainerScopedActionTests
    {
        //[Test]
        //public void RunAction_DisposesOfContainer()
        //{
        //    var container = Substitute.For<IUnityContainer>();
        //    container.Resolve(null, null).ReturnsForAnyArgs(new TestResolveClass());
        //    var sut = new UnityContainerScopedAction<TestResolveClass>(() => container);

        //    sut.RunAction(CancellationToken.None);

        //    container.Received().Dispose();
        //}

        //[Test]
        //public void RunAction_RunsConstructedClassWithProvidedCancellationToken()
        //{
        //    var resolvedClass = new TestResolveClass();
        //    var container = Substitute.For<IUnityContainer>();
        //    container.Resolve(null, null).ReturnsForAnyArgs(resolvedClass);
        //    var sut = new UnityContainerScopedAction<TestResolveClass>(() => container);
        //    var cancellationToken = CancellationToken.None;

        //    sut.RunAction(cancellationToken);

        //    Assert.That(resolvedClass.Token,Is.EqualTo(cancellationToken));
        //}

        //public class TestResolveClass : ITaskAction
        //{
        //    public CancellationToken Token { get; set; }

        //    public Task RunAction(CancellationToken cancellationToken)
        //    {
        //        Token = cancellationToken;
        //        return Task.FromResult(string.Empty);
        //    }
        //}
    }
}
