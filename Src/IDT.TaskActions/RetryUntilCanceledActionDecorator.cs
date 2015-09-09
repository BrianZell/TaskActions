using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public class RetryUntilCanceledActionDecorator : HandleExceptionAndRetryActionDecorator
    {
        public RetryUntilCanceledActionDecorator(ITaskAction action, IExceptionHandler exceptionHandler)
            : base(action, exceptionHandler)
        {            
        }

        protected override bool ShouldRetryAfterNExceptions(int consecutiveExceptions)
        {
            return true;
        }
    }
}
