using System;

public static class ExtensionMethods
{
    public static void CheckAndCall(this Action thiz)
    {
        if (thiz != null) thiz();
    }

    public static void CheckAndCall<T>(this Action<T> thiz, T arg)
    {
        if (thiz != null) thiz(arg);
    }
}
