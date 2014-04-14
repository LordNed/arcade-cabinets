
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using MiniJSON;

namespace arcade_cabinets
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ensure they're passing the proper number of arguments.
            if (args.Length != 1)
            {
#if !DEBUG
                DebugLog("Invalid number of arguments specified! Usage:");
                DebugLog("softwarewatchdog.exe <json encoded args here.>");
                DebugLog("");
                DebugLog("See included documentation for json string format.");
                return;
#endif
            }

            //Now let's validate the json.
            var jsonData = Json.Deserialize(args[0]) as Dictionary<string, object>;
            if (jsonData == null)
            {

            }


        }

        private static void DebugLog(string format, params object[] args )
        {
            Console.WriteLine(format, args);
        }


        void Zero()
        {
            bool bLoop = true;
            Process trackedProcess = null;
            while (bLoop)
            {
                Process[] localByName = Process.GetProcessesByName("bako");
                if (localByName.Length > 0 && trackedProcess == null)
                {
                    trackedProcess = localByName[0];
                }
                else if (localByName.Length == 0)
                {
                    Console.WriteLine("Waiting...");
                    trackedProcess = null;
                }

                if (trackedProcess != null)
                {
                    StringBuilder buffer = new StringBuilder(256);
                    IntPtr handle = GetForegroundWindow();

                    GetWindowText(handle, buffer, 256);

                    if (buffer.ToString() != trackedProcess.ProcessName)
                    {
                        Console.WriteLine("Mismatched focus found. Found: {0} Expected: {1}", buffer.ToString(),
                            trackedProcess.ProcessName);

                        SetForegroundWindow(trackedProcess.MainWindowHandle);
                    }

                    Console.WriteLine("Focused Window: " + buffer);
                }

                System.Threading.Thread.Sleep(250);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    }
}
