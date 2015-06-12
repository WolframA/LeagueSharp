namespace LeagueSharp.IoC.Binding
{
    using System;

    public interface IValueConverter
    {
        #region Public Methods and Operators

        object Convert(object source, Type targetType);

        object ConvertBack(object sourceValue, object target);

        #endregion
    }
}