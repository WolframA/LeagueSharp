namespace LeagueSharp.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class TypeExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns an instance of the <paramref name="type" /> on which the method is invoked.
        /// </summary>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <returns>An instance of the <paramref name="type" />.</returns>
        public static object GetInstance(this Type type)
        {
            return GetInstance<TypeToIgnore>(type, null);
        }

        /// <summary>
        ///     Returns an instance of the <paramref name="type" /> on which the method is invoked.
        /// </summary>
        /// <typeparam name="TArg">The type of the argument to pass to the constructor.</typeparam>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <param name="argument">The argument to pass to the constructor.</param>
        /// <returns>An instance of the given <paramref name="type" />.</returns>
        public static object GetInstance<TArg>(this Type type, TArg argument)
        {
            return GetInstance<TArg, TypeToIgnore>(type, argument, null);
        }

        /// <summary>
        ///     Returns an instance of the <paramref name="type" /> on which the method is invoked.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument to pass to the constructor.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument to pass to the constructor.</typeparam>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <param name="argument1">The first argument to pass to the constructor.</param>
        /// <param name="argument2">The second argument to pass to the constructor.</param>
        /// <returns>An instance of the given <paramref name="type" />.</returns>
        public static object GetInstance<TArg1, TArg2>(this Type type, TArg1 argument1, TArg2 argument2)
        {
            return GetInstance<TArg1, TArg2, TypeToIgnore>(type, argument1, argument2, null);
        }

        /// <summary>
        ///     Returns an instance of the <paramref name="type" /> on which the method is invoked.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument to pass to the constructor.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument to pass to the constructor.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument to pass to the constructor.</typeparam>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <param name="argument1">The first argument to pass to the constructor.</param>
        /// <param name="argument2">The second argument to pass to the constructor.</param>
        /// <param name="argument3">The third argument to pass to the constructor.</param>
        /// <returns>An instance of the given <paramref name="type" />.</returns>
        public static object GetInstance<TArg1, TArg2, TArg3>(
            this Type type,
            TArg1 argument1,
            TArg2 argument2,
            TArg3 argument3)
        {
            return InstanceCreationFactory<TArg1, TArg2, TArg3>.CreateInstanceOf(type, argument1, argument2, argument3);
        }

        #endregion

        private static class InstanceCreationFactory<TArg1, TArg2, TArg3>
        {
            #region Static Fields

            // This dictionary will hold a cache of object-creation functions, keyed by the Type to create:
            private static readonly Dictionary<Type, Func<TArg1, TArg2, TArg3, object>> _instanceCreationMethods =
                new Dictionary<Type, Func<TArg1, TArg2, TArg3, object>>();

            #endregion

            #region Public Methods and Operators

            public static object CreateInstanceOf(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            {
                CacheInstanceCreationMethodIfRequired(type);

                return _instanceCreationMethods[type].Invoke(arg1, arg2, arg3);
            }

            #endregion

            #region Methods

            private static void CacheInstanceCreationMethodIfRequired(Type type)
            {
                if (_instanceCreationMethods.ContainsKey(type))
                {
                    return;
                }

                var argumentTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };

                var constructorArgumentTypes = argumentTypes.Where(t => t != typeof(TypeToIgnore)).ToArray();

                var constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    constructorArgumentTypes,
                    new ParameterModifier[0]);

                var lamdaParameterExpressions = new[]
                                                    {
                                                        Expression.Parameter(typeof(TArg1), "param1"),
                                                        Expression.Parameter(typeof(TArg2), "param2"),
                                                        Expression.Parameter(typeof(TArg3), "param3")
                                                    };

                Expression[] constructorParameterExpressions =
                    lamdaParameterExpressions.Take(constructorArgumentTypes.Length).ToArray();

                var constructorCallExpression = Expression.New(constructor, constructorParameterExpressions);

                var constructorCallingLambda =
                    Expression.Lambda<Func<TArg1, TArg2, TArg3, object>>(
                        constructorCallExpression,
                        lamdaParameterExpressions).Compile();

                _instanceCreationMethods[type] = constructorCallingLambda;
            }

            #endregion
        }

        // To allow for overloads with differing numbers of arguments, we flag arguments which should be 
        // ignored by using this Type:
        private class TypeToIgnore
        {
        }
    }
}