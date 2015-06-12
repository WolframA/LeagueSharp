namespace LeagueSharp.IoC.Binding.Injector
{
    using System;
    using System.Linq;

    using LeagueSharp.IoC.Container;

    public class InterfaceInjector : IInjector
    {
        #region Public Methods and Operators

        public void Inject(object instance)
        {
            var injectables = from property in instance.GetType().GetProperties()
                              where property.CanRead && property.CanWrite && property.PropertyType.IsInterface
                              select property;

            foreach (var propertyInfo in injectables)
            {
                var injection = IoC.GetAll(propertyInfo.PropertyType).ToArray();
                if (injection.Any())
                {
                    Console.WriteLine(
                        "InterfaceInjector[{0}] Serice[{1}]",
                        propertyInfo.Name,
                        injection.First().GetType().FullName);
                    propertyInfo.SetValue(instance, injection.First(), null);
                }
            }
        }

        #endregion
    }
}