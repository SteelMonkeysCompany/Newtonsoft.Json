using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
#if HAVE_READ_ONLY_COLLECTIONS
    internal sealed class AotReadOnlyDictionaryWrapper : IWrappedDictionary
    {
        private readonly object _dictionary;

        private readonly ReadOnlyDictionaryReflector _reflector;

        private object _syncRoot;

        public object UnderlyingDictionary => _dictionary;

        public int Count => _reflector.GetCount(_dictionary);

        public bool IsReadOnly => true;

        bool IDictionary.IsFixedSize => true;

        public ICollection Keys => CopySequence(_reflector.GetKeys(_dictionary));

        public ICollection Values => CopySequence(_reflector.GetValues(_dictionary));

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

        public object this[object key] { get => _reflector.GetItem(_dictionary, key); set => throw new NotSupportedException(); }

        public AotReadOnlyDictionaryWrapper(IEnumerable dictionary, ReadOnlyDictionaryReflector reflector)
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

        public void Clear() => throw new NotSupportedException();

        public void Add(object key, object value) => throw new NotSupportedException();

        public bool Contains(object key) => _reflector.ContainsKey(_dictionary, key);

        public void Remove(object key) => throw new NotSupportedException();

        public void CopyTo(Array array, int index) => throw new NotSupportedException();

        private static ICollection CopySequence(IEnumerable sequence)
        {
            var copy = new List<object>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (object item in sequence)
            {
                copy.Add(item);
            }

            return copy;
        }
    }
#endif
}