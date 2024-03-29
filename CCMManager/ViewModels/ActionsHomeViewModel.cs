﻿//CCMManager
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
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using CCMManager.Framework;
using CCMManager.Models;
using CCMManager.Models.Actions;
using CCMManager.Services;

namespace CCMManager.ViewModels
{
    [Export(typeof(ActionsHomeViewModel))]
    [Export(typeof(IChildScreen<ActionsViewModel>))]
    public class ActionsHomeViewModel : Screen, IChildScreen<ActionsViewModel>
    {
        #region Internal

        private ClassroomRepository _repository;
        ActionThreadManager _actionManager = new ActionThreadManager();
        #endregion //Internal 

        #region Constructors

        [ImportingConstructor]
        public ActionsHomeViewModel(ClassroomRepository repository)
        {
            DisplayName = "Actions";
            _repository = repository;
            Task.Factory.StartNew(() => { _actionManager.ActionProcessor(); });
                        
        }

        #endregion

        #region Properties & BackingFields

        private BindableCollection<ClassroomActions> _classrooms;
        public BindableCollection<ClassroomActions> Classrooms
        {
            get { return _classrooms; }
            set
            {
                _classrooms = value;
                NotifyOfPropertyChange(() => Classrooms);
            }
        }

        public BindableCollection<ActionsHomeModel> Computers
        {
            get
            {
                if (SelectedClassroom != null)
                    return SelectedClassroom.Computers;
                else
                    return null;
            }
        }       

        private ClassroomActions _selectedClassroom;
        public ClassroomActions SelectedClassroom
        {
            get { return _selectedClassroom; }
            set
            {
                _selectedClassroom = value;
                //SetComputerActions();
                NotifyOfPropertyChange(() => SelectedClassroom);
                NotifyOfPropertyChange(() => Computers);
            }
        }

        private ActionsHomeModel _selectedComputer;
        public ActionsHomeModel SelectedComputer
        {
            get { return _selectedComputer; }
            set
            {
                _selectedComputer = value;
                NotifyOfPropertyChange(() => SelectedComputer);
            }
        }

        private bool _togStatusPoller;
        public bool togStatusPoller
        {
            get { return _togStatusPoller; }
            set
            {
                _togStatusPoller = value;
                if (togStatusPoller)
                {
                    StartStatusPoller();
                }
                else
                {
                    StopStatusPoller();
                }
                NotifyOfPropertyChange(() => togStatusPoller);
            }
        }

        public string ScreenId
        {
            get { return GetType().Name; }
        }

        public int? Order
        {
            get { return 0; }
        }

        #endregion //Properties and BackingFields

        #region Methods

        public void EditClassrooms(object o)
        {
            //Load the EditorViewModel instead of this one...
            var myDirectParent = Parent as ActionsViewModel;
            var realParent = myDirectParent.Parent as ShellViewModel;

            realParent.ActivateItem(realParent.Items[1]);
            
        }

        public void ViewComputer(object sender)
        {
            Console.WriteLine("Hit");
            var item = sender as ActionsHomeViewModel;
            Console.WriteLine(item.SelectedComputer.Name);

            var parent = Parent as ActionsViewModel;
            ComputerDetailsViewModel newItem = new ComputerDetailsViewModel() { DisplayName = item.SelectedComputer.Name };
            parent.Items.Add(newItem);
            parent.ActivateItem(newItem);
        }

        private void RefreshClassrooms()
        {
            _repository = new ClassroomRepository();
            var rClassrooms = new BindableCollection<IClassroom>(_repository.GetClassrooms());
            this.Classrooms = new BindableCollection<ClassroomActions>();
            foreach (IClassroom c in rClassrooms)
            {
                this.Classrooms.Add(new ClassroomActions(c.Name, c.Computers));
            }
            //this.Classrooms = new BindableCollection<IClassroom>(_repository.GetClassrooms());

            if (Classrooms.Count > 0)
            {
                if (SelectedClassroom == null)
                    SelectedClassroom = Classrooms[0];
            }
                

            NotifyOfPropertyChange(() => Classrooms);
        }

