using StockAnalayze.Common;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void DeleteDirectory(string directoryPath)
        {
            runCommand($"rm -rf {directoryPath}");
        }
        public void CreateDirectory(string directoryPath)
        {
            runCommand($"mkdir {directoryPath}");
        }
        public void DeleteHadoopDirectory(string directoryPath)
        {
            runCommand($"hadoop fs -rm -r -f {directoryPath}");
        }
        public void CopyFilesToRemote(List<FileInfo> files, string remotePath)
        {
            _sshCp.Connect();

            foreach (FileInfo file in files)
            {
                _sshCp.Put(file.FullName, Path.Combine(remotePath, file.Name));
            }

            _sshCp.Close();
        }
        public void CopyFileFromRemote(string localPath, string remotePath)
        {
            _sshCp.Connect();
            _sshCp.Get(remotePath, localPath);
            _sshCp.Close();
        }
        public void CreateHadoopDirectory(string hadoopPath)
        {
            runCommand($"hadoop fs -mkdir {hadoopPath}");
        }
        public void PutHadoopFiles(string remotePath, string hadoopPath)
        {
            string r = remotePath.TrimEnd('/');
            string h = hadoopPath.TrimEnd('/');
            runCommand($"hadoop fs -put {r} {h}");
        }
        public void GetHadoopFiles(string hadoopPath, string remotePath)
        {
            string h = hadoopPath.TrimEnd('/');
            string r = remotePath.TrimEnd('/');
            runCommand($"hadoop fs -get {h} {r}");
        }
        public void CompileJava(string javaPath)
        {
            runCommand($"javac -classpath `hadoop classpath` {javaPath}*.java");
        }
        public void CreateJarRemotely(string classFilesDirectoryPath, string fullJarPath)
        {
            runCommand($"jar -cvf {fullJarPath} {classFilesDirectoryPath}*class");
        }
        public void runHadoop(string jarPath, string mainClass, string hadoopInput, string hadoopOutput)
        {
            string i = hadoopInput.TrimEnd('/');
            string o = hadoopOutput.TrimEnd('/');
            runCommand($"hadoop jar {jarPath} {mainClass} {i} {o}");
        }

        private void runCommand(string command, string stdout = "", string stderr = "")
        {
            _shell.Connect();
            _shell.RedirectToConsole();
            _exec.Connect();

            int ret = _exec.RunCommand(command, ref stdout, ref stderr);

            if (ret != 0)
            {
                System.Diagnostics.Debugger.Break();
            }

            _exec.Close();
            _shell.Close();
        }
    }
}
