using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace SharpQA
{
    internal static class WindowsTests
    {
        private static readonly List<string> requirements = new List<string>
        {
            "Microsoft .NET Framework 4.5 Multi-Targeting Pack",
            "Microsoft .NET Framework 4.5.1 Multi-Targeting Pack",
            "Microsoft Visual C++ 2012 Redistributable (x86)"
        };

        private static bool Requirements()
        {
            var installed = new List<string>();
            const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            using (var key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (key == null)
                {
                    return false;
                }

                foreach (var subkeyName in key.GetSubKeyNames())
                {
                    using (var subkey = key.OpenSubKey(subkeyName))
                    {
                        if (subkey == null)
                        {
                            continue;
                        }

                        var value = subkey.GetValue("DisplayName");
                        if (value != null)
                        {
                            installed.Add(value.ToString());
                        }
                    }
                }
            }

            foreach (var requirement in requirements)
            {
                if (!installed.Any(s => s.StartsWith(requirement)))
                {
                    Log.Info("Not Found: " + installed.FirstOrDefault(s => s.StartsWith(requirement)));
                    return false;
                }

                Log.Info("Installed: " + installed.FirstOrDefault(s => s.StartsWith(requirement)));
            }

            return true;
        }

        public static void Run()
        {
            Log.InfoHeader("Computer / Software");
            Log.Test("Windows.Requirements", Requirements());
            Log.Test("Windows.Version", Version());
            Log.TestFinish();
        }

        private static bool Version()
        {
            Log.Info("Version: " + Environment.Version);
            Log.Info("OSVersion: " + Environment.OSVersion);
            Log.Info("ApplicationData: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    switch (Environment.OSVersion.Version.Major)
                    {
                        case 6:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                    return true;
                            }
                            break;
                    }
                    break;

                default:
                    return false;
            }

            return false;
        }
    }
}