using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
    internal sealed class AotCollectionWrapper : IWrappedCollection
    {
        /// <summary>
        ///     It implements <see cref="ICollection{T}" />.
        /// </summary>
        private readonly IEnumerable _collection;

        private readonly GenericCollectionReflector _reflector;

        private object _syncRoot;

        public object UnderlyingCollection => _collection;

        public int Count => _reflector.GetCount(_collection);

        public bool IsReadOnly => _reflector.GetIsReadOnly(_collection);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot is null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        bool IList.IsFixedSize => IsReadOnly;

        public object this[int index]
        {
            get => throw new NotSupportedException(nameof(AotCollectionWrapper) + " does not support indexer."); 
            set => throw new NotSupportedException(nameof(AotCollectionWrapper) + " does not support indexer.");
        }

        public AotCollectionWrapper(IEnumerable collection, GenericCollectionReflector reflector)
        {
            _collection = collection;
            _reflector = reflector;
        }

        public void Clear()
        {
            _reflector.Clear(_collection);
        }

        public IEnumerator GetEnumerator() => _collection.GetEnumerator();

        public int Add(object value)
        {
            _reflector.Add(_collection, value);

            return Count - 1;
        }

        public bool Contains(object value)
        {
            return IsCompatible(value) && _reflector.Contains(_collection, value);
        }

        int IList.IndexOf(object value)
        {
            throw new NotSupportedException(nameof(AotCollectionWrapper) + " does not support " + nameof(IList.IndexOf) + ".");
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(nameof(AotCollectionWrapper) + " does not support " + nameof(IList.RemoveAt) + ".");
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(nameof(AotCollectionWrapper) + " does not support " + nameof(IList.Insert) + ".");
        }

        public void Remove(object value)
        {
            if (IsCompatible(value))
            {
                _reflector.Remove(_collection, value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            _reflector.CopyTo(_collection, array, index);
        }

        private bool IsCompatible(object value)
        {
            if (value is null)
            {
                return !_reflector.ItemType.IsValueType() || ReflectionUtils.IsNullableType(_reflector.ItemType);
            }

            Type type = value.GetType();
            return _reflector.ItemType.IsAssignableFrom(type);
        }
    }
}