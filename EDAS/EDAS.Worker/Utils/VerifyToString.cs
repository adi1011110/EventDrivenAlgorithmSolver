namespace EDAS.Worker.Utils;

public static class VerifyToString
{
    public static bool IsOverriden<T>()
    {
        var toStringMethod = typeof(T).GetMethod("ToString", Type.EmptyTypes);

        return toStringMethod != null && toStringMethod.DeclaringType != typeof(object);
    }
}
