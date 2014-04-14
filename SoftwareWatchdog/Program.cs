
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MiniJSON;

namespace arcade_cabinets
{
    class Program
    {
        //Used to ensure we only force-focus once. This allows developers to
        //alt-tab during gameplay without the game popping back up every second.
        private static bool _completedFocusHack = false;


        static void Main(string[] args)
        {
            //Ensure they're passing the proper number of arguments.
            if (args.Length != 1)
            {
                DebugLog("Invalid number of arguments specified! Usage:");
                DebugLog("softwarewatchdog.exe <json encoded args here.>");
                DebugLog("");
                DebugLog("See included documentation for json string format.");
                return;
            }

            //Now let's validate the json.
            var jsonData = Json.Deserialize(args[0]) as Dictionary<string, object>;
            if (jsonData == null)
            {
                DebugLog("Invalid json format specified. Use an online json validator to find out why! Json:");
                DebugLog(args[0]);
                return;
            }

            //Now that we know they've given us a valid Json string, lets ensure it has the required output.
            if (!ValidateRequiredJsonArguments(jsonData))
            {
                return;
            }

            //Okay, we now know we have what we need at bare minimum. Lets launch the game.
            Process gameProcess = StartGameProcess(jsonData["gameData"] as Dictionary<string, object>);
            if (gameProcess == null)
            {
                DebugLog("Failure launching game process, aborting!");
            }
            else
            {
                //Now we're going to wait until the game quits.
                bool bLoop = true;
                while (bLoop)
                {
                    //Ensure the game is in focus (see CheckFocusHack for more details)
                    CheckFocusHack(gameProcess);

                    bLoop = !gameProcess.HasExited;

                    System.Threading.Thread.Sleep(1000);
                }
            } 
            
            //Okay we've launched the game, they've played it, they quit. Now we want to launch the launcher again.
            StartProcess(jsonData["launcherPath"] as string);
        }

        private static Process StartGameProcess(Dictionary<string, object> jsonData)
        {
            string gamePath = jsonData["gamePath"] as string;
            string launchArgs = jsonData["launchParams"] as string ?? "";
            return StartProcess(gamePath, launchArgs);
        }

        /// <summary>
        /// Ensures the json string has been passed in with all of the required information
        /// to properly run the watchdog.
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        private static bool ValidateRequiredJsonArguments(Dictionary<string, object> jsonData)
        {
            bool bValid = true;

            //Ensure the launcher we want to re-open exists.
            string launcherPath = jsonData["launcherPath"] as string;
            if (launcherPath == null)
            {
                DebugLog("Missing launcherPath argument!");
                bValid = false;
            }
            else if (!File.Exists(launcherPath))
            {
                DebugLog("Specified launcher doesn't exist! Path:");
                DebugLog(launcherPath);
                bValid = false;
            }

            //Ensure the game we want to open exists.
            var gameData = jsonData["gameData"] as Dictionary<string, object>;
            if (gameData == null)
            {
                DebugLog("Missing gameData argument!");
                bValid = false;
            }
            else
            {
                var gamePath = gameData["gamePath"] as string;
                if (!File.Exists(gamePath))
                {
                    DebugLog("Specified game doesn't exist! Path:");
                    DebugLog(gamePath);
                    bValid = false;
                }
            }

            return bValid;
        }

        /// <summary>
        /// Starts an arbitrary process.
        /// </summary>
        /// <param name="exePath">Path to exe (ie: C:\MyGame.exe)</param>
        /// <param name="launchArgs">Arguments to pass to the executable if required. (ie: -nosound)</param>
        /// <returns></returns>
        private static Process StartProcess(string exePath, string launchArgs = "")
        {
            //This should work all of the time, but having it fall back to C:\ if it doesn't so the Watchdog doesn't crash.
            string gameDirectory = Path.GetDirectoryName(exePath) ?? "C:\\";


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exePath;
            startInfo.WorkingDirectory = gameDirectory;
            startInfo.Arguments = launchArgs;

            return Process.Start(startInfo);
        }

        /// <summary>
        /// Writes out debug information to either the console window (in a Debug application with a window)
        /// or to the disk alongside the softwarewatchdog.exe. 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private static void DebugLog(string format, params object[] args )
        {
#if DEBUG
            Console.WriteLine(format, args);
#else
            string logFile = "logOutput.txt";
            File.AppendAllText(logFile, String.Format(format, args) + Environment.NewLine);
#endif
        }

        /// <summary>
        /// During the writing of V2 of this software (this being V3 now...) it was discovered that if you
        /// spamed on the keyboard while games were loading the game would take over full screen as expected
        /// but the keyboard focus would not be on the game window and it would break the machine.
        /// 
        /// It appears to be a Windows bug that is causing the issue, as it affected all of our games on multiple
        /// computers (ie: XNA, Flash, Love2D/SDL, GameMaker, etc.).
        /// 
        /// During debugging it was found that when a process launches, Windows briefly focuses a program named
        /// "Process Manager". If no keys are spammed during this time the game opens and takes focus as expected.
        /// However, if the keyboard is spammed during opening, it appears that this "Process Manager" process never
        /// loses focus as expected, and thus all keyboard input is sent to this invisible process. 
        /// 
        /// To resolve this, we will check which window is focused while the game is running and if it finds that it
        /// is not the game then it forces the game. This is only done once however, to allow a developer to alt-tab
        /// via keyboard while the game is running after the game boots.
        /// </summary>
        /// <param name="gameProcess">Process of the game which should be in focus.</param>
        private static void CheckFocusHack(Process gameProcess)
        {
            if (!_completedFocusHack)
            {
                //Check the focused window process vs. what's desired.
                IntPtr focusedWindowHandle = GetForegroundWindow();
                Process focusedProcess = FindProcess(focusedWindowHandle);

                if (focusedProcess.ProcessName != gameProcess.ProcessName)
                {
                    SetForegroundWindow(gameProcess.MainWindowHandle);
                }
                else
                {
                    //If the focused process matches the game process, we should be good.
                    _completedFocusHack = true;
                }
            }
        }

        public static Process FindProcess(IntPtr yourHandle)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.Handle == yourHandle)
                {
                    return p;
                }
            }

            return null;
        }

        //DLL Imports for the WinAPI to get/set the focused window.
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
