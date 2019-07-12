using System;

namespace Newtonsoft.Json.Utilities
{
    internal readonly struct KeyValueTypesTuple : IEquatable<KeyValueTypesTuple>
    {
        public Type KeyType { get; }

        public Type ValueType { get; }

        public KeyValueTypesTuple(Type keyType, Type valueType)
        {
            KeyType = keyType;
            ValueType = valueType;
        }

        public bool Equals(KeyValueTypesTuple other)
        {
            return KeyType == other.KeyType && ValueType == other.ValueType;
        }

        public override bool Equals(object obj) => obj is KeyValueTypesTuple other && Equals(other);

        public override int GetHashCode()
        {
            int h1 = KeyType.GetHashCode();
            int h2 = ValueType.GetHashCode();

            uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }
    }
}