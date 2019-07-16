using System.Collections;

namespace Newtonsoft.Json.Utilities
{
    internal sealed class AotDictionaryEnumeratorWrapper : IDictionaryEnumerator
    {
        private readonly IEnumerator _wrapped;

        private readonly KeyValuePairReflector _reflector;

        public object Key { get; private set; }

        public object Value { get; private set; }

        public DictionaryEntry Entry => new DictionaryEntry(Key, Value);

        public object Current => Entry;

        public AotDictionaryEnumeratorWrapper(IEnumerator wrapped, KeyValuePairReflector reflector)
        {
            ValidationUtils.ArgumentNotNull(wrapped, nameof(wrapped));
            ValidationUtils.ArgumentNotNull(reflector, nameof(reflector));

            _wrapped = wrapped;
            _reflector = reflector;
        }

        public bool MoveNext()
        {
            if (_wrapped.MoveNext())
            {
                object current = _wrapped.Current;

                Key = _reflector.GetKey(current);
                Value = _reflector.GetValue(current);

                return true;
            }

            return false;
        }

        public void Reset() => _wrapped.Reset();
    }
}