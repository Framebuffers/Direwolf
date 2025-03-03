using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
    public readonly record struct Catch(Dictionary<string, object> Result)
    {
        public override string ToString()
        {
            Console.WriteLine("Catched data!");
            return JsonSerializer.Serialize(Result).ToString();
        }
    }
}
