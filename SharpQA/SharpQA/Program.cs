using System;
using System.Diagnostics;
using System.Reflection;

namespace SharpQA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now + " LeagueSharp User Diagnostics Tool " + Assembly.GetExecutingAssembly().GetName().Version + " started." + Environment.NewLine);
            WindowsTests.Run();
            LoaderTests.Run();
            LeagueSharpTests.Run();
            SnTests.Run();
            CommonTests.Run();

            Log.Save();

            Console.WriteLine("press any key to continue");
            Console.ReadKey();
            Process.Start(Log.LogFile);
        }
    }
}