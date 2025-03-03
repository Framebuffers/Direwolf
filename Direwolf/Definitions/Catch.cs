using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Direwolf.Definitions
{
    public readonly record struct Catch([property: JsonExtensionData] Dictionary<string, object> Result)
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(Result).ToString();
        }
    }
  }
