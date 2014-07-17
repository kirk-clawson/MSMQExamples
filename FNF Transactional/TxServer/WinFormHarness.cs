using System;
using System.Windows.Forms;

namespace TxServer
{
    public partial class WinFormHarness : Form
    {
        private IWindowsService _serverImpl;
        private bool _isRunning;

        public WinFormHarness()
        {
            InitializeComponent();
        }

        public WinFormHarness(IWindowsService serverImpl) : this()
        {
            _serverImpl = serverImpl;
            btnStart.Click += BtnStartOnClick;
            btnStop.Click += BtnStopOnClick;
            lblServiceName.Text = _serverImpl.ServiceName;
        }

        private void SetFormState()
        {
            btnStart.Enabled = !_isRunning;
            btnStop.Enabled = _isRunning;
        }

        private void BtnStopOnClick(object sender, EventArgs eventArgs)
        {
            if (_isDisposed) throw new ObjectDisposedException("WinFormsHarness");
            _serverImpl.OnStop();
            _isRunning = false;
            SetFormState();
        }

        private void BtnStartOnClick(object sender, EventArgs eventArgs)
        {
            if (_isDisposed) throw new ObjectDisposedException("WinFormsHarness");
            _serverImpl.OnStart(new string[]{});
            _isRunning = true;
            SetFormState();
        }

        #region IDisposable Implementation

        private bool _isDisposed;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
                if (_serverImpl != null)
                {
                    _serverImpl.Dispose();
                    _serverImpl = null;
                }
                _isDisposed = true;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
