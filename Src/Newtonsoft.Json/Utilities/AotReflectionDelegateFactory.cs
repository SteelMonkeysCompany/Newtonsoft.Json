using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Utilities
{
    internal class AotReflectionDelegateFactory : LateBoundReflectionDelegateFactory
    {
        public new static AotReflectionDelegateFactory Instance { get; } = new AotReflectionDelegateFactory();

        /// <inheritdoc />
        public override Func<object> CreateTemporaryCollectionConstructor(Type collectionItemType)
        {
            return () => new List<object>();
        }

        /// <inheritdoc />
        public override ObjectConstructor<object> CreateCollectionWrapperConstructor(Type collectionItemType)
        {
            return args => new AotCollectionWrapper(
                collection: (IEnumerable)args[0],
                reflector: GenericCollectionReflector.OfCollectionWithItemType(collectionItemType));
        }

        /// <inheritdoc />
        public override Func<object> CreateTemporaryDictionary(Type dictionaryKeyType = null, Type dictionaryValueType = null)
        {
            return () => new Dictionary<object, object>();
        }

        /// <inheritdoc />
        public override ObjectConstructor<object> CreateDictionaryWrapperConstructor(Type dictionaryKeyType, Type dictionaryValueType)
        {
            return args => new AotDictionaryWrapper(
                dictionary: (IEnumerable)args[0],
                reflector: GenericDictionaryReflector.OfKeyAndValueTypes(dictionaryKeyType, dictionaryValueType));
        }

#if HAVE_READ_ONLY_COLLECTIONS
        /// <inheritdoc />
        public override ObjectConstructor<object> CreateReadOnlyDictionaryWrapperConstructor(Type dictionaryKeyType, Type dictionaryValueType)
        {
            return args => new AotReadOnlyDictionaryWrapper(
                dictionary: (IEnumerable)args[0],
                reflector: ReadOnlyDictionaryReflector.OfKeyAndValueTypes(dictionaryKeyType, dictionaryValueType));
        }
#endif

        /// <inheritdoc />
        public override ObjectConstructor<object> CreateEnumerableWrapperConstructor(Type keyType, Type valueType)
        {
            throw new NotImplementedException();
        }
    }
}