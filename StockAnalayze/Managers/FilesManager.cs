using StockAnalayze.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalayze.Managers
{
    class FilesManager
    {
        private readonly SSHManager _sshManager;

        public FilesManager()
        {
            _sshManager = new SSHManager(Consts.DEFAULT_HOST, Consts.DEFAULT_USERNAME, Consts.DEFAULT_PASSWORD);
        }
    }
}
