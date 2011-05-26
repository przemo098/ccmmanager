//CCMManager
//Copyright (c) 2011 by David Kamphuis
//
//   This file is part of CCMManager.
//
//    CCMManager is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Foobar is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using CCMManager.Models;

namespace CCMManager.Services
{
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
