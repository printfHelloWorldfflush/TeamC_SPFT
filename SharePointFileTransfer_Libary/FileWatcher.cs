using System;
using System.IO;
using System.Security.Permissions;
using System.Threading;

namespace SharePointFileTransfer_Library
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class FileWatcher
    {
        #region Propertys
        public static FileSystemWatcher MyFileSystemWatcher { get { return fs; } }
        private static FileSystemWatcher fs = null;
        private volatile bool isInterrupted = false;
        private static string path = "";
        private string exPath = "./FileWatcher_Exceptions.txt";
        private string nameOfFile = null;
        private string pathOfFile = null;
        private bool isChanged = false;
        private bool _publicIsChanged = false;

        public bool IsChanged
        {
            get
            {
                var v = _publicIsChanged;
                if(v) _publicIsChanged = false;
                return v;
            }
        }
        public string MyMessage { get; private set; }
        #endregion

        #region Public Methods
        public FileWatcher(string path)
        {
            fs = new FileSystemWatcher(path);
            fs.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fs.Changed += new FileSystemEventHandler(OnChanged);
            fs.EnableRaisingEvents = true;
        }

        public void run()
        {
            while (!isInterrupted)
            {
                try
                {
                    if (isChanged)
                    {
                        SharePointFileTransfer.UploadFiles(nameOfFile, pathOfFile);
                        changed(nameOfFile, pathOfFile);
                    }
                }
                catch (Exception ex)
                {
                    string oldText = "";
                    if (File.Exists(exPath))
                        oldText = File.ReadAllText(exPath);

                    string textToWrite = oldText + Environment.NewLine + "<" + DateTime.Now.ToLongTimeString() + "> " + ex.Message;
                    File.WriteAllText(exPath, textToWrite);
                }

                Thread.Sleep(100);
            }

            SharePointFileTransfer.Disconnect();
        }

        public void Interrupt()
        {
            isInterrupted = true;
        }

        public static void ChangePath(string newPath)
        {
            path = newPath;
            fs.Path = path;
        }
        #endregion

        #region Private Methods
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            isChanged = true;
            nameOfFile = e.Name;
            pathOfFile = e.FullPath;
        }

        private void changed(string nameOfFile, string pathOfFile)
        {
            MyMessage = ("File: " + nameOfFile + "; Path: " + pathOfFile + " uploaded!");
            _publicIsChanged = true;
            isChanged = false;
        }
        #endregion
    }
}
