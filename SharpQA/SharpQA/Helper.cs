using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpQA
{
    internal static class Helper
    {
        public static string LeagueSharpAppData
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                    "LS" + Environment.UserName.GetHashCode().ToString("X"));
            }
        }

        public static Version GithubVersion(string url)
        {
            using (var c = new WebClient())
            {
                return AssemblyInfoToVersion(c.DownloadString(url));
            }
        }

        public static Version AssemblyInfoToVersion(string content)
        {
            var match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(content);

            if (match.Success)
            {
                return new Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]));
            }

            return null;
        }

        public static bool AssemblyVersion(string s, Version expected = null)
        {
            if (!File.Exists(s))
            {
                return false;
            }

            var info = AssemblyName.GetAssemblyName(s);
            Log.Info("");
            Log.Info("File: " + s);
            Log.Info("Version: " + info.Version);

            if (expected != null)
            {
                Log.Info("Expected Version: " + expected);
            }

            if (info.GetPublicKeyToken().Length > 0)
            {
                Log.Info("PublicKey: " + BitConverter.ToString(info.GetPublicKeyToken()));
            }

            if (expected != null && info.Version < expected)
            {
                return false;
            }

            return true;
        }

        public static bool FileExist(string s)
        {
            if (!File.Exists(s))
            {
                Log.Info("NOT Found: " + s);
                return false;
            }
            Log.Info("Found: " + new FileInfo(s).FullName);
            return true;
        }

        public static string GetRandomName(string oldName, string username)
        {
            var ar1 = Md5Hash(oldName);
            var ar2 = Md5Hash(username);
            const string allowedChars = "0123456789abcdefhijkmnopqrstuvwxyz";
            var result = "";

            for (var i = 0; i < Math.Min(15, Math.Max(3, username.Length)); i++)
            {
                var j = (ar1.ToCharArray()[i]*ar2.ToCharArray()[i])*2;
                j = j%(allowedChars.Length - 1);
                result = result + allowedChars[j];
            }

            return result + ".dll";
        }

        public static string Md5Hash(string s)
        {
            var sb = new StringBuilder();
            HashAlgorithm algorithm = MD5.Create();
            var h = algorithm.ComputeHash(Encoding.Default.GetBytes(s));

            foreach (var b in h)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}