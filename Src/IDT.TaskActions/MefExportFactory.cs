using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public class MefExportFactory<T> : ExportFactory<T>
    {
        public MefExportFactory(Func<CompositionContainer> exportLifetimeContextCreator)
            : base(() => ConvertToTuple<T>(exportLifetimeContextCreator()))
        {
        }

        private static Tuple<TAction, Action> ConvertToTuple<TAction>(CompositionContainer container)
        {
            var item = container.GetExportedValue<TAction>();
            return new Tuple<TAction, Action>(item,container.Dispose);
        }
    }
}
