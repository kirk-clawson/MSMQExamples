using System.ServiceProcess;

namespace Common
{
    public partial class SmcHarness : ServiceBase
    {
        private readonly IWindowsService _serverImpl;

        public SmcHarness()
        {
            InitializeComponent();
        }

        public SmcHarness(IWindowsService serverImpl) : this()
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
