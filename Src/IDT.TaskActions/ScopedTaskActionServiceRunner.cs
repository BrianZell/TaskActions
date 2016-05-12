using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public class ScopedTaskActionServiceRunner<T> : TaskActionServiceRunner
        where T : ITaskAction
    {
        public ScopedTaskActionServiceRunner(ExportFactory<T> actionFactory, IExceptionHandler exceptionHandler)
            : base(new ScopedAction<T>(actionFactory), exceptionHandler)
        {
        }
    }
}
