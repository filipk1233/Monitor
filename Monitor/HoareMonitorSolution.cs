using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonitorImplementation.HoareMonitor
{
    public class HoareMonitorSolution : HoareMonitorImplementation
    {
        protected ISignal signal;
        protected ICondition condition;

        public HoareMonitorSolution()
        {
            signal = CreateSignal();
            condition = GetCondition();
        }

        public void SendSignal()
        {
            signal.Send();
        }

        public void WaitSignal()
        {
            signal.Wait();
        }

        public bool AwaitSignal()
        {
            return signal.Await();
        }

        public void SendCondition()
        {
            condition.Send();
        }

        public void WaitCondition()
        {
            condition.Wait();
        }

        public bool AwaitCondition()
        {
            return condition.Await();
        }
    }
}
