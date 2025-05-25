using System.Diagnostics;

using Autodesk.Revit.DB;

using Direwolf.Definitions.RevitApi;

namespace Direwolf.Definitions.Extensions;

public static class ElementExtensions
{
    public static bool TryGetParameters(
        this Element              element,
        out  List<RevitParameter> parameters)
    {
        try
        {
            parameters = [];

            if (element.Category is null || element.Document is null)
                return false;
            foreach (object? p in element.Parameters)
            {
                if (p is not Parameter param) continue;
                parameters.Add(RevitParameter.Create(param));
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            parameters = null!;

            return false;
        }
    }
}