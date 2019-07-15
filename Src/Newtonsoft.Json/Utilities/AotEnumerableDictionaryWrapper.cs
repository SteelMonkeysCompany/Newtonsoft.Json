using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
    internal sealed class AotEnumerableDictionaryWrapper : IEnumerable<KeyValuePair<object, object>>
    {
        private readonly IEnumerable _enumerableDictionary;

        private readonly KeyValuePairReflector _keyValuePairReflector;

        public AotEnumerableDictionaryWrapper(IEnumerable enumerableDictionary, KeyValuePairReflector keyValuePairReflector)
        {
            ValidationUtils.ArgumentNotNull(enumerableDictionary, nameof(enumerableDictionary));
            ValidationUtils.ArgumentNotNull(keyValuePairReflector, nameof(keyValuePairReflector));

            _enumerableDictionary = enumerableDictionary;
            _keyValuePairReflector = keyValuePairReflector;
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            foreach (object keyValuePair in _enumerableDictionary)
            {
                yield return new KeyValuePair<object, object>(
                    key: _keyValuePairReflector.GetKey(keyValuePair),
                    value: _keyValuePairReflector.GetValue(keyValuePair));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}