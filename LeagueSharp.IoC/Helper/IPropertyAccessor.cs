namespace LeagueSharp.IoC.Helper
{
    /// <summary>
    ///     The IPropertyAccessor interface defines a Property
    ///     accessor.
    /// </summary>
    public interface IPropertyAccessor
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value stored in the Property for
        ///     the specified target.
        /// </summary>
        /// <param name="target">
        ///     Object to retrieve
        ///     the Property from.
        /// </param>
        /// <returns>Property value.</returns>
        object Get(object target);

        /// <summary>
        ///     Sets the value for the Property of
        ///     the specified target.
        /// </summary>
        /// <param name="target">
        ///     Object to set the
        ///     Property on.
        /// </param>
        /// <param name="value">Property value.</param>
        void Set(object target, object value);

        #endregion
    }
}