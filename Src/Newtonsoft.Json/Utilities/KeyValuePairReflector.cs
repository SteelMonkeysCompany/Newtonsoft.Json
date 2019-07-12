using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
    internal sealed class KeyValuePairReflector
    {
        private PropertyReflector _keyProperty;

        private PropertyReflector _valueProperty;

        private KeyValuePairReflector(Type keyType, Type valueType)
        {
            Type keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);

            _keyProperty = new PropertyReflector(keyValuePairType, nameof(KeyValuePair<object, object>.Key));
            _valueProperty = new PropertyReflector(keyValuePairType, nameof(KeyValuePair<object, object>.Key));
        }

        private static ThreadSafeStore<KeyValueTypesTuple, KeyValuePairReflector> InstanceStore { get; }

        static KeyValuePairReflector()
        {
            InstanceStore = new ThreadSafeStore<KeyValueTypesTuple, KeyValuePairReflector>(t => new KeyValuePairReflector(t.KeyType, t.ValueType));
        }

        public static KeyValuePairReflector OfTypes(Type keyType, Type valueType)
        {
            ValidationUtils.ArgumentNotNull(keyType, nameof(keyType));
            ValidationUtils.ArgumentNotNull(valueType, nameof(valueType));

            return InstanceStore.Get(new KeyValueTypesTuple(keyType, valueType));
        }

        public object GetKey(object keyValuePair) => _keyProperty.Get(keyValuePair);

        public object GetValue(object keyValuePair) => _valueProperty.Get(keyValuePair);
    }
}