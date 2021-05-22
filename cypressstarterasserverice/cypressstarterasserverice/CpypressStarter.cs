using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace cypressstarterasserverice
{
    class CpypressStarter
    {
        private readonly Timer _timer;
        private static string logpath = @"C:\Users\laszab\Desktop\cypress-automation-framework\start.log";
        private static string CypressStart = @"C:\Users\laszab\Desktop\cypress-automation-framework\CypressStart.sh";
        private static string FolderWatch = @"C:\Users\laszab\Desktop\cypress-automation-framework\TestFolder";

        public CpypressStarter()
        {
            _timer = new Timer(10000) { AutoReset = false }; //60000 1  minut
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
           
             var watcher = new FileSystemWatcher(FolderWatch);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Do Nothing With Cypress Panel."); //todo start whitout cmd panel as windows service
                      
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {

            //if (e.ChangeType != WatcherChangeTypes.Changed)
            //{
            //    return;
            //}

            string valueChange = DateTime.Now + $" Changed: {e.FullPath}";
            Console.WriteLine(valueChange);

            using (StreamWriter sw = File.AppendText(logpath))
            {
                sw.WriteLine(valueChange, " cypress started ");
                
            }

            //todo ha megy vmi akkor tobb nem futhat
            var runningProcessByName = Process.GetProcessesByName("bash");
            if (runningProcessByName.Length == 0)
            {
                Process.Start(CypressStart);
            }

           
        }
        
            //!!!!!!!!!!!!!!!!!!!! if something changinig happen in folders test will run.

        
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {

            string value = DateTime.Now + $" Created: {e.FullPath}";
            Console.WriteLine(value);

            using (StreamWriter sw = File.AppendText(logpath))
            {
                sw.WriteLine(value);
            }
        }
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string value = (DateTime.Now + $" Deleted: {e.FullPath}");
            Console.WriteLine(value);

            using (StreamWriter sw = File.AppendText(logpath))
            {
                sw.WriteLine(value);
            }
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            string renamed = (DateTime.Now + $" Renamed:");
            string old = (DateTime.Now + $"    Old: {e.OldFullPath}");
            string neew = (DateTime.Now + $"    New: {e.FullPath}");
            Console.WriteLine(renamed, old, neew);

            using (StreamWriter sw = File.AppendText(logpath))
            {
                sw.WriteLine(renamed, old, neew);
            }
        }
        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());
        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($" Message: {ex.Message}");
                Console.WriteLine(" Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                PrintException(ex.InnerException);

                using (StreamWriter sw = File.AppendText(logpath))
                {
                    sw.WriteLine($" Message: {ex.Message}");
                    sw.WriteLine(" Stacktrace:");
                    sw.WriteLine(ex.StackTrace);
                    sw.WriteLine(ex.InnerException);

                }
            }
        }
        public void Start() 
        { _timer.Start(); }
        
        public void Stop() 
        { 
           _timer.Stop();
        }


        
            





    }
}