        protected override void OnActivate()
        {
            RefreshClassrooms();
            base.OnActivate();
        }

        private void StartStatusPoller()
        {
            //foreach (ActionsHomeModel c in SelectedClassroom.Computers)
            //{
                //_statusPoller.Start(SelectedClassroom.Computers.ToList<ActionsHomeModel>());
            //}
            //List<Task> tasks = new List<Task>();
            //Dictionary<ComputerStates, ImageSource> StatusPics = new Dictionary<ComputerStates, ImageSource>();
            //var imgOnline = new BitmapImage(new Uri("pack://application:,,,/Images/network-idle.png"));
            //var imgOffline = new BitmapImage(new Uri("pack://application:,,,/Images/network-wired-3.png"));
            //var imgLoggedOn = new BitmapImage(new Uri("pack://application:,,,/Images/identity.png"));
            //var imgError = new BitmapImage(new Uri("pack://application:,,,/Images/network-error.png"));
            //var imgAccessError = new BitmapImage(new Uri("pack://application:,,,/Images/object-locked.png"));
            //var imgUnknown = new BitmapImage(new Uri("pack://application:,,,/Images/system-help-3.png"));
            //StatusPics.Add(ComputerStates.Online, imgOnline as ImageSource);
            //StatusPics.Add(ComputerStates.LoggedOn, imgLoggedOn as ImageSource);
            //StatusPics.Add(ComputerStates.Offline, imgOffline as ImageSource);
            //StatusPics.Add(ComputerStates.DNSError, imgError as ImageSource);
            //StatusPics.Add(ComputerStates.Broken, imgError as ImageSource);
            //StatusPics.Add(ComputerStates.AccessDenied, imgAccessError as ImageSource);
            //StatusPics.Add(ComputerStates.Unknown, imgUnknown as ImageSource);

            foreach (ActionsHomeModel c in SelectedClassroom.Computers)
            {
                //StatusAction sa = new StatusAction(StatusPics);
                StatusAction sa = new StatusAction();
                ActionsHomeModel pc = c;
                var t = Task.Factory.StartNew(() =>
                    {
                        sa.Execute(pc);
                    });
                //tasks.Add(t);
            }
        }

        private void StopStatusPoller()
        {
            //_statusPoller.Stop();
        }

        #endregion //Methods

        #region Public Menu Actions

