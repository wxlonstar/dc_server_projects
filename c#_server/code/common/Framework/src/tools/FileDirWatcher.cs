using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;

namespace dc
{
    /// <summary>
    /// 文件系统监视
    /// @author hannibal
    /// @time 2016-9-9
    /// </summary>
    public sealed class FileDirWatcher
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
    
        #region 定义委托
        /// <summary>
        /// 内容改变
        /// </summary>
        public delegate void OnFileChanged(string file_path);
        /// <summary>
        /// 创建
        /// </summary>
        public delegate void OnFileCreated(string file_path);
        /// <summary>
        /// 删除
        /// </summary>
        public delegate void OnFileDeleted(string file_path);
        /// <summary>
        /// 改名
        /// </summary>
        public delegate void OnFileRenamed(string old_path, string new_path);
        #endregion

        #region 定义事件
        public event OnFileChanged OnChangedEvt;
        public event OnFileCreated OnCreatedEvt;
        public event OnFileDeleted OnDeletedEvt;
        public event OnFileRenamed OnRenamedEvt;
        #endregion

        /// <summary>
        /// 开启监视
        /// </summary>
        /// <param name="path">监视目录</param>
        /// <param name="ext">扩展名</param>
        /// <param name="include_child">是否监视子目录</param>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public void Start(string path, string ext, bool include_child)
        {
            Log.Info("开始监控文件目录:" + path);
            watcher.Path = path;
            watcher.IncludeSubdirectories = include_child;
            /* 设置为监视 LastWrite 和 LastAccess 时间方面的更改，以及目录中文本文件的创建、删除或重命名。 */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // 只监控.txt文件
            watcher.Filter = ext;

            // 添加事件处理器。
            watcher.Changed += new FileSystemEventHandler(OnChangedFile);
            watcher.Created += new FileSystemEventHandler(OnCreatedFile);
            watcher.Deleted += new FileSystemEventHandler(OnDeletedFile);
            watcher.Renamed += new RenamedEventHandler(OnRenamedFile);

            try
            {
                watcher.EnableRaisingEvents = true;
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
        }
        public void Stop()
        {
            if(watcher != null)
            {
                Log.Info("停止监控文件目录:" + watcher.Path);
                try
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    watcher = null;
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
        }
        private void OnChangedFile(object source, FileSystemEventArgs e)
        {
            if (OnChangedEvt != null)
                OnChangedEvt(e.FullPath);
        }
        private void OnCreatedFile(object source, FileSystemEventArgs e)
        {
            if (OnCreatedEvt != null)
                OnCreatedEvt(e.FullPath);
        }
        private void OnDeletedFile(object source, FileSystemEventArgs e)
        {
            if (OnDeletedEvt != null)
                OnDeletedEvt(e.FullPath);
        }
        private void OnRenamedFile(object source, RenamedEventArgs e)
        {
            if (OnRenamedEvt != null)
                OnRenamedEvt(e.OldFullPath, e.FullPath);
        }
    }
}
