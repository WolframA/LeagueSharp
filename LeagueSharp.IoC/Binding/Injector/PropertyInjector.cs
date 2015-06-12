namespace LeagueSharp.IoC.Binding.Injector
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.IoC.Binding.Attributes;
    using LeagueSharp.IoC.Container;

    public class PropertyInjector : IInjector
    {
        #region Static Fields

        private static readonly Type EnumerableType = typeof(IEnumerable);

        #endregion

        #region Public Methods and Operators

        public void Inject(object instance)
        {
            var injectables = from property in instance.GetType().GetProperties()
                              where
                                  property.CanRead && property.CanWrite
                                  && Attribute.IsDefined(property, typeof(InjectAttribute))
                              select property;

            foreach (var propertyInfo in injectables)
            {
                var attribute = propertyInfo.GetCustomAttribute<InjectAttribute>();
                var converter = IoC.Get<IValueConverter>(attribute.Service.Name + "Converter");
                object value = null;

                if (EnumerableType.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    value = IoC.GetAll(attribute.Service, attribute.Key);
                }
                else
                {
                    value = IoC.Get(attribute.Service, attribute.Key);
                }

                Console.WriteLine(
                    "PropertyInjector[{0}] Key[{1}] Service[{2}] Converter[{3}]",
                    propertyInfo.Name,
                    attribute.Key,
                    attribute.Service,
                    converter);

                //if (value == null)
                //{
                //    continue;
                //}

                //if (converter != null)
                //{
                //}

                //if (attribute.BindingMethod == BindingMethod.OneTime)
                //{
                //    propertyInfo.SetValue(instance, value, null);
                //}

                //if (attribute.BindingMethod == BindingMethod.OneWay)
                //{
                //    propertyInfo.SetValue(instance, value, null);
                //}

                //if (attribute.BindingMethod == BindingMethod.TwoWay)
                //{
                //    propertyInfo.SetValue(instance, value, null);
                //}
            }
        }

        #endregion
    }
}