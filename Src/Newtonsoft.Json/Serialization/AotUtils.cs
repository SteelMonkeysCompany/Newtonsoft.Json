using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
    internal static class AotUtils
    {
        private sealed class PreserveAttribute : Attribute
        {
        }

        private struct AnyType
        {
        }

        private interface IAnyTypeFactory
        {
            object ConstructAnyValue();
        }

        private sealed class SomeTypeFactory<T> : IAnyTypeFactory
        {
            [Preserve]
            public SomeTypeFactory()
            {
            }

            [Preserve, MethodImpl(MethodImplOptions.NoOptimization)]
            public object ConstructAnyValue() => Activator.CreateInstance<T>();
        }

        private static bool? _isNoJit;

        public static bool IsNoJit
        {
            [MethodImpl(MethodImplOptions.NoOptimization)]
            get
            {
                if (_isNoJit is null)
                {
                    try
                    {
                        // Unity and Mono developers always tries to analyze a code that uses a reflection to keep code used through reflection
                        // from non-compilation. So let's make sure that it will be a little harder for them to understand which code is used here.

                        Type closedGenericType = GetClosedGenericTypeConstructor().Invoke(GetGenericTypeDefinition(), GetAnyType());
                        var factory = (IAnyTypeFactory)Activator.CreateInstance(closedGenericType);
                        _ = factory.ConstructAnyValue();

                        _isNoJit = false;
                    }
                    catch
                    {
                        _isNoJit = true;
                    }
                }

                return _isNoJit.GetValueOrDefault();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static Type GetGenericTypeDefinition() => typeof(SomeTypeFactory<>);

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static Type GetAnyType() => typeof(AnyType);

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static Func<Type, Type, Type> GetClosedGenericTypeConstructor() => (definition, arg) => definition.MakeGenericType(arg);
    }
}