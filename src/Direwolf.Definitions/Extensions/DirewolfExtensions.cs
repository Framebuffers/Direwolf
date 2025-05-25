using System.Diagnostics;

namespace Direwolf.Definitions.Extensions;

public static class DirewolfExtensions
{
    public static void ToConsole(this string txt)
    {
        Debug.Write(txt);
    }
}