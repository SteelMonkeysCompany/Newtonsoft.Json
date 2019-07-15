using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
#if HAVE_READ_ONLY_COLLECTIONS
    internal sealed class ReadOnlyDictionaryReflector
    {
        private readonly Type _keyType;

        private readonly Type _valueType;

        private PropertyReflector _countProperty;

        private PropertyReflector _keysProperty;

        private PropertyReflector _valuesProperty;

        private PropertyReflector _indexer;

        private MethodReflector _containsKeyMethod;

        public KeyValuePairReflector KeyValuePairReflector => KeyValuePairReflector.OfTypes(_keyType, _valueType);

        private static ThreadSafeStore<KeyValueTypesTuple, ReadOnlyDictionaryReflector> InstanceStore { get; }

        static ReadOnlyDictionaryReflector()
        {
            InstanceStore = new ThreadSafeStore<KeyValueTypesTuple, ReadOnlyDictionaryReflector>(
                t => new ReadOnlyDictionaryReflector(t.KeyType, t.ValueType));
        }

        private ReadOnlyDictionaryReflector(Type keyType, Type valueType)
        {
            _keyType = keyType;
            _valueType = valueType;

            Type dictionaryType = typeof(IReadOnlyDictionary<,>).MakeGenericType(keyType, valueType);

            _countProperty = new PropertyReflector(dictionaryType, nameof(IReadOnlyDictionary<object, object>.Count));
            _keysProperty = new PropertyReflector(dictionaryType, nameof(IReadOnlyDictionary<object, object>.Keys));
            _valuesProperty = new PropertyReflector(dictionaryType, nameof(IReadOnlyDictionary<object, object>.Values));
            _indexer = new PropertyReflector(dictionaryType, "Item");
            _containsKeyMethod = new MethodReflector(dictionaryType, nameof(IDictionary<object, object>.ContainsKey));
        }

        public static ReadOnlyDictionaryReflector OfKeyAndValueTypes(Type keyType, Type valueType)
        {
            return InstanceStore.Get(new KeyValueTypesTuple(keyType, valueType));
        }

        public int GetCount(object dictionary) => (int)_countProperty.Get(dictionary);

        public IEnumerable GetKeys(object dictionary) => (IEnumerable)_keysProperty.Get(dictionary);

        public IEnumerable GetValues(object dictionary) => (IEnumerable)_valuesProperty.Get(dictionary);

        public object GetItem(object dictionary, object key) => _indexer.Get(dictionary, key);

        public bool ContainsKey(object dictionary, object key) => (bool)_containsKeyMethod.Invoke(dictionary, key);
    }
#endif
}