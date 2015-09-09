using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    public interface IInterval
    {
        TimeSpan Calculate(int count);
    }
}
