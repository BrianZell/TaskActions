using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace IDT.TaskActions.Unity
{
    public class UnityTaskActionServiceRunner<TAction> : TaskActionServiceRunner
        where TAction : ITaskAction
    {
        public UnityTaskActionServiceRunner(Func<IUnityContainer> containerFactory, IExceptionHandler exceptionHandler)
            : base(new UnityContainerScopedAction<TAction>(containerFactory), exceptionHandler)
        {
        }
    }
}
