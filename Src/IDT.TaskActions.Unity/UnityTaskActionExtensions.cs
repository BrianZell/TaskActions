using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace IDT.TaskActions.Unity
{
    public static class UnityTaskActionExtensions
    {
        public static IUnityContainer RegisterMany<TFrom,TTo>(this IUnityContainer source) 
            where TTo : TFrom
        {
            if (!source.IsRegistered(typeof(IEnumerable<TFrom>)))
            {
                source.RegisterType<IEnumerable<TFrom>, TFrom[]>();
            }

            return source.RegisterType<TFrom, TTo>(nameof(TTo));
        }

        public static IUnityContainer RegisterExportFactory<T>(this IUnityContainer container)
        {
            return container.RegisterType<ExportFactory<T>>(new InjectionFactory(CreateExportFactory<T>));
        }

        private static ExportFactory<T> CreateExportFactory<T>(IUnityContainer container, Type type, string name)
        {
            Func<IUnityContainer> beginScope = container.CreateChildContainer;

            var factory = new ExportFactory<T>(() =>
            {
                var scope = beginScope();
                if (name == null)
                {
                    return Tuple.Create(scope.Resolve<T>(), new Action(scope.Dispose));
                }

                return Tuple.Create(scope.Resolve<T>(name), new Action(scope.Dispose));
            });

            return factory;
        }
    }
}
