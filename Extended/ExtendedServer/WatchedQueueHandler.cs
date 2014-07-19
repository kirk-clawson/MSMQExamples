using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ExtendedServer
{
    public class WatchedQueueHandler : IWindowsService 
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public string ServiceName { get; private set; }
        public void OnStart(string[] args)
        {
            throw new NotImplementedException();
        }

        public void OnStop()
        {
            throw new NotImplementedException();
        }
    }
}
