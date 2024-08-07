using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OGService
{
    public class ProcessAutoUpdate : IProcessService
    {
        private Timer TimerCheckUpdate;
        private string FileDownloadPath;
        private string FileZipExtractorPath;
        public string ProcessName => this.GetType().Name;
        public void ProcessInit()
        {
            ProcessHelper.InitFunction(() => InitAutoUpdate(), nameof(InitAutoUpdate));
        }
        #region InitAutoUpdate
        private void InitAutoUpdate()
        {
            try
            {
                var pc = VersionHelper.GetCurrentComputerInfo();

                LogHelper.LogInfo("InitAutoUpdate");
                this.TimerCheckUpdate = new Timer();
                this.TimerCheckUpdate.Interval = 3000;
                this.TimerCheckUpdate.Elapsed += TimerCheckUpdateProcess;
                this.TimerCheckUpdate.Start();
                
            }
            catch (Exception ex)
            {
                LogHelper.LogInfo($"InitAutoUpdate Exception:{ex.Message} {ex.StackTrace}");
            }
            
        }
        private void TimerCheckUpdateProcess(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.TimerCheckUpdate.Stop();
                var resultCheckUpdate = CheckUpdate();
                if (resultCheckUpdate.IsUpdateAvailable)
                {
                    StartUpdate(resultCheckUpdate);
                }
                else
                {
                    this.TimerCheckUpdate.Interval = 60000;
                    this.TimerCheckUpdate.Start();
                }
            }
            catch(Exception ex)
            {
                LogHelper.LogInfo($"TimerCheckUpdateProcess Exception:{ex.Message} {ex.StackTrace}");
                this.TimerCheckUpdate.Interval = 60000;
                this.TimerCheckUpdate.Start();
            }

        }
        private UpdateInfoEventArgs CheckUpdate()
        {
            UpdateInfoEventArgs args = new UpdateInfoEventArgs();
            try
            {
                LogHelper.LogInfo($"CheckUpdate");
                Process[] processes = Process.GetProcessesByName(AppConst.AppProcessName);
                if (processes.Length > 0)
                {
                    args.IsUpdateAvailable = false;
                    return args;
                }

                var isNewVersion = VersionHelper.CheckNewVersionOnline();
                if (isNewVersion)
                {
                    var versionOnline = VersionHelper.GetCurrentVersionOnline();
                    args.IsUpdateAvailable = true;
                    args.CurrentVersion = versionOnline.Version;
                    args.DownloadURL = versionOnline.Url;
                    return args;
                }

                return args;
            }
            catch (Exception)
            {
                args.IsUpdateAvailable = false;
                return args;
            }
           

            
        }

        private void StartUpdate(UpdateInfoEventArgs updateInfo)
        {
            try
            {
                var uri = new Uri(updateInfo.DownloadURL);
                if (!Directory.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Update")))
                    Directory.CreateDirectory(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Update"));

                this.FileDownloadPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Update", $"{Guid.NewGuid()}.zip");
                using (WebClient wc = new WebClient())
                {
                    
                    //(sender, arg) => startWatcher_EventArrived(sender, arg);
                    wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    //wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += (sender, arg) => wc_DownloadFileCompleted(sender, arg, updateInfo);
                    wc.DownloadFileAsync(uri, FileDownloadPath);


                }
            }
            catch (Exception)
            {

            }
        }
        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, UpdateInfoEventArgs updateInfo)
        {
            try
            {
               
                var currentFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                var currentFileZip = Path.Combine(currentFolder, "ZipExtractor.exe");
                this.FileZipExtractorPath = Path.Combine(currentFolder, "Update", "ZipExtractor.exe");

                if (File.Exists(currentFileZip))
                {
                    File.Copy(currentFileZip, this.FileZipExtractorPath, true);
                    File.Copy(Path.Combine(currentFolder, "app.manifest"), Path.Combine(currentFolder, "Update", "app.manifest"), true);

                }


                if (File.Exists(FileDownloadPath))
                {
                   
                    ApplicationLoader.PROCESS_INFORMATION info;
                    ApplicationLoader.StartProcessAndBypassUAC($"{FileZipExtractorPath} \"{FileDownloadPath}\" \"{currentFolder}\"", out info);

                }
                else
                {
                    if (File.Exists(FileZipExtractorPath))
                        File.Delete(FileZipExtractorPath);
                    if (Directory.Exists(Path.GetDirectoryName(FileZipExtractorPath)))
                        Directory.Delete(Path.GetDirectoryName(FileZipExtractorPath));
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                LogHelper.LogInfo($"Version updated:{updateInfo.CurrentVersion}");
                VersionHelper.SetVersionRegistry(updateInfo.CurrentVersion);
                this.TimerCheckUpdate.Interval = 60000;
                this.TimerCheckUpdate.Start();
            }
        }
        #endregion


    }
}
