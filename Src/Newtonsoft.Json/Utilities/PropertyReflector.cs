using System;
using System.Reflection;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
    internal struct PropertyReflector
    {
        private readonly Type _declaredType;

        private readonly string _name;

        private PropertyInfo _cache;

        public PropertyReflector(Type declaredType, string name)
        {
            _name = name;
            _declaredType = declaredType;
            _cache = null;
        }

        private PropertyInfo GetProperty()
        {
            PropertyInfo property = _cache;
            if (property is null)
            {
                property = _declaredType.GetProperty(_name);
                if (property is null)
                {
                    throw new NotImplementedException("Null result is not expected.");
                }

                Interlocked.CompareExchange(ref _cache, property, null);
            }

            return property;
        }

        public object Get(object target, params object[] args)
        {
            return GetProperty().GetValue(target, args);
        }

        public void Set(object target, object value, params object[] args)
        {
            GetProperty().SetValue(target, value, args);
        }
    }
}