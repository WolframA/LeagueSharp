namespace LeagueSharp.IoC.Binding.Binder
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.IoC.Binding.Attributes;
    using LeagueSharp.IoC.Binding.Helper;
    using LeagueSharp.IoC.Container;

    public class ModelBinder : IBinder
    {
        #region Static Fields

        private static readonly Type NotifyPropertyChanged = typeof(INotifyPropertyChanged);
        private static readonly Type EnumerableType = typeof(IEnumerable);

        #endregion

        #region Public Methods and Operators

        public void Bind(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (!NotifyPropertyChanged.IsInstanceOfType(source))
            {
                Console.WriteLine("Error[{0}] does not implement INotifyPropertyChanged", source.GetType().FullName);
            }

            var injectables = from property in source.GetType().GetProperties()
                              where
                                  property.CanRead && property.CanWrite
                                  && Attribute.IsDefined(property, typeof(BindAttribute))
                              select property;

            foreach (var propertyInfo in injectables)
            {
                var attribute = propertyInfo.GetCustomAttribute<BindAttribute>();
                var converter = IoC.Get<IValueConverter>(attribute.Service.Name + "Converter");
                object target = null;

                Console.WriteLine(
                    "Bind[{0}] Property[{1}] Key[{2}] Service[{3}] Converter[{4}]",
                    propertyInfo.Name,
                    attribute.Property,
                    attribute.Key,
                    attribute.Service,
                    converter);

                //if (EnumerableType.IsAssignableFrom(propertyInfo.PropertyType))
                //{
                //    target = IoC.GetAll(attribute.Service, attribute.Key);
                //}
                //else
                {
                    target = IoC.Get(attribute.Service, attribute.Key);
                }

                var sourceAccessor = new PropertyAccessor(source.GetType(), propertyInfo.Name);
                var targetAccessor = new PropertyAccessor(attribute.Service, attribute.Property ?? propertyInfo.Name);

                ((INotifyPropertyChanged)(target)).PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == targetAccessor.Property)
                        {
                            var sourceValue = sourceAccessor.Get(source);
                            var targetValue = targetAccessor.Get(target);

                            if (sourceValue == null || !sourceValue.Equals(targetValue))
                            {
                                Console.WriteLine(
                                    "Set[ {0} @ {1} ] {2} >>> {3}",
                                    args.PropertyName,
                                    sender.GetType().FullName,
                                    sourceValue,
                                    targetValue);

                                sourceAccessor.Set(source, targetValue);
                            }
                        }
                    };

                ((INotifyPropertyChanged)(source)).PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == sourceAccessor.Property)
                        {
                            var sourceValue = sourceAccessor.Get(source);
                            var targetValue = targetAccessor.Get(target);

                            if (sourceValue == null || !sourceValue.Equals(targetValue))
                            {
                                Console.WriteLine(
                                    "Set[ {0} @ {1} ] {2} >>> {3}",
                                    args.PropertyName,
                                    sender.GetType().FullName,
                                    targetValue,
                                    sourceValue);

                                targetAccessor.Set(target, sourceValue);
                            }
                        }
                    };
            }
        }

        #endregion
    }
}