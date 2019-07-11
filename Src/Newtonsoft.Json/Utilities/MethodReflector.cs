using System;
using System.Reflection;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
    internal struct MethodReflector
    {
        private readonly Type _collectionType;

        private readonly string _name;

        private MethodInfo _cache;

        public MethodReflector(Type collectionType, string name)
        {
            _collectionType = collectionType;
            _name = name;
            _cache = null;
        }

        public object Invoke(object collection, params object[] args)
        {
            MethodInfo method = _cache;
            if (method == null)
            {
                method = _collectionType.GetMethod(_name);
                if (method == null)
                {
                    throw new NotImplementedException("Null result is not expected.");
                }

                Interlocked.CompareExchange(ref _cache, method, null);
            }

            return method.Invoke(collection, args);
        }
    }
}