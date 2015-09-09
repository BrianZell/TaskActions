using System;

namespace IDT.TaskActions
{
    public interface IExceptionHandler
    {
        void HandleException(Exception exception);
    }
}
