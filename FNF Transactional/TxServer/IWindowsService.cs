using System;

namespace TxServer
{
    public interface IWindowsService : IDisposable
    {
        string ServiceName { get; }
        void OnStart(string[] args);
        void OnStop();
    }
}