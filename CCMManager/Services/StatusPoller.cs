using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCMManager.Services
{
    using Models;
    using System.Threading;
    using Caliburn.Micro;
    public class StatusPollerManager
    {
        private List<ActionsHomeModel> _computersToAction;
        private StatusPoller _statusPoller = new StatusPoller();

        public void Start(List<ActionsHomeModel> clients)
        {
            _computersToAction = clients;
            WorkItem.Interrupted = false;

            foreach (ActionsHomeModel client in _computersToAction)
            {
                ThreadPool.QueueUserWorkItem(_statusPoller.ThreadPoolCallback, client);
            }
        }

        public void Stop()
        {
            WorkItem.Interrupted = true;
        }
    }

    public class StatusPoller : WorkItem
    {
        public override void ThreadPoolCallback(object context)
        {
            try
            {
                ActionsHomeModel client = context as ActionsHomeModel;
                //WMIProvider oWMI = new WMIProvider(ref client);
                if (!Interrupted)
                {
                    // Do Long task here.
                    //oWMI.GetHostStatus();
                }
                else
                {
                    // Told to stop...
                }
                OnDone();
            }
            finally
            {
                WorkItemDone();
            }
        }
    }

    public abstract class WorkItem
    {
        public event EventHandler<EventArgs> Done;

        public abstract void ThreadPoolCallback(object context);

        private static bool _interrupted;
        public static bool Interrupted
        {
            get { return _interrupted; }
            set
            {
                _interrupted = value;
            }
        }
        private static long _numWorkItems = 0;
        protected long NumWorkItems
        {
            get { return _numWorkItems; }
        }
        protected WorkItem()
        {
            Interlocked.Increment(ref WorkItem._numWorkItems);
        }
        protected void WorkItemDone()
        {
            Interlocked.Decrement(ref WorkItem._numWorkItems);
        }
        protected void OnDone()
        {
            if (Done != null)
            {
                Done(this, new EventArgs());
            }
        }

    }
}
