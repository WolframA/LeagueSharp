namespace LeagueSharp.IoC.Container
{
    using System;

    public class FactoryFactory<T>
    {
        #region Public Methods and Operators

        public Func<T> Create(ILocator container)
        {
            return () => (T)container.GetInstance(typeof(T), null);
        }

        #endregion
    }
}