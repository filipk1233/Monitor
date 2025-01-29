using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonitorImplementation.HoareMonitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MonitorTest
{
    [TestClass]
    public class HoareMonitorTest
    {
        [TestMethod]
        public void TestSignalCreation()
        {
            // Prepare
            var monitor = new TestMonitor();
            var signal = monitor.TestSignal;

            // Act
            // No act here, this is a signal creation test

            // Test
            Assert.IsNotNull(signal);
            Assert.IsTrue(monitor.IsConsistent);

            // Dispose
            monitor.Dispose();
        }

        [TestMethod]
        public void TestWait()
        {
            // Prepare
            var monitor = new TestMonitor();
            bool waited = false;

            Thread thread = new Thread(() =>
            {
                monitor.Enter();
                try
                {
                    monitor.TestSignal.Wait();
                    waited = true;
                }
                finally
                {
                    monitor.Exit();
                }
            });

            // Act
            thread.Start();
            Thread.Sleep(100);

            // Test
            Assert.IsFalse(waited);

            // Dispose
            monitor.Dispose();
        }

        [TestMethod]
        public void TestSignal()
        {
            // Prepare
            var monitor = new TestMonitor();
            bool waited = false;

            Thread thread = new Thread(() =>
            {
                monitor.Enter();
                try
                {
                    monitor.TestSignal.Wait();
                    waited = true;
                }
                finally
                {
                    monitor.Exit();
                }
            });

            thread.Start();
            Thread.Sleep(100); 

            // Act
            monitor.Enter();
            monitor.TestSignal.Send();
            monitor.Exit();
            thread.Join();

            // Test
            Assert.IsTrue(waited);
            Assert.IsTrue(monitor.IsConsistent);

            // Dispose
            monitor.Dispose();
        }

        // Test according to C# dispose test example
        [TestMethod]
        public void TestDispose()
        {
            // Prepare
            var monitor = new TestMonitor();

            // Act
            monitor.Dispose();

            // Test
            Assert.ThrowsException<ObjectDisposedException>(() => monitor.TestSignal.Wait());

            // Dispose
            // No dispose step here, already disposed
        }

        private class TestMonitor : HoareMonitorImplementation, IDisposable
        {
            public ISignal TestSignal 
            { 
              get;
              set; 
            }
            public bool IsConsistent = true;

            public TestMonitor()
            {
                TestSignal = CreateSignal();
            }

            public void Enter()
            {
                enterMonitorSection();
            }

            public void Exit()
            {
                exitHoareMonitorSection();
            }

            public new void Dispose()
            {
                base.Dispose();
                IsConsistent = false;
            }
        }
    }
}