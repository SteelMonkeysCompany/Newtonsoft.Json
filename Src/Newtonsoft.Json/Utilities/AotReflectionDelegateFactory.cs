using System;
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Func<object> CreateTemporaryDictionary(Type dictionaryKeyType = null, Type dictionaryValueType = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ObjectConstructor<object> CreateDictionaryWrapperConstructor(Type dictionaryKeyType, Type dictionaryValueType, Type genericCollectionDefinitionType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ObjectConstructor<object> CreateEnumerableWrapperConstructor(Type keyType, Type valueType)
        {
            throw new NotImplementedException();
        }
    }
}