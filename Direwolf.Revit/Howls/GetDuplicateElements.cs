using System.Diagnostics;
using Autodesk.Revit.DB;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetDuplicateElements : RevitHowl
{
    public GetDuplicateElements(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Hunt()
    {
        using FilteredElementCollector collector = new(GetRevitDocument());
        ICollection<Element> elements = collector
            .WhereElementIsNotElementType()
            .WhereElementIsViewIndependent()
            .ToList();

        Dictionary<string, List<Element>> elementGroups = [];

        foreach (var element in elements)
            try
            {
                using var location = element.Location;
                switch (location)
                {
                    case LocationPoint locationPoint:
                    {
                        var key =
                            $"{element.GetType().Name}-{double.Round(locationPoint.Point.X)},{double.Round(locationPoint.Point.Y)},{double.Round(locationPoint.Point.Z)}";

                        if (!elementGroups.TryGetValue(key, out List<Element>? value))
                        {
                            value = [];
                            elementGroups[key] = value;
                        }

                        value.Add(element);
                        break;
                    }

                    case LocationCurve locationCurve:
                    {
                        var key =
                            $"{element.GetType().Name}-{locationCurve.Curve.GetEndPoint(0)}-{locationCurve.Curve.GetEndPoint(1)}";

                        if (!elementGroups.TryGetValue(key, out List<Element>? value))
                        {
                            value = [];
                            elementGroups[key] = value;
                        }

                        value.Add(element);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

        var d = new Dictionary<string, object>
        {
            ["duplicateElements"] = elementGroups
        };
        // SendCatchToCallback(new Prey(d));
        return true;
    }
}