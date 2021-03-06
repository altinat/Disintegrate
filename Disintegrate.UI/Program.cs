﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Squirrel;
using Force.DeepCloner;

namespace Disintegrate.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            try
            {
                using (var mgrTask = UpdateManager.GitHubUpdateManager(@"https://github.com/OrangeFlash81/Disintegrate"))
                {
                    var mgr = mgrTask.Result;

                    SquirrelAwareApp.HandleEvents(
                        onInitialInstall: v =>
                        {
                            mgr.CreateShortcutsForExecutable("Disintegrate.UI.exe", ShortcutLocation.Startup, false);
                        },
                        onAppUpdate: v =>
                        {
                            mgr.CreateShortcutsForExecutable("Disintegrate.UI.exe", ShortcutLocation.Startup, true);
                        }
                    );

                    await mgr.UpdateApp();
                }

                PresenceManager.PreferenceLoader = Customization.Loader.LoadPreferences;

                PresenceManager.Index(new Apps.Dota2App());
                PresenceManager.Index(new Apps.GlobalOffensiveApp());
                PresenceManager.Index(new Apps.HearthstoneApp());
                PresenceManager.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new TrayIconContext(args.Contains("show")));
            }
            catch (Exception exception)
            {
                var logContents = $@"The core Disintegrate app crashed.
Details:
{exception.Message}
{exception.StackTrace}";

                Crash.WriteLog(logContents);
            }
        }
    }
}
