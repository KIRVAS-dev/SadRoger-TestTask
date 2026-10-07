using System;
using UnityEngine;

namespace InterfaceRefs
{
    [Serializable]
    public struct InterfaceRef<T> where T : class
    {
        [SerializeField] UnityEngine.Object target;

        public T Value => target as T;

        public static implicit operator T(InterfaceRef<T> reference) => reference.Value;
    }
}
