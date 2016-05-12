using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace IDT.TaskActions.Unity
{
    public class UnityExportFactory<T> : ExportFactory<T>
    {
        public UnityExportFactory(Func<IUnityContainer> containerFactory)
            : base(() => CreateTuple<T>(containerFactory()))
        {
        }

        private static Tuple<TAction,Action> CreateTuple<TAction>(IUnityContainer container)
        {
            return new Tuple<TAction,Action>(container.Resolve<TAction>(), () => container.Dispose());
        }
    }
}
