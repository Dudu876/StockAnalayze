using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh;

namespace StockAnalayze.Managers
{
    class SSHManager
    {
        private readonly SshShell _shell = null;
        private SshExec _exec = null;
        private SshTransferProtocolBase _sshCp = null;

        public SSHManager(string host, string username, string password)
        {
            _shell = new SshShell(host, username, password);
            _exec = new SshExec(host, username, password);
            _sshCp = new Scp(host, username, password);
        }
    }
}
