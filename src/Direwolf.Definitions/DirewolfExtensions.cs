using System.Diagnostics;

namespace Direwolf.Definitions;

public static class DirewolfExtensions
{
    public static T TryDo<T>(Func<T> func, T defaultVal = default!)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            return defaultVal;
        }
    }
}