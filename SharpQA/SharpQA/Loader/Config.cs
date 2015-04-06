using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Serialization;

namespace LeagueSharp.Loader.Data
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class Config
    {
        public bool FirstRun { get; set; }
        public Hotkeys Hotkeys { get; set; }
        public bool Install { get; set; }

        [XmlArrayItem("KnownRepositories", IsNullable = true)]
        public ObservableCollection<string> KnownRepositories { get; set; }

        public string LeagueOfLegendsExePath { get; set; }
        public string Password { get; set; }

        [XmlArrayItem("Profiles", IsNullable = true)]
        public ObservableCollection<Profile> Profiles { get; set; }

        public string SelectedColor { get; set; }
        public string SelectedLanguage { get; set; }
        public Profile SelectedProfile { get; set; }
        public ConfigSettings Settings { get; set; }
        public bool ShowDevOptions { get; set; }
        public bool TosAccepted { get; set; }
        public bool UpdateOnLoad { get; set; }
        public string Username { get; set; }
    }


    [XmlType(AnonymousType = true)]
    public class ConfigSettings
    {
        [XmlArrayItem("GameSettings", IsNullable = true)]
        public ObservableCollection<GameSettings> GameSettings { get; set; }
    }

    public class GameSettings
    {
        public string Name { get; set; }
        public List<string> PosibleValues { get; set; }
        public string SelectedValue { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class Hotkeys
    {
        [XmlArrayItem("SelectedHotkeys", IsNullable = true)]
        public ObservableCollection<HotkeyEntry> SelectedHotkeys { get; set; }
    }

    public class Profile
    {
        public ObservableCollection<LeagueSharpAssembly> InstalledAssemblies { get; set; }
        public string Name { get; set; }
    }

    public class HotkeyEntry
    {
        public Key DefaultKey { get; set; }
        public string Description { get; set; }
        public string DisplayDescription { get; set; }
        public Key Hotkey { get; set; }
        public byte HotkeyInt { get; set; }
        public string HotkeyString { get; set; }
        public string Name { get; set; }
    }

    public enum AssemblyType
    {
        Library,
        Executable,
        Unknown
    }

    public enum AssemblyStatus
    {
        Ready,
        Updating,
        UpdatingError,
        CompilingError,
        Compiling
    }

    [XmlType(AnonymousType = true)]
    [Serializable]
    public class LeagueSharpAssembly
    {
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool InjectChecked { get; set; }
        public bool InstallChecked { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string PathToBinary { get; set; }
        public string PathToProjectFile { get; set; }
        public AssemblyStatus Status { get; set; }
        public string SvnUrl { get; set; }
        public AssemblyType Type { get; set; }
        public string Version { get; set; }
    }
}