namespace LeagueSharp.IoC.Container
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class IoC
    {
        #region Constructors and Destructors

        static IoC()
        {
            Default = new IoC();
        }

        public IoC()
            : this(new DefaultContainer())
        {
        }

        public IoC(DefaultContainer container)
        {
            this.Locator = container;
            this.Container = container;
        }

        #endregion

        #region Public Properties

        public static IoC Default { get; set; }

        public IContainer Container { get; set; }

        public ILocator Locator { get; set; }

        #endregion

        #region Public Methods and Operators

        public static object BuildInstance(Type implementation)
        {
            return Default.Container.BuildInstance(implementation);
        }

        public static void BuildUp(object instance)
        {
            Default.Container.BuildUp(instance);
        }

        public static IContainer CreateChildContainer()
        {
            return Default.Container.CreateChildContainer();
        }

        public static T Get<T>(string key = null)
        {
            return (T)Default.Locator.GetInstance(typeof(T), key);
        }

        public static object Get(Type type, string key = null)
        {
            return Default.Locator.GetInstance(type, key);
        }

        public static IEnumerable<T> GetAll<T>(string prefix = null)
        {
            return Default.Locator.GetAllInstances(typeof(T), prefix).Cast<T>();
        }

        public static IEnumerable<object> GetAll(Type type, string prefix = null)
        {
            return Default.Locator.GetAllInstances(type, prefix);
        }

        public static IEnumerable<object> GetAllInstances(Type type, string prefix = null)
        {
            return Default.Locator.GetAllInstances(type, prefix);
        }

        public static object GetInstance(Type service, string key)
        {
            return Default.Locator.GetInstance(service, key);
        }

        public static bool HasHandler(Type service, string key)
        {
            return Default.Locator.HasHandler(service, key);
        }

        public static void RegisterHandler(Type service, string key, Func<IContainer, object> handler)
        {
            Default.Container.RegisterHandler(service, key, handler);
        }

        public static object RegisterInstance(Type service, string key, object implementation)
        {
            return Default.Container.RegisterInstance(service, key, implementation);
        }

        public static void RegisterPerRequest(Type service, string key, Type implementation)
        {
            Default.Container.RegisterPerRequest(service, key, implementation);
        }

        public static void RegisterSingleton(Type service, string key, Type implementation)
        {
            Default.Container.RegisterSingleton(service, key, implementation);
        }

        public static void UnregisterHandler(Type service, string key)
        {
            Default.Container.UnregisterHandler(service, key);
        }

        #endregion
    }
}