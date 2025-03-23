using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf.Extensions
{
    public static class DirewolfDiagnosticExtensions
    {
        public static void ToConsole(this string s) => Debug.Print(s);
        public static void ToJson(this string s) => JsonSerializer.Serialize(s);
        public static void ToJsonIndented(this string s) => JsonSerializer.Serialize(s, new JsonSerializerOptions() { WriteIndented = true });
    }
}
