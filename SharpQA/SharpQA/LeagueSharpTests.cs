using System;
using System.IO;

namespace SharpQA
{
    internal static class LeagueSharpTests
    {
        private static bool Requirements()
        {
            var result = true;

            try
            {
                if (!Helper.FileExist(LeagueSharp))
                {
                    result = false;
                }

                if (!Helper.FileExist(LeagueSharpCore))
                {
                    result = false;
                }

                if (!Helper.FileExist(LeagueSharpBootstrap))
                {
                    result = false;
                }

                if (!Helper.FileExist(LeagueSharpSandbox))
                {
                    result = false;
                }

                if (!Helper.FileExist(LeagueSharpSharpDX))
                {
                    result = false;
                }

                if (!Helper.FileExist(LeagueSharpDirect3D9))
                {
                    result = false;
                }

                if (!Helper.FileExist(LeagueSharpXInput))
                {
                    result = false;
                }

                if (!Helper.FileExist(clipperlibrary))
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
            Log.InfoHeader("LeagueSharp Core");
            Log.Test("LeagueSharp.Requirements", Requirements());
            Log.Test("LeagueSharp.Version", Version());
            Log.TestFinish();
        }

        private static bool Version()
        {
            var result = true;

            try
            {
                if (!Helper.AssemblyVersion(LeagueSharp))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(LeagueSharpSandbox))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(LeagueSharpSharpDX))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(LeagueSharpDirect3D9))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(LeagueSharpXInput))
                {
                    result = false;
                }

                if (!Helper.AssemblyVersion(clipperlibrary))
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

        private static readonly string SystemDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System");
        private static readonly string LeagueSharp = Path.Combine(SystemDirectory, "LeagueSharp.dll");
        private static readonly string LeagueSharpCore = Path.Combine(SystemDirectory, "Leaguesharp.Core.dll");
        private static readonly string LeagueSharpBootstrap = Path.Combine(SystemDirectory, "LeagueSharp.Bootstrap.dll");
        private static readonly string LeagueSharpSandbox = Path.Combine(SystemDirectory, "LeagueSharp.Sandbox.dll");
        private static readonly string LeagueSharpSharpDX = Path.Combine(SystemDirectory, "SharpDX.dll");
        private static readonly string LeagueSharpDirect3D9 = Path.Combine(SystemDirectory, "SharpDX.Direct3D9.dll");
        private static readonly string LeagueSharpXInput = Path.Combine(SystemDirectory, "SharpDX.XInput.dll");
        private static readonly string clipperlibrary = Path.Combine(SystemDirectory, "clipper_library.dll");
    }
}