namespace LeagueSharp.IoC.Container
{
    using System;
    using System.Collections.Generic;

    public interface ILocator
    {
        #region Public Methods and Operators

        IEnumerable<object> GetAllInstances(Type type, string prefix = null);

        object GetInstance(Type service, string key);

        bool HasHandler(Type service, string key);

        #endregion
    }
}