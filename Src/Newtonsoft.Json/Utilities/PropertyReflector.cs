using System;
using System.Reflection;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
    internal struct PropertyReflector
    {
        private static readonly object[] _noArgs = { };

        private readonly Type _collectionType;

        private readonly string _name;

        private PropertyInfo _cache;

        public PropertyReflector(Type collectionType, string name)
        {
            _name = name;
            _collectionType = collectionType;
            _cache = null;
        }

        public object Get(object collection)
        {
            PropertyInfo property = _cache;
            if (property == null)
            {
                property = _collectionType.GetProperty(_name);
                if (property == null)
                {
                    throw new NotImplementedException("Null result is not expected.");
                }

                Interlocked.CompareExchange(ref _cache, property, null);
            }

            return property.GetValue(collection, _noArgs);
        }
    }
}