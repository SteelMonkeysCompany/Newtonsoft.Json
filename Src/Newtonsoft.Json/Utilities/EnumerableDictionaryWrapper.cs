using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Utilities
{
    internal class EnumerableDictionaryWrapper<TEnumeratorKey, TEnumeratorValue> : IEnumerable<KeyValuePair<object, object>>
    {
        private readonly IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

        public EnumerableDictionaryWrapper(IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
        {
            ValidationUtils.ArgumentNotNull(e, nameof(e));
            _e = e;
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            foreach (KeyValuePair<TEnumeratorKey, TEnumeratorValue> item in _e)
            {
                yield return new KeyValuePair<object, object>(item.Key, item.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}