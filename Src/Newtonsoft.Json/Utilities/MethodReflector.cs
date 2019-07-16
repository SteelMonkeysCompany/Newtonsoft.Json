using System;
using System.Reflection;
using System.Threading;

namespace Newtonsoft.Json.Utilities
{
    internal struct MethodReflector
    {
        private readonly Type _declaredType;

        private readonly string _name;

        private MethodInfo _cache;

        public MethodReflector(Type declaredType, string name)
        {
            _declaredType = declaredType;
            _name = name;
            _cache = null;
        }

        public object Invoke(object target, params object[] args)
        {
            MethodInfo method = _cache;
            if (method is null)
            {
                method = _declaredType.GetMethod(_name);
                if (method is null)
                {
                    throw new NotImplementedException("Null result is not expected.");
                }

                Interlocked.CompareExchange(ref _cache, method, null);
            }

            return method.Invoke(target, args);
        }
    }
}