namespace LeagueSharp.IoC.Container
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using LeagueSharp.IoC.Binding.Binder;
    using LeagueSharp.IoC.Binding.Injector;

    public class DefaultContainer : IContainer, ILocator
    {
        #region Static Fields

        private static readonly Type DelegateType = typeof(Delegate);

        private static readonly Type EnumerableType = typeof(IEnumerable);

        #endregion

        #region Fields

        private readonly List<ContainerEntry> entries;

        #endregion

        #region Constructors and Destructors

        public DefaultContainer()
        {
            this.entries = new List<ContainerEntry>();
        }

        public DefaultContainer(IEnumerable<ContainerEntry> entries)
        {
            this.entries = new List<ContainerEntry>(entries);
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when a new instance is created.
        /// </summary>
        public event Action<object> Activated = delegate { };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Actually does the work of creating the instance and satisfying it's constructor dependencies.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object BuildInstance(Type type)
        {
            var args = this.DetermineConstructorArgs(type);
            return this.ActivateInstance(type, args);
        }

        /// <summary>
        ///     Pushes dependencies into an existing instance based on interface properties with setters.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public void BuildUp(object instance)
        {
            var injectors = this.GetAllInstances(typeof(IInjector)).Cast<IInjector>();

            foreach (var injector in injectors)
            {
                injector.Inject(instance);
            }

            var binders = this.GetAllInstances(typeof(IBinder)).Cast<IBinder>();

            foreach (var binder in binders)
            {
                binder.Bind(instance);
            }
        }

        /// <summary>
        ///     Creates a child container.
        /// </summary>
        /// <returns>A new container.</returns>
        public IContainer CreateChildContainer()
        {
            return new DefaultContainer(this.entries);
        }

        //public IDictionary<string, T> Get<T>(string prefix = null)
        //{
        //    var services = this.entries.Where(e => e.Service == typeof(T));

        //    if (prefix != null)
        //    {
        //        services = services.Where(e => e.Key.StartsWith(prefix));
        //    }

        //    return services.ToDictionary(entry => entry.Key, entry => (T)this.GetInstance(entry.Service, entry.Key));
        //}

        public IEnumerable<object> GetAllInstances(Type service)
        {
            var entry = this.GetEntry(service, null);
            return entry != null ? entry.Select(x => x(this)) : new object[0];
        }

        /// <summary>
        ///     Requests all instances of a given type.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="prefix"></param>
        /// <returns>All the instances or an empty enumerable if none are found.</returns>
        public IEnumerable<object> GetAllInstances(Type service, string prefix = null)
        {
            var entry = this.GetEntry(service, null);
            return entry != null ? entry.Select(x => x(this)) : new object[0];
        }

        /// <summary>
        ///     Requests an instance.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <returns>The instance, or null if a handler is not found.</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public object GetInstance(Type service, string key)
        {
            var entry = this.GetEntry(service, key);

            if (entry != null)
            {
                return entry.Single()(this);
            }

            if (service == null)
            {
                return null;
            }

            if (DelegateType.IsAssignableFrom(service))
            {
                var typeToCreate = service.GetGenericArguments()[0];
                var factoryFactoryType = typeof(FactoryFactory<>).MakeGenericType(typeToCreate);
                var factoryFactoryHost = Activator.CreateInstance(factoryFactoryType);
                var factoryFactoryMethod = factoryFactoryType.GetMethod("Create", new[] { typeof(IContainer) });
                return factoryFactoryMethod.Invoke(factoryFactoryHost, new object[] { this });
            }

            if (EnumerableType.IsAssignableFrom(service) && service.IsGenericType)
            {
                var listType = service.GetGenericArguments()[0];
                var instances = this.GetAllInstances(listType).ToList();
                var array = Array.CreateInstance(listType, instances.Count);

                for (var i = 0; i < array.Length; i++)
                {
                    array.SetValue(instances[i], i);
                }

                return array;
            }

            return null;
        }

        /// <summary>
        ///     Determines if a handler for the service/key has previously been registered.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <returns>True if a handler is registere; false otherwise.</returns>
        public bool HasHandler(Type service, string key)
        {
            return this.GetEntry(service, key) != null;
        }

        /// <summary>
        ///     Registers a custom handler for serving requests from the container.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler(Type service, string key, Func<IContainer, object> handler)
        {
            this.GetOrCreateEntry(service, key).Add(handler);
        }

        /// <summary>
        ///     Registers the instance.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="implementation">The implementation.</param>
        public object RegisterInstance(Type service, string key, object implementation)
        {
            this.RegisterHandler(service, key, container => implementation);
            return implementation;
        }

        /// <summary>
        ///     Registers the instance.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="implementation">The implementation.</param>
        public object RegisterInstance(string key, object implementation)
        {
            this.RegisterHandler(implementation.GetType(), key, container => implementation);
            return implementation;
        }

        /// <summary>
        ///     Registers the class so that a new instance is created on every request.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="implementation">The implementation.</param>
        public void RegisterPerRequest(Type service, string key, Type implementation)
        {
            this.RegisterHandler(service, key, container => container.BuildInstance(implementation));
        }

        /// <summary>
        ///     Registers the class so that it is created once, on first request, and the same instance is returned to all
        ///     requestors thereafter.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        /// <param name="implementation">The implementation.</param>
        public void RegisterSingleton(Type service, string key, Type implementation)
        {
            object singleton = null;
            this.RegisterHandler(
                service,
                key,
                container => singleton ?? (singleton = container.BuildInstance(implementation)));
        }

        /// <summary>
        ///     Unregisters any handlers for the service/key that have previously been registered.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="key">The key.</param>
        public void UnregisterHandler(Type service, string key)
        {
            var entry = this.GetEntry(service, key);
            if (entry != null)
            {
                this.entries.Remove(entry);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an instance of the type with the specified constructor arguments.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The constructor args.</param>
        /// <returns>The created instance.</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        protected virtual object ActivateInstance(Type type, object[] args)
        {
            var instance = args.Length > 0 ? Activator.CreateInstance(type, args) : Activator.CreateInstance(type);
            this.Activated(instance);
            return instance;
        }

        /// <summary>
        ///     Select Eligible Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ConstructorInfo SelectEligibleConstructor(Type type)
        {
            return type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
        }

        /// <summary>
        ///     Determine Constructor Args
        /// </summary>
        /// <param name="implementation"></param>
        /// <returns></returns>
        private object[] DetermineConstructorArgs(Type implementation)
        {
            var args = new List<object>();
            var constructor = SelectEligibleConstructor(implementation);

            if (constructor != null)
            {
                args.AddRange(constructor.GetParameters().Select(info => this.GetInstance(info.ParameterType, null)));
            }

            return args.ToArray();
        }

        /// <summary>
        ///     Get Entry
        /// </summary>
        /// <param name="service"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private IEnumerable<ContainerEntry> GetEntries(Type service, string prefix = null)
        {
            if (service == null && prefix != null) // TODO: finish
            {
                return this.entries.Where(x => x.Key.StartsWith(prefix));
            }

            if (prefix == null)
            {
                return this.entries.Where(x => x.Service == service);
            }

            return this.entries.Where(x => x.Service == service && x.Key == prefix);
        }

        /// <summary>
        ///     Get Entry
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private ContainerEntry GetEntry(Type service, string key)
        {
            if (service == null)
            {
                return this.entries.FirstOrDefault(x => x.Key == key);
            }

            if (key == null)
            {
                return this.entries.FirstOrDefault(x => x.Service == service && x.Key == null)
                       ?? this.entries.FirstOrDefault(x => x.Service == service);
            }

            return this.entries.FirstOrDefault(x => x.Service == service && x.Key == key);
        }

        /// <summary>
        ///     Get Or Create Entry
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private ContainerEntry GetOrCreateEntry(Type service, string key)
        {
            var entry = this.GetEntry(service, key);
            if (entry == null)
            {
                entry = new ContainerEntry { Service = service, Key = key };
                this.entries.Add(entry);
            }

            return entry;
        }

        #endregion
    }
}