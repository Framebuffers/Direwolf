using Direwolf.Definitions;
using Direwolf.Revit.Howlers;

namespace Direwolf.Revit.UI.ExcelInterop
{
    public record class ExcelHowler : RevitHowler
    {
        public void SendToExcel(Prey p)
        {
            // Prey by design are Dictionary<string, object>.
            // And, because they are records after all, they can be easily serialized.
            // The JSON structure of Prey is: 
            // [
            //  { "Result": {
            //      "Key": "TestName",
            //      "Value": "test_result" }
            //      },
            //    "Result": {
            //      ...
            //     }
            //   }
            //  ]
            //
            // [ArrayOfResults] { "Result": { "Key"
        }
    }
}
