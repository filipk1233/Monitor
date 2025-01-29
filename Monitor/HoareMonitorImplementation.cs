using MonitorImplementation.HoareMonitor;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonitorImplementation.HoareMonitor
{
    public abstract class HoareMonitorImplementation : HoareMonitor, IDisposable
    {
        protected bool isEntered = false;
        private Queue<Thread> monitorQueue = new();
        private bool disposedValue;

        protected void enterMonitorSection()
        {
            Monitor.Enter(this);
            isEntered = true;
        }

        protected void exitHoareMonitorSection()
        {
            if (isEntered)
            {
                isEntered = false;
                Monitor.Exit(this);
                Monitor.Pulse(this);
            }
        }

        public class Signal : ISignal, IDisposable
        {
            private bool _disposed = false;
            private Queue<Thread> signalQueue = new();

            private HoareMonitorImplementation hoareMonitorImp;

            private AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            public Signal(HoareMonitorImplementation monitor)
            {
                hoareMonitorImp = monitor;
            }

            public void Send()
            {
                lock (this)
                {
                    if (signalQueue.Count > 0)
                    {
                        hoareMonitorImp.addToQueue(signalQueue.Dequeue());
                        autoResetEvent.Set();
                    }
                }
            }

            public void Wait()
            {
                lock (this)
                {
                    signalQueue.Enqueue(Thread.CurrentThread);
                    hoareMonitorImp.enterMonitorSection();
                    autoResetEvent.WaitOne();
                    hoareMonitorImp.exitHoareMonitorSection();
                }
            }

            public bool Await()
            {
                lock (this)
                {
                    if (signalQueue.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    autoResetEvent.Dispose();
                    signalQueue.Clear();
                }

                _disposed = true;
            }
        }

        protected class Condition : ICondition
        {
            private HoareMonitorImplementation hoareMonitorImp;

            private Queue<Thread> conditionQueue = new();

            public Condition(HoareMonitorImplementation monitor)
            {
                hoareMonitorImp = monitor;
            }

            public void Send()
            {
                lock (this)
                {
                    if (conditionQueue.Count > 0)
                    {

                    }
                }
            }

            public void Wait()
            {
                lock (this)
                {

                }
            }
            public bool Await()
            {
                lock (this)
                {
                    if (conditionQueue.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        protected void addToQueue(Thread thread)
        {
            monitorQueue.Enqueue(thread);
        }

        protected override ISignal CreateSignal()
        {
            return new Signal(this);
        }

        protected override ICondition GetCondition()
        {
            return new Condition(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    monitorQueue.Clear();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}