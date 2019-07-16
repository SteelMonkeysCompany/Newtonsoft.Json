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
            var reflector = GenericCollectionReflector.OfCollectionWithItemType(collectionItemType);

            return args => new AotCollectionWrapper(collection: (IEnumerable)args[0], reflector);
        }

        /// <inheritdoc />
        public override Func<object> CreateTemporaryDictionary(Type dictionaryKeyType = null, Type dictionaryValueType = null)
        {
            return () => new Dictionary<object, object>();
        }

        /// <inheritdoc />
        public override ObjectConstructor<object> CreateDictionaryWrapperConstructor(Type dictionaryKeyType, Type dictionaryValueType)
        {
            var reflector = GenericDictionaryReflector.OfKeyAndValueTypes(dictionaryKeyType, dictionaryValueType);

            return args => new AotDictionaryWrapper(dictionary: (IEnumerable)args[0], reflector);
        }

#if HAVE_READ_ONLY_COLLECTIONS
        /// <inheritdoc />
        public override ObjectConstructor<object> CreateReadOnlyDictionaryWrapperConstructor(Type dictionaryKeyType, Type dictionaryValueType)
        {
            var reflector = ReadOnlyDictionaryReflector.OfKeyAndValueTypes(dictionaryKeyType, dictionaryValueType);

            return args => new AotReadOnlyDictionaryWrapper(dictionary: (IEnumerable)args[0], reflector);
        }
#endif

        /// <inheritdoc />
        public override ObjectConstructor<object> CreateEnumerableWrapperConstructor(Type keyType, Type valueType)
        {
            var keyValuePairReflector = KeyValuePairReflector.OfTypes(keyType, valueType);

            return args => new AotEnumerableDictionaryWrapper(enumerableDictionary: (IEnumerable)args[0], keyValuePairReflector);
        }
    }
}