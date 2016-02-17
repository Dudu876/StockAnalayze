using StockAnalayze.Common;
using StockAnalayze.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static void PrepareLocal()
        {
            StatusModel.Instance.Status = "Prepearing the local Directorys";
            new DirectoryInfo(Consts.STOCKS_DATA_PATH_BASE).Empty();
            new DirectoryInfo(Consts.STOCKS_PROCESSED_PATH_BASE).Empty();
            new DirectoryInfo(Consts.OUTPUT_PATH_BASE).Empty();
        }

        public void PrepareRemote()
        {
            StatusModel.Instance.Status = "Preparing remote files";
            _sshManager.DeleteDirectory(Consts.REMOTE_INPUT_PATH);
            _sshManager.DeleteDirectory(Consts.REMOTE_JAVA_PATH);
            _sshManager.DeleteDirectory(Consts.REMOTE_OUTPUT_PATH);

            _sshManager.DeleteHadoopDirectory(Consts.HADOOP_PATH_BASE);

            _sshManager.CreateDirectory(Consts.REMOTE_PATH_BASE);
            _sshManager.CreateDirectory(Consts.REMOTE_INPUT_PATH);
            _sshManager.CreateDirectory(Consts.REMOTE_JAVA_PATH);

            _sshManager.CreateHadoopDirectory(Consts.HADOOP_PATH_BASE);
            //_sshManager.CreateHadoopDirectory(Consts.HADOOP_INPUT_PATH);

            _sshManager.CopyFilesToRemote(new DirectoryInfo(Consts.STOCKS_PROCESSED_PATH_BASE).GetFiles().ToList(),
                                            Consts.REMOTE_INPUT_PATH);
            _sshManager.CopyFilesToRemote(new DirectoryInfo(Consts.LOCAL_JAVA_FILES_DIR_PATH).GetFiles().ToList(),
                                            Consts.REMOTE_JAVA_PATH);

            StatusModel.Instance.Status = "Upload files to hadoop";
            _sshManager.PutHadoopFiles(Consts.REMOTE_INPUT_PATH, Consts.HADOOP_INPUT_PATH);

            StatusModel.Instance.Status = "Compiling";
            _sshManager.CompileJava(Consts.REMOTE_JAVA_PATH);
            _sshManager.CreateJarRemotely(Consts.REMOTE_JAVA_PATH, Consts.REMOTE_JAR_PATH);
        }

        public async void RunHadoop()
        {
            StatusModel.Instance.Status = "Run hadoop";
            _sshManager.RunHadoop(Consts.REMOTE_JAR_PATH, 
                                  Consts.MAIN_CLASS_HADOOP, 
                                  Consts.HADOOP_INPUT_PATH, 
                                  Consts.HADOOP_OUTPUT_PATH);
        }

        public void RetriveOutput()
        {
            StatusModel.Instance.Status = "Retrive output files";
            _sshManager.GetHadoopFiles(Consts.HADOOP_OUTPUT_PATH, Consts.REMOTE_OUTPUT_PATH);
            _sshManager.CopyFileFromRemote(Consts.LOCAL_OUTPUT_FILENAME, Consts.HADOOP_OUTPUT_FILENAME);
        }

        public async void TestRun()
        {
            StatusModel.Instance.Status = "Dudu";
            StatusModel.Instance.setProgress(50, 100);
            _sshManager.DeleteDirectory(Consts.REMOTE_INPUT_PATH);
            _sshManager.DeleteDirectory(Consts.REMOTE_JAVA_PATH);
            _sshManager.DeleteDirectory(Consts.REMOTE_OUTPUT_PATH);

            _sshManager.DeleteHadoopDirectory(Consts.HADOOP_PATH_BASE);

            _sshManager.CreateDirectory(Consts.REMOTE_PATH_BASE);
            _sshManager.CreateDirectory(Consts.REMOTE_INPUT_PATH);
            _sshManager.CreateDirectory(Consts.REMOTE_JAVA_PATH);

            _sshManager.CreateHadoopDirectory(Consts.HADOOP_PATH_BASE);
            //_sshManager.CreateHadoopDirectory(Consts.HADOOP_INPUT_BASE);

            _sshManager.CopyFilesToRemote(new DirectoryInfo($@"..\..\TestInput").GetFiles().ToList(),
                                            Consts.REMOTE_INPUT_PATH);
            _sshManager.CopyFilesToRemote(new DirectoryInfo(Consts.LOCAL_JAVA_FILES_DIR_PATH).GetFiles().ToList(),
                                            Consts.REMOTE_JAVA_PATH);

            _sshManager.PutHadoopFiles(Consts.REMOTE_INPUT_PATH, Consts.HADOOP_INPUT_PATH);

            _sshManager.CompileJava(Consts.REMOTE_JAVA_PATH);
            _sshManager.CreateJarRemotely(Consts.REMOTE_JAVA_PATH, Consts.REMOTE_JAR_PATH);

            Task t = Task.Factory.StartNew(() => RunHadoop());
            HadoopJobStatus();
            await t;

            RetriveOutput();

            //return true;
            //Task<bool> t = start();
            //return await Task.Run(() => start());
        }

        public void HadoopJobStatus()
        {
            StatusModel.Instance.Status = "Checking the hadoop status";
            
            _sshManager.CheckJobStatus();
        }

        public async Task<bool> Start()
        {
            PrepareRemote();
            Task t = Task.Factory.StartNew(() => RunHadoop());
            HadoopJobStatus();
            await t;
            RetriveOutput();
            return true;
        }
    }
}
