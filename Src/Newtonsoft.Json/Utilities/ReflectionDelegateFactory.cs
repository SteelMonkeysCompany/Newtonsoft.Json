#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json.Serialization;
#if !HAVE_LINQ
using Newtonsoft.Json.Utilities.LinqBridge;
#else
using System.Linq;

#endif

namespace Newtonsoft.Json.Utilities
{
    internal abstract class ReflectionDelegateFactory
    {
        public Func<T, object> CreateGet<T>(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                // https://github.com/dotnet/corefx/issues/26053
                if (propertyInfo.PropertyType.IsByRef)
                {
                    throw new InvalidOperationException("Could not create getter for {0}. ByRef return values are not supported.".FormatWith(CultureInfo.InvariantCulture, propertyInfo));
                }

                return CreateGet<T>(propertyInfo);
            }

            if (memberInfo is FieldInfo fieldInfo)
            {
                return CreateGet<T>(fieldInfo);
            }

            throw new Exception("Could not create getter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
        }

        public Action<T, object> CreateSet<T>(MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                return CreateSet<T>(propertyInfo);
            }

            if (memberInfo is FieldInfo fieldInfo)
            {
                return CreateSet<T>(fieldInfo);
            }

            throw new Exception("Could not create setter for {0}.".FormatWith(CultureInfo.InvariantCulture, memberInfo));
        }

        public abstract MethodCall<T, object> CreateMethodCall<T>(MethodBase method);
        public abstract ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method);
        public abstract Func<T> CreateDefaultConstructor<T>(Type type);
        public abstract Func<T, object> CreateGet<T>(PropertyInfo propertyInfo);
        public abstract Func<T, object> CreateGet<T>(FieldInfo fieldInfo);
        public abstract Action<T, object> CreateSet<T>(FieldInfo fieldInfo);
        public abstract Action<T, object> CreateSet<T>(PropertyInfo propertyInfo);

        /// <remarks>
        ///     Returned constructor returns <see cref="IList" />.
        /// </remarks>
        public virtual Func<object> CreateTemporaryCollectionConstructor(Type collectionItemType)
        {
            Type temporaryListType = typeof(List<>).MakeGenericType(collectionItemType);
            return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(temporaryListType);
        }

        /// <remarks>
        ///     Returned constructor takes <see cref="IList" /> or <see cref="ICollection{T}" />
        ///     and returns <see cref="IWrappedCollection" />.
        /// </remarks>
        public virtual ObjectConstructor<object> CreateCollectionWrapperConstructor(Type collectionItemType)
        {
            Type genericWrapperType = typeof(CollectionWrapper<>).MakeGenericType(collectionItemType);
            Type constructorArgument = typeof(ICollection<>).MakeGenericType(collectionItemType);

            ConstructorInfo genericWrapperConstructor = genericWrapperType.GetConstructor(new[] { constructorArgument });
            return JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(genericWrapperConstructor);
        }

        /// <remarks>
        ///     Returned constructor returns <see cref="IDictionary" />.
        /// </remarks>
        public virtual Func<object> CreateTemporaryDictionary(Type dictionaryKeyType = null, Type dictionaryValueType = null)
        {
            Type temporaryDictionaryType = typeof(Dictionary<,>).MakeGenericType(dictionaryKeyType ?? typeof(object), dictionaryValueType ?? typeof(object));

            return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(temporaryDictionaryType);
        }

        /// <remarks>
        ///     Returned constructor takes collection of type <paramref name="genericCollectionDefinitionType" />
        ///     and returns <see cref="IWrappedDictionary" />.
        /// </remarks>
        public virtual ObjectConstructor<object> CreateDictionaryWrapperConstructor(Type dictionaryKeyType, Type dictionaryValueType, Type genericCollectionDefinitionType)
        {
            var genericWrapperType = typeof(DictionaryWrapper<,>).MakeGenericType(dictionaryKeyType, dictionaryValueType);

            ConstructorInfo genericWrapperConstructor = genericWrapperType.GetConstructor(new[] { genericCollectionDefinitionType });
            return JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(genericWrapperConstructor);
        }

        /// <remarks>
        ///     Returned constructor returns IEnumerable{KeyValuePair{object, object}}.
        /// </remarks>
        public virtual ObjectConstructor<object> CreateEnumerableWrapperConstructor(Type keyType, Type valueType)
        {
            Type enumerableWrapper = typeof(EnumerableDictionaryWrapper<,>).MakeGenericType(keyType, valueType);
            ConstructorInfo constructors = enumerableWrapper.GetConstructors().First();
            return JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructors);
        }
    }
}