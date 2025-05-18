using System.Diagnostics;

namespace Direwolf.Extensions;

public static class DirewolfExtensions
{
    public static void ToConsole(this string txt)
    {
        Debug.Write(txt);
    }
}