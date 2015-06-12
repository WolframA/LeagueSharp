namespace LeagueSharp.IoC.Container
{
    using System;
    using System.Collections.Generic;

    public class ContainerEntry : List<Func<IContainer, object>>
    {
        #region Public Properties

        public string Key { get; set; }

        public Type Service { get; set; }

        #endregion
    }
}