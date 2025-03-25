using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Direwolf.Extensions
{
    public static class StringExtensions
    {
        public static void ToConsole(this string s) => Debug.Print(s);
        public static void ToConsoleJson(this string s) => Debug.Print(JsonSerializer.Serialize(s, new JsonSerializerOptions() { WriteIndented = true }));
    }
}
