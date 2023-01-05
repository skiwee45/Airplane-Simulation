using UnityEngine;

public static class NullCheck
{
    public static bool IsNull<T>(this T myObject, string message = "") where T : class
    {
        return (myObject is Object obj) ?
            obj : 
            myObject == null;
    }
}