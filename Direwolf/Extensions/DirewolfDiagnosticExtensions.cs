using System.Diagnostics;
using System.Text.Json;

namespace Direwolf.Extensions
{
    public static class DirewolfDiagnosticExtensions
    {
        public static void ToConsole(this string s) => Debug.Print(s);
        public static void ToJson(this string s) => JsonSerializer.Serialize(s);
        public static void ToJsonIndented(this string s) => JsonSerializer.Serialize(s, new JsonSerializerOptions() { WriteIndented = true });
    }

}
