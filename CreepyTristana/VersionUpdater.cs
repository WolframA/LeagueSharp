namespace creepyTristana
{
    using System;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using LeagueSharp;

    public static class VersionUpdater
    {
        public static System.Version Version;
        public static void UpdateCheck()
        {
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        // updater by h3h3
                        using (var c = new WebClient())
                        {
                            var rawVersion =
                                c.DownloadString(
                                    "https://github.com/nabbhacker/ExoryREPO/blob/master/ExorTristana/Properties/AssemblyInfo.cs");

                            var match =
                                new Regex(
                                    @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                                    .Match(rawVersion);
                                    
                            Version = Assembly.GetExecutingAssembly().GetName().Version;        

                            if (match.Success)
                            {
                                var gitVersion =
                                    new Version(
                                        string.Format(
                                            "{0}.{1}.{2}.{3}",
                                            match.Groups[1],
                                            match.Groups[2],
                                            match.Groups[3],
                                            match.Groups[4]));

                                if (gitVersion != Version)
                                {
                                    Game.PrintChat("<font color=\"#FFCC00\">Creep's Tristana</font> is Outdated, don't complain until you update the assembly!");
                                }
                            }
                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
        }
    }
}