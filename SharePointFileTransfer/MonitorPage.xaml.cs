using System;
using System.Windows;
using System.Windows.Controls;
using SharePointFileTransfer_Library;
using System.Threading;
using System.IO;
using System.Configuration;

namespace SharePointFileTransfer
{

    /// <summary>
    /// Interaction logic for MonitorPage.xaml
    /// </summary>
    public partial class MonitorPage : Page
    {
        #region Propertys
        Thread FileWatcherUpdaterThread;
        private FileWatcher myFileWatcher;
        private Thread myFileWatcherThread;
        public volatile bool threadStarted;
        public string history;
        private string logPath = "./Log.lg";
        private string threadStatePath = "./ threadState.bol";

        private class FileWatcherUpdater
        {
            FileWatcher fw;
            MonitorPage mp;

            public FileWatcherUpdater(FileWatcher fw, MonitorPage mp)
            {
                this.fw = fw;
                this.mp = mp;
            }

            public void run()
            {
                while (mp.threadStarted)
                {
                    if (!mp.IsVisible)
                        mp.threadStarted = false;

                    if (mp.myFileWatcher.IsChanged)
                    {
                        mp.updateMonitor(fw.MyMessage);
                    }

                    Thread.Sleep(100);
                }
            }
        }
        #endregion

        #region Public Methods
        public MonitorPage()
        {
            InitializeComponent();
            Init();
        }

        public void updateMonitor(string Text)
        {
            tb_Monitor.Dispatcher.Invoke(new Action<string>(updatedMonitor), new object[] { Text });
        }

        public void updatedMonitor(string Text)
        {
            string txt = (Environment.NewLine + "<" + DateTime.Now.ToLongTimeString() + "> " + Text);
            loadHistory();
            history += txt;
            tb_Monitor.Text += txt;
            File.WriteAllText(logPath, history);
        }

        #endregion

        #region Private Methods
        private void Init()
        {
            checkFiles();
            loadThreadState();
            loadHistory();
        }

        #region File related
        private void loadThreadState()
        {
            if (File.ReadAllText(threadStatePath) == "true")
            {
                threadStarted = true;
                startThreads();
            }

            updateThreadStateLabel();
        }

        private void checkFiles()
        {
            checkThreadStatePath();
            checkLogPath();
        }

        private void checkLogPath()
        {
            if (!File.Exists(logPath))
            {
                File.Create(logPath);
            }
        }

        private void checkThreadStatePath()
        {
            if(!File.Exists(threadStatePath))
            {
                File.WriteAllText(threadStatePath, "false");
            }
        }
        #endregion

        #region GUI related
        private void updateThreadStateLabel()
        {
            lb_ThreadState.Content = "Thread: " + (threadStarted ? "Running" : "Stopped");
        }

        private void loadHistory()
        {
            history = File.ReadAllText(logPath);
            tb_Monitor.Text = history;
        }

        private void bttn_StartStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loadThreadState();
                if (threadStarted)
                {
                    threadStarted = false;
                    stopThreads();
                    updatedMonitor("Thread stopped.");
                    File.WriteAllText("./threadState.bol", "false");
                }
                else
                {
                    startThreads();
                    threadStarted = true;
                    updatedMonitor("Thread started.");
                    File.WriteAllText("./threadState.bol", "true");
                }

                updateThreadStateLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void bttn_DeleteLogs_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(logPath, "");
            loadHistory();
        }
        #endregion

        #region Thread related
        private void stopThreads()
        {
            myFileWatcher.Interrupt();
        }

        private void startThreads()
        {
            SharePointFileTransfer_Library.SharePointFileTransfer.Connect();
            myFileWatcher = new FileWatcher(ConfigurationManager.AppSettings["filePath"]);
            myFileWatcherThread = new Thread(myFileWatcher.run);
            myFileWatcherThread.Start();
            startFWU();
        }

        private void startFWU()
        {
            FileWatcherUpdater fwu = new FileWatcherUpdater(myFileWatcher, this);
            FileWatcherUpdaterThread = new Thread(fwu.run);
            FileWatcherUpdaterThread.Start();
        }
#endregion

        #endregion
    }
}
