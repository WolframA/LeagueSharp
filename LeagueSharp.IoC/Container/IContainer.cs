namespace LeagueSharp.IoC.Container
{
    using System;

    public interface IContainer
    {
        #region Public Events

        event Action<object> Activated;

        #endregion

        #region Public Methods and Operators

        object BuildInstance(Type implementation);

        void BuildUp(object instance);

        IContainer CreateChildContainer();

        void RegisterHandler(Type service, string key, Func<IContainer, object> handler);

        object RegisterInstance(Type service, string key, object implementation);

        void RegisterPerRequest(Type service, string key, Type implementation);

        void RegisterSingleton(Type service, string key, Type implementation);

        void UnregisterHandler(Type service, string key);

        #endregion
    }
}