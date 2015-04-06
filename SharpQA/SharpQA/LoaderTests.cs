using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LeagueSharp.Loader.Data;

namespace SharpQA
{
    internal static class LoaderTests
    {
        private static Config config;
        private static readonly string ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
        private static readonly string LoaderFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loader.exe");

        private static bool Config()
        {
            try
            {
                if (config == null)
                {
                    return false;
                }

                Log.Info("");
                Log.Info("Auto Inject: " + config.Install);
                Log.Info("Auto Update: " + config.UpdateOnLoad);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private static bool LoadConfig()
        {
            if (!File.Exists(ConfigFile))
            {
                return false;
            }

            try
            {
                var serializer = new XmlSerializer(typeof (Config));
                using (var reader = new StreamReader(ConfigFile, Encoding.UTF8))
                {
                    config = (Config) serializer.Deserialize(reader);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        private static bool Requirements()
        {
            var result = true;

            try
            {
                if (!Helper.FileExist(LoaderFile))
                {
                    result = false;
                }

                if (!Helper.FileExist(ConfigFile))
                {
                    result = false;
                }

                if (!Helper.FileExist(Git2))
                {
                    result = false;
                }

                if (!Helper.FileExist(TaskbarNotificationFile))
                {
                    result = false;
                }

                if (!Helper.FileExist(LibGit2Sharp))
                {
                    result = false;
                }

                if (!Helper.FileExist(MahApps))
                {
                    result = false;
                }

                if (!Helper.FileExist(WindowsInteractivity))
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return result;
        }

        public static void Run()
        {
            Log.InfoHeader("LeagueSharp Loader");
            Log.Test("Loader.Requirements", Requirements());
            Log.Test("Loader.LoadConfig", LoadConfig());
            Log.Test("Loader.Version", Version());
            Log.Test("Loader.Config", Config());
            Log.TestFinish();
        }

        private static bool Version()
        {
            var result = true;

            try
            {
                if (!Helper.AssemblyVersion(LoaderFile, Helper.GithubVersion("https://raw.githubusercontent.com/LeagueSharp/LeagueSharp.Loader/master/Properties/AssemblyInfo.cs")))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(TaskbarNotificationFile))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(LibGit2Sharp))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(MahApps))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(WindowsInteractivity))
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return result;
        }

        private static readonly string BinDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
        private static readonly string Git2 = Path.Combine(BinDirectory, "git2-e0902fb.dll");
        private static readonly string TaskbarNotificationFile = Path.Combine(BinDirectory, "Hardcodet.Wpf.TaskbarNotification.dll");
        private static readonly string LibGit2Sharp = Path.Combine(BinDirectory, "LibGit2Sharp.dll");
        private static readonly string MahApps = Path.Combine(BinDirectory, "MahApps.Metro.dll");
        private static readonly string WindowsInteractivity = Path.Combine(BinDirectory, "System.Windows.Interactivity.dll");
    }
}