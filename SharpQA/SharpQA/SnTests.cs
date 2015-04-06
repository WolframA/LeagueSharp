using System;
using System.Diagnostics;
using System.IO;

namespace SharpQA
{
    internal static class SnTests
    {
        private static bool Requirements()
        {
            var result = true;

            try
            {
                if (!Helper.FileExist(KeyFile))
                {
                    result = false;
                }

                if (!Helper.FileExist(SnFile))
                {
                    result = false;
                }

                if (!Helper.FileExist(DllFile))
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
            Log.InfoHeader("Strong Name Utility");
            Log.Test("StrongName.Requirements", Requirements());
            Log.Test("StrongName.Sign", Sign());
            Log.Test("StrongName.Verify", Verify());
            Log.TestFinish();
        }

        private static bool Sign()
        {
            if (!File.Exists(SnFile) || !File.Exists(KeyFile) || !File.Exists(DllFile))
            {
                return false;
            }

            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        WorkingDirectory = SystemDirectory,
                        FileName = SnFile,
                        Arguments = string.Format("-Ra \"{0}\" key.snk", DllFile)
                    }
                };
                p.Start();
                p.WaitForExit();
                Log.Info(p.StandardOutput.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private static bool Verify()
        {
            if (!File.Exists(SnFile) || !File.Exists(KeyFile) || !File.Exists(DllFile))
            {
                return false;
            }

            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        WorkingDirectory = SystemDirectory,
                        FileName = SnFile,
                        Arguments = string.Format("-vf \"{0}\"", DllFile)
                    }
                };
                p.Start();
                p.WaitForExit();
                Log.Info(p.StandardOutput.ReadToEnd());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        private static readonly string SystemDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System");
        private static readonly string KeyFile = Path.Combine(SystemDirectory, "key.snk");
        private static readonly string SnFile = Path.Combine(SystemDirectory, "sn.exe");
        private static readonly string DllFile = Path.Combine(SystemDirectory, "LeagueSharp.dll");
    }
}