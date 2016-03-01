using System;
using UnityEngine;

namespace Dialog
{
    public static class ExtensionMethods
    {
        public static GameObject Instantiate(this GameObject thiz)
        {
            return UnityEngine.Object.Instantiate(thiz);
        }

        public static T Instantiate<T>(this T thiz) where T : Component
        {
            return thiz.gameObject.Instantiate().GetComponent<T>();
        }

        public static T AddComponent<T>(this Component thiz) where T : Component
        {
            return thiz.gameObject.AddComponent<T>();
        }

        public static void CheckAndCall(this Action thiz)
        {
            if (thiz != null) thiz();
        }

        public static void CheckAndCall<T>(this Action<T> thiz, T arg)
        {
            if (thiz != null) thiz(arg);
        }
    }
}