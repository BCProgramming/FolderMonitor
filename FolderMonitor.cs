using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FolderMonitor
{
    public class FolderMonitor : IDisposable
    {
        public class FileChangeEventArgs : EventArgs
        {
            public enum FileChangeType
            {
                Created,Deleted,Renamed
            }
            private FileSystemEventArgs fse;
            private FileChangeType _ChangeType;
            private String _FilePath;
            public FileChangeType ChangeType { get { return _ChangeType; } }
            public String FilePath { get { return _FilePath; } set { _FilePath = value; } }
            public FileSystemEventArgs SourceArgs { get { return fse; } }
            public FileChangeEventArgs(String pFile,FileSystemEventArgs pfse,FileChangeType pChangeType)
            {
                fse = pfse;
                _FilePath = pFile;
                _ChangeType = pChangeType;
            }
            public FileChangeEventArgs(String pFile,RenamedEventArgs prea)
            {
                _FilePath = pFile;
                _ChangeType = FileChangeType.Renamed;
                fse = prea;
            }
        }
        private bool _IsStarted = false;
        private String _FolderPath = null;
        private String _Filter = null;
        private bool _SubDirectories = false;
        public bool Started { get { return _IsStarted; } }
        public String FolderPath { get { return _FolderPath; } }

        public String Filter { get { return _Filter; } }
        public bool Subdirectories { get {  return _SubDirectories;} set {_SubDirectories = value;} }

        public event EventHandler<RenamedEventArgs> Renamed;
        public event EventHandler<FileSystemEventArgs> Created;
        public event EventHandler<FileSystemEventArgs> Deleted;
        public event EventHandler<FileChangeEventArgs> FileEvent;
        FileSystemWatcher fsw = null;
        public void RaiseFileEvent(FileChangeEventArgs fse)
        {
            var fe = FileEvent;
            if (fe != null) fe(this, fse);


        }
        public void RaiseRenamed(RenamedEventArgs e)
        {
            var copied = Renamed;
            if (copied != null) copied(this, e);
            RaiseFileEvent(new FileChangeEventArgs(e.FullPath,e));
        }
        public void RaiseCreated(FileSystemEventArgs e)
        {
            var copied = Created;
            if (copied != null) copied(this, e);
            RaiseFileEvent(new FileChangeEventArgs(e.FullPath,e,FileChangeEventArgs.FileChangeType.Created));
        }
        public void RaiseDeleted(FileSystemEventArgs e)
        {
            var copied = Deleted;
            if (copied != null) copied(this, e);
            RaiseFileEvent(new FileChangeEventArgs(e.FullPath,e,FileChangeEventArgs.FileChangeType.Deleted));
        }
        public FolderMonitor(String pPath,String pFilter)
        {
            _FolderPath = pPath;
            _Filter = pFilter;
        }
        public FolderMonitor(String pPath):this(pPath,"*.*")
        {

        }
        private bool _IsDisposed= false;
        public void Dispose()
        {
            if (_IsDisposed) return;
            Stop();
            _IsDisposed = true;
        }
        ~FolderMonitor()
        {
            Dispose();
        }
        public void Start()
        {
            if(_IsStarted)
            {
                fsw.Dispose();
                fsw = null;
            }
            fsw = new FileSystemWatcher(_FolderPath, _Filter);
            fsw.Created += fsw_Created;
            fsw.Deleted += fsw_Deleted;
            fsw.Renamed += fsw_Renamed;
            _IsStarted = true;
            fsw.EnableRaisingEvents = true;
            fsw.IncludeSubdirectories = _SubDirectories;
            
        }
        public void Stop()
        {
            fsw.Created -= fsw_Created;
            fsw.Deleted -= fsw_Deleted;
            fsw.Renamed -= fsw_Renamed;
            fsw.Dispose();
            _IsStarted = false;
            fsw = null;
        }

        void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            RaiseRenamed(e);
        }

        void fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            RaiseDeleted(e);
        }

        void fsw_Created(object sender, FileSystemEventArgs e)
        {
            RaiseCreated(e);
        }
    }

    public class MonitorManager:IDisposable
    {
        private MonitorConfiguration mc;
        private Dictionary<MonitorConfigurationItem, FolderMonitor> MonitorMapping = new Dictionary<MonitorConfigurationItem, FolderMonitor>();
        public event EventHandler<FolderMonitor.FileChangeEventArgs> FileChange;
        private void FireFileChange(Object sender,FolderMonitor.FileChangeEventArgs e)
        {
            var copied = FileChange;
            if (copied != null) copied(sender, e);
        }
        public MonitorManager(MonitorConfiguration mc)
        {
            foreach(var iterate in mc.GetItems())
            {
                FolderMonitor fm = new FolderMonitor(iterate.MonitorPath, iterate.Filter);
                fm.FileEvent += fm_FileEvent;
                fm.Start();
            }
        }

        void fm_FileEvent(object sender, FolderMonitor.FileChangeEventArgs e)
        {
            FireFileChange(sender,e);
        }

        ~MonitorManager()
        {
            Dispose();
        }
        bool _Disposed = false;
        public void Dispose()
        {
            if (_Disposed) return;
            _Disposed = true;
            foreach(FolderMonitor fm in MonitorMapping.Values)
            {
                fm.Dispose();
            }
            MonitorMapping = null;
        }
    }
}
