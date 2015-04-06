using System;
using System.Collections.Generic;
using System.IO;

namespace SharpQA
{
    internal static class Log
    {
        public const string LogFile = "SharpQA.txt";
        private static readonly List<string> Messages = new List<string>();
        private static readonly List<string> Tests = new List<string>();

        public static void Info(string s)
        {
            Messages.Add(s);
        }

        public static void InfoFormat(string s, params object[] args)
        {
            Messages.Add(string.Format(s, args));
        }

        public static void InfoHeader(string module)
        {
            Messages.Add("");
            Messages.Add(module + " - Tests");
            Messages.Add("=====================================");
        }

        public static void Save()
        {
            File.WriteAllText(LogFile, DateTime.Now + " LeagueSharp User QA Tests started." + Environment.NewLine + Environment.NewLine);

            File.AppendAllText(LogFile, "Tests" + Environment.NewLine);
            File.AppendAllText(LogFile, "=====================================" + Environment.NewLine);
            File.AppendAllLines(LogFile, Tests);

            File.AppendAllText(LogFile, Environment.NewLine);
            File.AppendAllLines(LogFile, Messages);
        }

        public static void Test(string module, bool result)
        {
            var s = string.Format("{0,-30} {1}", module, result ? "OK" : "FAILED");
            Tests.Add(s);
            Console.WriteLine(s);
        }

        public static void TestFinish()
        {
            Tests.Add("=====================================");
            Console.WriteLine("=====================================");
        }
    }
}