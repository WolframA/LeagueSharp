namespace LeagueSharp.IoC.Binding.Attributes
{
    using System;

    public class InjectAttribute : Attribute
    {
        #region Constructors and Destructors

        public InjectAttribute(string key, Type service)
        {
            this.Key = key;
            this.Service = service;
        }

        #endregion

        #region Public Properties

        public string Key { get; set; }

        public Type Service { get; set; }

        #endregion
    }
}