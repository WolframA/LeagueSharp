namespace LeagueSharp.IoC.Binding.Injector
{
    public interface IInjector
    {
        #region Public Methods and Operators

        void Inject(object instance);

        #endregion
    }
}