using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Common.Annotations;

namespace Common.Extended
{
    internal class ServiceViewModel : INotifyPropertyChanged
    {
        private readonly IWindowsService _serviceImpl;
        private bool _serviceIsRunning;
        private bool _serviceIsChanging;
        private ICommand _startCommand;
        private ICommand _stopCommand;

        public ServiceViewModel(IWindowsService serviceImpl)
        {
            _serviceImpl = serviceImpl;
            _serviceIsChanging = false;
            _serviceIsRunning = false;
        }

        public string ServiceName
        {
            get { return _serviceImpl.ServiceName; }
        }

        public bool CanStart
        {
            get { return !_serviceIsChanging && !_serviceIsRunning; }
        }

        public bool CanStop
        {
            get { return !_serviceIsChanging && _serviceIsRunning; }
        }

        public string ServiceStatus
        {
            get
            {
                if (_serviceIsRunning)
                {
                    return _serviceIsChanging ? "Stopping..." : "Running";
                }
                return _serviceIsChanging ? "Starting..." : "Stopped";
            }
        }

        public ICommand StartCommand
        {
            get { return _startCommand ?? (_startCommand = new SimpleRelayCommand(StartService, () => CanStart)); }
        }

        public ICommand StopCommand
        {
            get { return _stopCommand ?? (_stopCommand = new SimpleRelayCommand(StopService, () => CanStop)); }
        }

        public void StartService()
        {
            _serviceIsChanging = true;
            OnPropertyChanged("ServiceStatus");
            _serviceImpl.OnStart(null);
            _serviceIsChanging = false;
            _serviceIsRunning = true;
            OnPropertyChanged("ServiceStatus");
        }

        public void StopService()
        {
            _serviceIsChanging = true;
            OnPropertyChanged("ServiceStatus");
            _serviceImpl.OnStop();
            _serviceIsChanging = false;
            _serviceIsRunning = false;
            OnPropertyChanged("ServiceStatus");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
