using MonitorImplementation.HoareMonitor;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonitorImplementation.HoareMonitor
{
    public abstract class HoareMonitorImplementation : HoareMonitor
    {
        private Thread? currentThread;
        private Queue<Thread> monitorQueue = new();

        protected class Signal : ISignal
        {
            private Queue<Thread> signalQueue = new();

            private HoareMonitorImplementation hoareMonitorImp;

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
                        Thread signaledThread = signalQueue.Dequeue();
                        hoareMonitorImp.monitorQueue.Enqueue(signaledThread);

                        hoareMonitorImp.currentThread = signaledThread;
                        Monitor.Pulse(this);

                        Monitor.Wait(this);
                    }
                }
            }

            public void Wait()
            {
                lock (this)
                {
                    signalQueue.Enqueue(Thread.CurrentThread);
                    hoareMonitorImp.currentThread = null;
                    Monitor.Pulse(this);

                    Monitor.Wait(this);
                    hoareMonitorImp.currentThread = Thread.CurrentThread;
                }
            }

            public bool Await()
            {
                lock (this)
                {
                    if (hoareMonitorImp.monitorQueue.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
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
                        Thread waitingThread = conditionQueue.Dequeue();
                        hoareMonitorImp.monitorQueue.Enqueue(waitingThread);
                        Monitor.Pulse(this);
                    }
                }
            }

            public void Wait()
            {
                lock (this)
                {
                    conditionQueue.Enqueue(Thread.CurrentThread);

                    hoareMonitorImp.currentThread = null;
                    Monitor.Pulse(this);

                    Monitor.Wait(this);
                    hoareMonitorImp.currentThread = Thread.CurrentThread;
                }
            }
            public bool Await()
            {
                lock (this)
                {
                    if (hoareMonitorImp.monitorQueue.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        protected void AddThreadToTheQueue(Thread thread)
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
    }
}