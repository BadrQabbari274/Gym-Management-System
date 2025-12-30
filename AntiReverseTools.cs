using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.Runtime.CompilerServices;

namespace Forma_System
{
    public static class AntiReverseTools
    {
        static string[] suspiciousTools = new string[]
        {
        "dnspy", "x64dbg", "x32dbg", "ollydbg", "fiddler", "wireshark", "httpanalyzer", "httpdebug",
        "de4dot", "dotpeek", "cheatengine", "ida", "scylla", "megadumper" , "VScode" , "Microsoft Visual Studio 2022" , "Microsoft Visual Studio 2021",
        "Microsoft Visual Studio 2020" , "Microsoft Visual Studio 2019" , "Microsoft Visual Studio 2018" , "Microsoft Visual Studio 2017" , "Microsoft Visual Studio 2016",
        "Microsoft Visual Studio 2015" , "Microsoft Visual Studio 2014" , "Microsoft Visual Studio 2013" , "Microsoft Visual Studio 2012" , "Microsoft Visual Studio 2011",
        "Microsoft Visual Studio 2010"
        };

        static Timer checkTimer;

        public static void StartProtection()
        {
            CheckNow(); // أول فحص

            checkTimer = new Timer();
            checkTimer.Interval = 2000; // كل 2 ثواني
            checkTimer.Elapsed += (s, e) => CheckNow();
            checkTimer.Start();
        }

        private static void CheckNow()
        {
            try
            {
                var runningProcesses = Process.GetProcesses();

                foreach (var proc in runningProcesses)
                {
                    string procName = proc.ProcessName.ToLower();

                    if (suspiciousTools.Any(s => procName.Contains(s)))
                    {
                        // تم الكشف عن أداة مشبوهة
                        MessageBox.Show($"تم اكتشاف أداة غير مصرح بها: {proc.ProcessName}", "تحذير أمني", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Application.Current.Shutdown();

                    }
                }

                if (Debugger.IsAttached || Debugger.IsLogging())
                {
                    MessageBox.Show("تم اكتشاف Debugger مرفق!", "تحذير أمني", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erorr: {ex.Message}");
            }
        }
    }
}
