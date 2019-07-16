using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
    internal sealed class GenericCollectionReflector
    {
        private PropertyReflector _countProperty;

        private PropertyReflector _isReadOnlyProperty;

        private MethodReflector _clearMethod;

        private MethodReflector _addMethod;

        private MethodReflector _containsMethod;

        private MethodReflector _removeMethod;

        private MethodReflector _copyToMethod;

        public Type ItemType { get; }

        private static ThreadSafeStore<Type, GenericCollectionReflector> InstanceStore { get; }

        static GenericCollectionReflector()
        {
            InstanceStore = new ThreadSafeStore<Type, GenericCollectionReflector>(itemType => new GenericCollectionReflector(itemType));
        }

        private GenericCollectionReflector(Type itemType)
        {
            ItemType = itemType;
            Type collectionType = typeof(ICollection<>).MakeGenericType(itemType);

            _countProperty = new PropertyReflector(collectionType, nameof(ICollection<object>.Count));
            _isReadOnlyProperty = new PropertyReflector(collectionType, nameof(ICollection<object>.IsReadOnly));
            _clearMethod = new MethodReflector(collectionType, nameof(ICollection<object>.Clear));
            _addMethod = new MethodReflector(collectionType, nameof(ICollection<object>.Add));
            _containsMethod = new MethodReflector(collectionType, nameof(ICollection<object>.Contains));
            _removeMethod = new MethodReflector(collectionType, nameof(ICollection<object>.Remove));
            _copyToMethod = new MethodReflector(collectionType, nameof(ICollection<object>.CopyTo));
        }

        public static GenericCollectionReflector OfCollectionWithItemType(Type itemType)
        {
            ValidationUtils.ArgumentNotNull(itemType, nameof(itemType));

            return InstanceStore.Get(itemType);
        }

        public int GetCount(object collection) => (int)_countProperty.Get(collection);

        public bool GetIsReadOnly(object collection) => (bool)_isReadOnlyProperty.Get(collection);

        public void Clear(object collection) => _clearMethod.Invoke(collection);

        public void Add(object collection, object value) => _addMethod.Invoke(collection, value);

        public bool Contains(object collection, object value) => (bool)_containsMethod.Invoke(collection, value);

        public void Remove(object collection, object value) => _removeMethod.Invoke(collection, value);

        public void CopyTo(object collection, Array array, int arrayIndex) => _copyToMethod.Invoke(collection, array, arrayIndex);
    }
}