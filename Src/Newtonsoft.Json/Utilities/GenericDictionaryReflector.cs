using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
    internal sealed class GenericDictionaryReflector
    {
        private readonly Type _keyType;

        private readonly Type _valueType;

        private PropertyReflector _countProperty;

        private PropertyReflector _isReadOnlyProperty;

        private PropertyReflector _keysProperty;

        private PropertyReflector _valuesProperty;

        private PropertyReflector _indexer;

        private MethodReflector _clearMethod;

        private MethodReflector _addMethod;

        private MethodReflector _containsKeyMethod;

        private MethodReflector _removeMethod;

        private MethodReflector _copyToMethod;

        private MethodReflector _getEnumeratorMethod;

        public KeyValuePairReflector KeyValuePairReflector => KeyValuePairReflector.OfTypes(_keyType, _valueType);

        private static ThreadSafeStore<KeyValueTypesTuple, GenericDictionaryReflector> InstanceStore { get; }

        static GenericDictionaryReflector()
        {
            InstanceStore = new ThreadSafeStore<KeyValueTypesTuple, GenericDictionaryReflector>(
                t => new GenericDictionaryReflector(t.KeyType, t.ValueType));
        }

        private GenericDictionaryReflector(Type keyType, Type valueType)
        {
            _keyType = keyType;
            _valueType = valueType;
            Type dictionaryType = typeof(IDictionary<,>).MakeGenericType(keyType, valueType);

            _countProperty = new PropertyReflector(dictionaryType, nameof(IDictionary<object, object>.Count));
            _isReadOnlyProperty = new PropertyReflector(dictionaryType, nameof(IDictionary<object, object>.IsReadOnly));
            _keysProperty = new PropertyReflector(dictionaryType, nameof(IDictionary<object, object>.Keys));
            _valuesProperty = new PropertyReflector(dictionaryType, nameof(IDictionary<object, object>.Values));
            _indexer = new PropertyReflector(dictionaryType, "Item");
            _clearMethod = new MethodReflector(dictionaryType, nameof(IDictionary<object, object>.Clear));
            _addMethod = new MethodReflector(dictionaryType, nameof(IDictionary<object, object>.Add));
            _containsKeyMethod = new MethodReflector(dictionaryType, nameof(IDictionary<object, object>.ContainsKey));
            _removeMethod = new MethodReflector(dictionaryType, nameof(IDictionary<object, object>.Remove));
            _copyToMethod = new MethodReflector(dictionaryType, nameof(IDictionary<object, object>.CopyTo));
            _getEnumeratorMethod = new MethodReflector(dictionaryType, nameof(IDictionary<object, object>.GetEnumerator));
        }

        public static GenericDictionaryReflector OfKeyAndValueTypes(Type keyType, Type valueType)
        {
            return InstanceStore.Get(new KeyValueTypesTuple(keyType, valueType));
        }

        public int GetCount(object dictionary) => (int)_countProperty.Get(dictionary);

        public bool GetIsReadOnly(object dictionary) => (bool)_isReadOnlyProperty.Get(dictionary);

        public ICollection GetKeys(object dictionary) => (ICollection)_keysProperty.Get(dictionary);

        public ICollection GetValues(object dictionary) => (ICollection)_valuesProperty.Get(dictionary);

        public object GetItem(object dictionary, object key) => _indexer.Get(dictionary, key);

        public void SetItem(object dictionary, object key, object value) => _indexer.Set(dictionary, value, key);

        public void Clear(object dictionary) => _clearMethod.Invoke(dictionary);

        public void Add(object dictionary, object key, object value) => _addMethod.Invoke(dictionary, key, value);

        public bool ContainsKey(object dictionary, object key) => (bool)_containsKeyMethod.Invoke(dictionary, key);

        public void Remove(object dictionary, object key) => _removeMethod.Invoke(dictionary, key);

        public void CopyTo(object dictionary, Array array, int index) => _copyToMethod.Invoke(dictionary, array, index);

        public IEnumerator GetEnumerator(object dictionary) => (IEnumerator)_getEnumeratorMethod.Invoke(dictionary);
    }
}