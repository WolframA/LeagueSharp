namespace LeagueSharp.IoC.Binding.Helper
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;

    /// <summary>
    ///     The PropertyAccessor class provides fast dynamic access
    ///     to a Property of a specified target class.
    /// </summary>
    public class PropertyAccessor : IPropertyAccessor
    {
        #region Fields

        public readonly string Property;

        private readonly bool canRead;

        private readonly bool canWrite;

        private readonly Type propertyType;

        private readonly Type targetType;

        private IPropertyAccessor emittedPropertyAccessor;

        private Hashtable typeHash;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Creates a new Property accessor.
        /// </summary>
        /// <param name="targetType">Target object type.</param>
        /// <param name="property">Property name.</param>
        public PropertyAccessor(Type targetType, string property)
        {
            this.targetType = targetType;
            this.Property = property;
            var propertyInfo = targetType.GetProperty(property);
            //
            // Make sure the Property exists
            //
            if (propertyInfo == null)
            {
                throw new Exception(
                    string.Format("Property \"{0}\" does" + " not exist for type " + "{1}.", property, targetType));
            }
            else
            {
                this.canRead = propertyInfo.CanRead;
                this.canWrite = propertyInfo.CanWrite;
                this.propertyType = propertyInfo.PropertyType;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Whether or not the Property supports read access.
        /// </summary>
        public bool CanRead
        {
            get
            {
                return this.canRead;
            }
        }

        /// <summary>
        ///     Whether or not the Property supports write access.
        /// </summary>
        public bool CanWrite
        {
            get
            {
                return this.canWrite;
            }
        }

        /// <summary>
        ///     The Type of the Property being accessed.
        /// </summary>
        public Type PropertyType
        {
            get
            {
                return this.propertyType;
            }
        }

        /// <summary>
        ///     The Type of object this Property accessor was
        ///     created for.
        /// </summary>
        public Type TargetType
        {
            get
            {
                return this.targetType;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the Property value from the specified target.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <returns>Property value.</returns>
        public object Get(object target)
        {
            if (this.canRead)
            {
                if (this.emittedPropertyAccessor == null)
                {
                    this.Init();
                }
                return this.emittedPropertyAccessor.Get(target);
            }
            else
            {
                throw new Exception(string.Format("Property \"{0}\" does" + " not have a get method.", this.Property));
            }
        }

        /// <summary>
        ///     Sets the Property for the specified target.
        /// </summary>
        /// <param name="target">Target object.</param>
        /// <param name="value">Value to set.</param>
        public void Set(object target, object value)
        {
            if (this.canWrite)
            {
                if (this.emittedPropertyAccessor == null)
                {
                    this.Init();
                }
                this.emittedPropertyAccessor.Set(target, value);
            }
            else
            {
                throw new Exception(string.Format("Property \"{0}\" does" + " not have a set method.", this.Property));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Create an assembly that will provide the get and set methods.
        /// </summary>
        private Assembly EmitAssembly()
        {
            var assemblyName = new AssemblyName();
            assemblyName.Name = "PropertyAccessorAssembly";

            var newAssembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var newModule = newAssembly.DefineDynamicModule("Module");
            var myType = newModule.DefineType("Property", TypeAttributes.Public);

            myType.AddInterfaceImplementation(typeof(IPropertyAccessor));

            var getParamTypes = new[] { typeof(object) };
            var getReturnType = typeof(object);
            var getMethod = myType.DefineMethod(
                "Get",
                MethodAttributes.Public | MethodAttributes.Virtual,
                getReturnType,
                getParamTypes);

            var get = getMethod.GetILGenerator();
            var targetGetMethod = this.targetType.GetMethod("get_" + this.Property);

            if (targetGetMethod != null)
            {
                get.DeclareLocal(typeof(object));
                get.Emit(OpCodes.Ldarg_1);
                get.Emit(OpCodes.Castclass, this.targetType);
                get.EmitCall(OpCodes.Call, targetGetMethod, null);

                if (targetGetMethod.ReturnType.IsValueType)
                {
                    get.Emit(OpCodes.Box, targetGetMethod.ReturnType);
                }

                get.Emit(OpCodes.Stloc_0);
                get.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                get.ThrowException(typeof(MissingMethodException));
            }

            get.Emit(OpCodes.Ret);

            var setParamTypes = new[] { typeof(object), typeof(object) };
            var setMethod = myType.DefineMethod(
                "Set",
                MethodAttributes.Public | MethodAttributes.Virtual,
                null,
                setParamTypes);

            var set = setMethod.GetILGenerator();
            var targetSetMethod = this.targetType.GetMethod("set_" + this.Property);
            if (targetSetMethod != null)
            {
                var paramType = targetSetMethod.GetParameters()[0].ParameterType;
                set.DeclareLocal(paramType);
                set.Emit(OpCodes.Ldarg_1);
                set.Emit(OpCodes.Castclass, this.targetType);
                set.Emit(OpCodes.Ldarg_2);

                if (paramType.IsValueType)
                {
                    set.Emit(OpCodes.Unbox, paramType);
                    if (this.typeHash[paramType] != null)
                    {
                        var load = (OpCode)this.typeHash[paramType];
                        set.Emit(load);
                    }
                    else
                    {
                        set.Emit(OpCodes.Ldobj, paramType);
                    }
                }
                else
                {
                    set.Emit(OpCodes.Castclass, paramType);
                }

                set.EmitCall(OpCodes.Callvirt, targetSetMethod, null);
            }
            else
            {
                set.ThrowException(typeof(MissingMethodException));
            }

            set.Emit(OpCodes.Ret);
            myType.CreateType();

            return newAssembly;
        }

        private void Init()
        {
            this.InitTypes();

            // Create the assembly and an instance of the 
            // Property accessor class.
            var assembly = this.EmitAssembly();
            this.emittedPropertyAccessor = assembly.CreateInstance("Property") as IPropertyAccessor;

            if (this.emittedPropertyAccessor == null)
            {
                throw new Exception("Unable to create Property accessor.");
            }
        }

        private void InitTypes()
        {
            this.typeHash = new Hashtable();
            this.typeHash[typeof(sbyte)] = OpCodes.Ldind_I1;
            this.typeHash[typeof(byte)] = OpCodes.Ldind_U1;
            this.typeHash[typeof(char)] = OpCodes.Ldind_U2;
            this.typeHash[typeof(short)] = OpCodes.Ldind_I2;
            this.typeHash[typeof(ushort)] = OpCodes.Ldind_U2;
            this.typeHash[typeof(int)] = OpCodes.Ldind_I4;
            this.typeHash[typeof(uint)] = OpCodes.Ldind_U4;
            this.typeHash[typeof(long)] = OpCodes.Ldind_I8;
            this.typeHash[typeof(ulong)] = OpCodes.Ldind_I8;
            this.typeHash[typeof(bool)] = OpCodes.Ldind_I1;
            this.typeHash[typeof(double)] = OpCodes.Ldind_R8;
            this.typeHash[typeof(float)] = OpCodes.Ldind_R4;
        }

        #endregion
    }
}