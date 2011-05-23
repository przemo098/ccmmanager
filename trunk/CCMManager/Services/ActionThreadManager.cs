using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace CCMManager.Services
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Micro;

    public class ActionThreadManager
    {
        private List<ActionsHomeModel> _currentActionsList;
        private List<ActionsHomeModel> _processActionsList;
        private bool _interrupt = false;
        private int _threadwaittimeout = 5000;

        public ActionThreadManager()
        {
            this._currentActionsList = new List<ActionsHomeModel>();
            this._processActionsList = new List<ActionsHomeModel>();
        }

        public void AddAction(ActionsHomeModel singlePC)
        {
            if (!this._currentActionsList.Contains(singlePC))
            {
                lock (this._currentActionsList)
                {
                    this._currentActionsList.Add(singlePC);
                }
            }
        }

        public void AddAction(List<ActionsHomeModel> manyPCs)
        {
            foreach (ActionsHomeModel pc in manyPCs)
            {
                this.AddAction(pc);
            }
        }

        private void RemoveAction(ActionsHomeModel singlePC)
        {
            lock (this._currentActionsList)
            {
                this._currentActionsList.Remove(singlePC);
            }
        }

        public void ActionProcessor()
        {
            while (!_interrupt)
            {
                foreach (var c in _currentActionsList)
                {
                    ActionsHomeModel pc = c;
                    foreach (var r in pc.Actions)
                    {
                        RemoteAction  ra = r;
                        if (ra.State == RemoteActionState.NotStarted)
                        {
                            //start new
                            ra.State = RemoteActionState.InProgress;
                            Task.Factory.StartNew(() =>
                                {
                                    ra.Execute(pc);
                                });
                            
                        }
                        else if (ra.State == RemoteActionState.ReRun || ra.State == RemoteActionState.Error)
                        {
                            //re-run
                            ra.State = RemoteActionState.ReRunning;
                            Task.Factory.StartNew(() =>
                                {
                                    ra.Execute(pc);
                                });
                        }
                        else
                        {
                        }
                    }
                }
                CleanUpActionList();
                Thread.Sleep(_threadwaittimeout);
                lock (_currentActionsList)
                {
                    _processActionsList = new List<ActionsHomeModel>(_currentActionsList);
                }
            }
        }

        public void CleanUpActionList()
        {
            lock (_currentActionsList)
            {
                List<ActionsHomeModel> newList = new List<ActionsHomeModel>(_currentActionsList);
                foreach (ActionsHomeModel pc in newList)
                {
                    bool allDone = true;
                    foreach (RemoteAction a in pc.Actions)
                    {
                        if (a.State != RemoteActionState.Completed)
                        {
                            allDone = false;
                        }
                    }

                    if (allDone)
                    {
                        _currentActionsList.Remove(pc);
                    }
                }
            }
        }

    }
}
