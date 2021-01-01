public static class NullCheck
{
    public static bool IsNull<T>(this T myObject, string message = "") where T : class
    {
        if (myObject is UnityEngine.Object obj)
        {
            if (!obj)
            {
                return false;
            }
        }
        else
        {
            if (myObject == null)
            {
                return false;
            }
        }

        return true;
    }
}