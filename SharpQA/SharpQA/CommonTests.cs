using System;
using System.IO;

namespace SharpQA
{
    internal static class CommonTests
    {
        private static readonly string Common = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System", "LeagueSharp.Common.dll");

        private static bool Requirements()
        {
            if (!Helper.FileExist(Common))
            {
                return false;
            }

            return true;
        }

        public static void Run()
        {
            Log.InfoHeader("LeagueSharp.Common");
            Log.Test("Common.Requirements", Requirements());
            Log.Test("Common.Version", Version());
            Log.TestFinish();
        }

        private static bool Version()
        {
            if (!File.Exists(Common))
            {
                return false;
            }

            if (!Helper.AssemblyVersion(Common, Helper.GithubVersion("https://raw.githubusercontent.com/LeagueSharp/LeagueSharp.Common/master/Properties/AssemblyInfo.cs")))
            {
                return false;
            }

            return true;
        }
    }
}