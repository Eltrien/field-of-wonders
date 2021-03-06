﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Web;
namespace Ubiquitous
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(MainForm.ThreadException);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {

                var msg = "Error: " + ex.Source + " " + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;
                SendLog(msg);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            var msg = "Error: " + ex.Source + " " + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;
            SendLog(msg);
        }

        static void SendLog(String msg)
        {

            try
            {
                var wc = new WebClient();
               
                wc.Headers["Content-Type"] = "application/x-www-form-urlencoded; charset=UTF-8";
                wc.UploadString("http://xedocproject.com/crashlog/index.php", String.Format(
                    "p={0}&e={1}", "Ubiquitous", HttpUtility.UrlEncode(msg)));

                //System.IO.File.WriteAllText(@"C:\UbiquitousCrashLog.txt", msg);
                Debug.Print(msg);
            }
            catch { }
        }


    }
}
