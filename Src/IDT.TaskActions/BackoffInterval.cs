using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDT.TaskActions
{
    /// <summary>
    /// TimeSpan = smaller of (resetInterval * (count ^ scalingFactor)) or maxInterval
    /// </summary>
    public class BackoffInterval : IInterval
    {
        private readonly TimeSpan _resetInterval;
        private readonly TimeSpan _maxInterval;
        private readonly int _scalingFactor;

        public BackoffInterval(TimeSpan resetInterval, TimeSpan maxInterval, int scalingFactor)
        {
            _resetInterval = resetInterval;
            _maxInterval = maxInterval;
            _scalingFactor = scalingFactor;
        }

        public TimeSpan Calculate(int count)
        {
            var currentReset = new TimeSpan(_resetInterval.Ticks * Convert.ToInt64(Math.Pow(_scalingFactor, count)));
            return currentReset < _maxInterval ? currentReset : _maxInterval;
        }

        public static BackoffInterval Default
        {
            get { return new BackoffInterval(TimeSpan.FromSeconds(30), TimeSpan.FromHours(1), 2); }
        }
    }
}
