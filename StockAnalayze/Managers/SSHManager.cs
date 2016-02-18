using StockAnalayze.Common;
using StockAnalayze.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
            int i = 0;
            StatusModel.Instance.Status = "Copying files";
            foreach (FileInfo file in files)
            {
                _sshCp.Put(file.FullName, Path.Combine(remotePath, file.Name));
                i++;
                StatusModel.Instance.setProgress(i, files.Count);
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
            runCommand($"hadoop fs -put {r}/* {h}");
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
        public void RunHadoop(string jarPath, string mainClass, string clusters)
        {
            //string i = hadoopInput.TrimEnd('/');
            //string o = hadoopOutput.TrimEnd('/');
            runCommand($"hadoop jar {jarPath} {mainClass} {clusters}");
        }

        public void CheckJobStatus()
        {
            string stdout = "";
            //string stderr = "";
            bool atleastOne = false;
            SshShell shell = new SshShell(Consts.DEFAULT_HOST, Consts.DEFAULT_USERNAME, Consts.DEFAULT_PASSWORD);
            SshExec exec = new SshExec(Consts.DEFAULT_HOST, Consts.DEFAULT_USERNAME, Consts.DEFAULT_PASSWORD);
            Thread.Sleep(1000);

            string jobId;
            int runningJobs = 0;
            while (!atleastOne || runningJobs != 0)
            {
                stdout = runStatusCommand(shell, exec, "mapred job -list");
                runningJobs = Int32.Parse(stdout.Split(null).ToList()[0]);

                if (runningJobs != 0)
                {
                    atleastOne = true;
                    jobId = stdout.Split(null).ToList().Where(line => line.Contains("job_")).First();

                    double map;
                    double reduce;
                    while (true)
                    {
                        stdout = runStatusCommand(shell, exec, $"mapred job -status {jobId}");
                        map = Double.Parse((stdout.Split('\n').Where(line =>
                            line.Contains("map()")).First().ToString()).Split(null)[2]) * 100;
                        reduce = Double.Parse((stdout.Split('\n').Where(line =>
                            line.Contains("reduce()")).First().ToString()).Split(null)[2]) * 100;

                        StatusModel.Instance.setProgress((int)((reduce + map) / 2), 100);
                        StatusModel.Instance.Status = $"Map {(int)map}% - Reduce {(int)reduce}%";
                        if (reduce == 100) break;
                        Thread.Sleep(500);
                    }
                }
            }

            StatusModel.Instance.Status = "Finished MapReduce!";
        }

        private string runStatusCommand(SshShell shell, SshExec exec, string command)
        {
            string stdout = "";
            string stderr = "";
            shell.Connect();
            shell.RedirectToConsole();
            exec.Connect();

            int ret = exec.RunCommand(command, ref stdout, ref stderr);

            exec.Close();
            shell.Close();
            return stdout;
        }

        public void runCommand(string command, string stdout = "", string stderr = "")
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
