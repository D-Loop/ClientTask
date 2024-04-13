using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Threading;
using System.Runtime.CompilerServices;
using ClientTask.Helper;
using System.Threading.Tasks;
using ElMessage;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using ClientTask.Models;
using System.Windows;
using Application = ClientTask.Models.Application;

namespace ClientTask.ViewModels
{
    public class ClientViewModel: INotifyPropertyChanged
    {

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion


        public ClientViewModel()
        {
            LogMessages = new ObservableCollection<string>();
            VisibilityWindow = Visibility.Collapsed;

            CommandConnectToServer = new DelegateCommand(OnConnectToServer);
            CommandGetStatistisc = new DelegateCommand(OnGetStatistisc);
            CommandCloseProcess = new DelegateCommand(OnCloseProcess);
            CommandCloseWindow = new DelegateCommand(OnCloseWindow);
            CommandGetApplications =new DelegateCommand(OnGetApplications);
            CommandOpenWindow = new DelegateCommand(OnOpenWindow);

            ProcessDataList = new ObservableCollection<ProcessData>();

            CommandConnectToServer.Execute(null);

        }

        #region Properties

        private ElConnectionServer connectionElServer;

        private ObservableCollection<string> _logMessages { get; set; }
        public ObservableCollection<string> LogMessages
        {
            get { return _logMessages; }
            set
            {
                _logMessages = value;
                OnPropertyChanged("LogMessages");
            }
        }

        private bool _isConnected { get; set; }
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        private Visibility _visibilityWindow { get; set; }
        public Visibility VisibilityWindow
        {
            get { return _visibilityWindow; }
            set
            {
                _visibilityWindow = value;
                OnPropertyChanged("VisibilityWindow");
            }
        }
        private ProcessData _selectedProcess { get; set; }
        public ProcessData SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                OnPropertyChanged("SelectedProcess");
            }
        }

        private Application _selectedApp { get; set; }
        public Application SelectedApp
        {
            get { return _selectedApp; }
            set
            {
                _selectedApp = value;
                OnPropertyChanged("SelectedApp");
            }
        }
        private ObservableCollection<ProcessData> _processDataList { get; set; }
        public ObservableCollection<ProcessData> ProcessDataList
        {
            get { return _processDataList; }
            set
            {
                _processDataList = value;
                OnPropertyChanged("ProcessDataList");
            }
        }


        private ObservableCollection<Application> _applicationList { get; set; }
        public ObservableCollection<Application> ApplicationList
        {
            get { return _applicationList; }
            set
            {
                _applicationList = value;
                OnPropertyChanged("ApplicationList");
            }
        }

        #endregion

        #region Commands

        public ICommand CommandGetStatistisc { get; set; }
        private void OnGetStatistisc(object obj)
        {
            try
            {
                connectionElServer.SendMessage(
                    new ElMessageClient("BasicFunctions", "GetAllProcessesWithData", Response_GetStatistisc),
                    JsonConvert.SerializeObject(""));
            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }


        public ICommand CommandGetApplications{ get; set; }
        private void OnGetApplications(object obj)
        {
            try
            {
                connectionElServer.SendMessage(
                    new ElMessageClient("BasicFunctions", "GetAllApplications", Response_GetApplications),
                    JsonConvert.SerializeObject(""));
            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }

        public ICommand CommandCloseProcess{ get; set; }
        private void OnCloseProcess(object obj)
        {
            try
            {
                var id = SelectedApp is null ? SelectedProcess?.Id: SelectedApp?.Id;
                if (id == 0) return;

                connectionElServer.SendMessage(
                    new ElMessageClient("BasicFunctions", "KillSelectedProcess", Response_KillSelectedProcess),
                    JsonConvert.SerializeObject(id));
            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }

        public ICommand CommandCloseWindow { get; set; }
        private void OnCloseWindow(object obj)
        {
            try
            {
                VisibilityWindow = Visibility.Collapsed; 
            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }
        public ICommand CommandOpenWindow { get; set; }
        private void OnOpenWindow(object obj)
        {
            try
            {
                VisibilityWindow = Visibility.Visible;
            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }



        public ICommand CommandConnectToServer { get; }
        private void OnConnectToServer(object obj)
        {
            try
            {
                connectionElServer = new ElConnectionServer();
                IsConnected = connectionElServer.ConnectToServer("127.0.0.1", "61111", "LOBAS", "1111");
                LogMessages.Add($"Connect - {(IsConnected ? "Yes" : "No")} - {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}");

            }
            catch (Exception ex)
            {
                LogMessages.Add($"Error: {ex.Message}");
            }
        }
        #endregion

        #region ServerResponses

        private void Response_GetApplications(string data)
        {
            try
            {
                ProcessDataList = null;
                ApplicationList = JsonConvert.DeserializeObject<ObservableCollection<Application>>(data);
            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }

        private void Response_GetStatistisc(string data)
        {
            try
            {
                ApplicationList = null;
                ProcessDataList = JsonConvert.DeserializeObject<ObservableCollection<ProcessData>>(data);
            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }
        private void Response_KillSelectedProcess(string data)
        {
            try
            {
                var description = new { Result = false };
                var res = JsonConvert.DeserializeAnonymousType(data, description);
                
                if(res!=null && res.Result)
                    if(ApplicationList!=null)
                        CommandGetApplications.Execute(null);
                    else 
                        CommandGetStatistisc.Execute(null);
                else
                    LogMessages.Add($"Kill: No");

                VisibilityWindow=Visibility.Collapsed;

            }
            catch (Exception e)
            {
                LogMessages.Add($"Error: {e.Message}");
            }
        }


        #endregion

    }
}
