using System;
using System.Collections;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
    internal sealed class AotDictionaryWrapper : IWrappedDictionary
    {
        private readonly object _dictionary;

        private readonly GenericDictionaryReflector _reflector;

        private object _syncRoot;

        public object UnderlyingDictionary => _dictionary;

        public int Count => _reflector.GetCount(_dictionary);

        public bool IsReadOnly => _reflector.GetIsReadOnly(_dictionary);

        bool IDictionary.IsFixedSize => false;

        public ICollection Keys => CopyCollection(_reflector.GetKeys(_dictionary));

        public ICollection Values => CopyCollection(_reflector.GetValues(_dictionary));

        public bool IsSynchronized => false;

        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        public object this[object key] { get => _reflector.GetItem(_dictionary, key); set => _reflector.SetItem(_dictionary, key, value); }

        public AotDictionaryWrapper(IEnumerable dictionary, GenericDictionaryReflector reflector)
        {
            ValidationUtils.ArgumentNotNull(dictionary, nameof(dictionary));
            ValidationUtils.ArgumentNotNull(reflector, nameof(reflector));

            _dictionary = dictionary;
            _reflector = reflector;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return new AotDictionaryEnumeratorWrapper(
                wrapped: ((IEnumerable)_dictionary).GetEnumerator(),
                reflector: _reflector.KeyValuePairReflector);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear() => _reflector.Clear(_dictionary);

        public void Add(object key, object value) => _reflector.Add(_dictionary, key, value);

        public bool Contains(object key) => _reflector.ContainsKey(_dictionary, key);

        public void Remove(object key) => _reflector.Remove(_dictionary, key);

        public void CopyTo(Array array, int index) => _reflector.CopyTo(_dictionary, array, index);

        private static ICollection CopyCollection(ICollection collection)
        {
            var copy = new object[collection.Count];
            collection.CopyTo(copy, 0);

            return copy;
        }
    }
}