        public void PolicyDownloadClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                PolicyDownloadAction action = new PolicyDownloadAction(pics);
                AddPolicyDownloadAction(pc, action);
            }
        }
        public void PolicyDownloadSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            PolicyDownloadAction action = new PolicyDownloadAction(pics);
            if (SelectedComputer != null)
            {
                AddPolicyDownloadAction(SelectedComputer, action);
            }
        }
        public void PolicyApplyClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                PolicyApplyAction action = new PolicyApplyAction(pics);
                AddPolicyApplyAction(pc, action);
            }
        }
        public void PolicyApplySelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            PolicyApplyAction action = new PolicyApplyAction(pics);
            if (SelectedComputer != null)
            {
                AddPolicyApplyAction(SelectedComputer, action);
            }
        }
        public void PolicyHardResetClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                PolicyHardResetAction action = new PolicyHardResetAction(pics);
                AddPolicyHardResetAction(pc, action);
            }
        }
        public void PolicyHardResetSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            PolicyHardResetAction action = new PolicyHardResetAction(pics);
            if (SelectedComputer != null)
            {
                AddPolicyHardResetAction(SelectedComputer, action);
            }
        }
        public void PolicyResetClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);

            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                PolicyResetAction action = new PolicyResetAction(pics);
                AddPolicyResetAction(pc, action);
            }
        }
        public void PolicyResetSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            PolicyResetAction action = new PolicyResetAction(pics);
            if (SelectedComputer != null)
            {
                AddPolicyResetAction(SelectedComputer, action);
            }
        }
        public void DCMScanClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                DCMScanAction action = new DCMScanAction(pics);
                AddDCMScanAction(pc, action);
            }
        }
        public void DCMScanSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            DCMScanAction action = new DCMScanAction(pics);
            if (SelectedComputer != null)
            {
                AddDCMScanAction(SelectedComputer, action);
            }
        }
        public void DDRScanClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);

            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                DataDiscoveryAction action = new DataDiscoveryAction(pics);
                AddDDRAction(pc, action);
            }
        }
        public void DDRScanSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            DataDiscoveryAction action = new DataDiscoveryAction(pics);
            if (SelectedComputer != null)
            {
                AddDDRAction(SelectedComputer, action);
            }
        }
        public void RebootClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                RebootAction action = new RebootAction(pics);
                ActionsHomeModel i = pc;
                AddRebootAction(i, action);
            }
        }
        public void RebootSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            if (SelectedComputer != null)
            {
                RebootAction action = new RebootAction(pics);
                AddRebootAction(SelectedComputer, action);
            }
        }
        public void LogoffClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                LogoffAction logoff = new LogoffAction(pics);
                AddLogoffAction(pc, logoff);
            }
        }
        public void LogoffSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            LogoffAction action = new LogoffAction(pics);
            if (SelectedComputer != null)
            {
                AddLogoffAction(SelectedComputer, action);
            }
        }
        public void LockClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                LockAction locka = new LockAction(pics);
                AddLockAction(pc, locka);
            }
        }
        public void LockSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            LockAction action = new LockAction(pics);
            if (SelectedComputer != null)
            {
                AddLockAction(SelectedComputer, action);
            }
        }
        public void GPUpdateClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                GPUpdateAction gpu = new GPUpdateAction(pics);
                AddGPUpdateAction(pc, gpu);
            }
        }
        public void GPUpdateSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/media-playlist-shuffle-3.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            GPUpdateAction action = new GPUpdateAction(pics);
            if (SelectedComputer != null)
            {
                AddGPUpdateAction(SelectedComputer, action);
            }
            
        }
        public void WakeClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/quickopen.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                WakeOnLanAction wol = new WakeOnLanAction(pics);
                AddWOLAction(pc, wol);
            }
        }
        public void WakeSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/quickopen.png"));
            pics.Add(RemoteActionState.NotStarted, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgDefault as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Pending, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgDefault as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgDefault as ImageSource);
            WakeOnLanAction wol = new WakeOnLanAction(pics);
            if (SelectedComputer != null)
            {
                AddWOLAction(SelectedComputer, wol);
                //SelectedComputer.Actions.Add(sa);
                //_actionManager.AddAction(SelectedComputer);
            }
        }
        public void ShutdownClassroom(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/system-shutdown-3.png"));
            var imgError = new BitmapImage(new Uri("pack://application:,,,/Images/system-shutdown-3-error.png"));
            var imgInProgress = new BitmapImage(new Uri("pack://application:,,,/Images/system-shutdown-3-inprogress.png"));
            pics.Add(RemoteActionState.NotStarted, imgInProgress as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgInProgress as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgInProgress as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgError as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgInProgress as ImageSource);
            
            foreach (ActionsHomeModel pc in SelectedClassroom.Computers)
            {
                ShutdownAction sa = new ShutdownAction(pics);
                AddShutdownAction(pc, sa);
            }
            //_actionManager.AddAction(SelectedClassroom.Computers.ToList<ActionsHomeModel>());
        }
        public void ShutdownSelected(object sender)
        {
            Dictionary<RemoteActionState, ImageSource> pics = new Dictionary<RemoteActionState, ImageSource>();
            var imgDefault = new BitmapImage(new Uri("pack://application:,,,/Images/system-shutdown-3.png"));
            var imgError = new BitmapImage(new Uri("pack://application:,,,/Images/system-shutdown-3-error.png"));
            var imgInProgress = new BitmapImage(new Uri("pack://application:,,,/Images/system-shutdown-3-inprogress.png"));
            pics.Add(RemoteActionState.NotStarted, imgInProgress as ImageSource);
            pics.Add(RemoteActionState.InProgress, imgInProgress as ImageSource);
            pics.Add(RemoteActionState.ReRun, imgInProgress as ImageSource);
            pics.Add(RemoteActionState.ReRunning, imgInProgress as ImageSource);
            pics.Add(RemoteActionState.Completed, imgDefault as ImageSource);
            pics.Add(RemoteActionState.Error, imgError as ImageSource);

            ShutdownAction sa = new ShutdownAction(pics);
            if (SelectedComputer != null)
            {
                AddShutdownAction(SelectedComputer, sa);
                //SelectedComputer.Actions.Add(sa);
                //_actionManager.AddAction(SelectedComputer);
            }
        }

        #endregion //Public Menu Actions

        #region Private Menu Actions

        private void AddPolicyDownloadAction(ActionsHomeModel pc, PolicyDownloadAction polDown)
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(PolicyDownloadAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(polDown);
                _actionManager.AddAction(pc);
            }
        }
        private void AddPolicyApplyAction(ActionsHomeModel pc, PolicyApplyAction polApply)
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(PolicyApplyAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(polApply);
                _actionManager.AddAction(pc);
            }
        }
        private void AddPolicyResetAction(ActionsHomeModel pc, PolicyResetAction polReset)
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(PolicyResetAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(polReset);
                _actionManager.AddAction(pc);
            }
        }
        private void AddPolicyHardResetAction(ActionsHomeModel pc, PolicyHardResetAction polReset)
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(PolicyHardResetAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(polReset);
                _actionManager.AddAction(pc);
            }
        }
        private void AddDCMScanAction(ActionsHomeModel pc, DCMScanAction dcmScan)
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(DCMScanAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(dcmScan);
                _actionManager.AddAction(pc);
            }
        }
        private void AddRebootAction(ActionsHomeModel pc, RebootAction reboot) 
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(RebootAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(reboot);
                _actionManager.AddAction(pc);
            }
        }
        private void AddLogoffAction(ActionsHomeModel pc, LogoffAction logoff) 
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(LogoffAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(logoff);
                _actionManager.AddAction(pc);
            }
        }
        private void AddLockAction(ActionsHomeModel pc, LockAction locka) 
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(LockAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(locka);
                _actionManager.AddAction(pc);
            }
        }
        private void AddGPUpdateAction(ActionsHomeModel pc, GPUpdateAction gpu) 
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(GPUpdateAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(gpu);
                _actionManager.AddAction(pc);
            }
        }
        private void AddDDRAction(ActionsHomeModel pc, DataDiscoveryAction ddr)
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(DataDiscoveryAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(ddr);
                _actionManager.AddAction(pc);
            }
        }
        
        private void AddWOLAction(ActionsHomeModel pc, WakeOnLanAction wol)
        {
            bool pcHasAction = false;
            Console.WriteLine("Adding WoL for {0}", pc.Name);
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(WakeOnLanAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        //_actionManager.Pause();
                        _actionManager.AddAction(pc);
                        //_actionManager.Resume();
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(wol);
                //_actionManager.Pause();
                _actionManager.AddAction(pc);
                //_actionManager.Resume();
            }
        }
        private void AddShutdownAction(ActionsHomeModel pc, ShutdownAction sa)
        {
            bool pcHasAction = false;
            foreach (RemoteAction ra in pc.Actions)
            {
                if (ra.GetType() == typeof(ShutdownAction))
                {
                    //Already contains this one.
                    pcHasAction = true;
                    //re-run it if completed/error
                    if (ra.State == RemoteActionState.Completed || ra.State == RemoteActionState.Error)
                    {
                        ra.State = RemoteActionState.ReRun;
                        _actionManager.AddAction(pc);
                    }
                }
            }

            if (!pcHasAction)
            {
                pc.Actions.Add(sa);
                _actionManager.AddAction(pc);
            }
        }

        #endregion //Private Menu Actions
    }
}
