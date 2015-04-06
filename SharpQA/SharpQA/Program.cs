using System;
using System.Diagnostics;

namespace SharpQA
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now + " LeagueSharp User QA Tests started." + Environment.NewLine);
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