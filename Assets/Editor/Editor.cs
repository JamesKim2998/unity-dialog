using UnityEditor;
using UnityEngine;

namespace Dialog
{
    public class ComponentEditor<T> : Editor
        where T : Component
    {
        private new T target;

        public T Target
        {
            get { return target ?? (target = (T)base.target); }
        }

        protected virtual void OnEnable()
        {
        }
    }

    public class ScriptableObjectEditor<T> : Editor
        where T : ScriptableObject
    {
        private new T target;

        public T Target
        {
            get { return target ?? (target = (T)base.target); }
        }

        protected virtual void OnEnable()
        {
        }
    }
}