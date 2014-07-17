using System.ServiceProcess;

namespace TxServer
{
    public partial class ServiceHarness : ServiceBase
    {
        private readonly IWindowsService _serverImpl;

        public ServiceHarness()
        {
            InitializeComponent();
        }

        public ServiceHarness(IWindowsService serverImpl) : this()
        {
            _serverImpl = serverImpl;
        }

        protected override void OnStart(string[] args)
        {
            _serverImpl.OnStart(args);
        }

        protected override void OnStop()
        {
            _serverImpl.OnStop();
        }
    }
}
