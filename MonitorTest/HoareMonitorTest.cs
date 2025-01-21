using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorImplementation.HoareMonitor;

namespace MonitorTest
{
    [TestClass]
    public class HoareMonitorTest
    {
        private HoareMonitorSolution monitor = new();

        [TestMethod]
        public void InterceptTest()
        {
            int interceptCounter = 0;

            Monitor.Enter(monitor);
            interceptCounter++;
            monitor.WaitSignal();
            Monitor.Exit(monitor);

            Monitor.Enter(monitor);
            interceptCounter++;
            monitor.SendSignal();
            Monitor.Exit(monitor);

            Assert.AreEqual(2, interceptCounter);
        }
    }
}
