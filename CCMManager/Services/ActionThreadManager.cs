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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CCMManager.Models;

namespace CCMManager.Services
{
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
                        if (ra.State == RemoteActionState.NotStarted || ra.State == RemoteActionState.Pending)
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
