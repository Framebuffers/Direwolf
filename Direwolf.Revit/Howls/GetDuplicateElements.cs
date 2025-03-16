using Autodesk.Revit.DB;
using Direwolf.Definitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direwolf.Revit.Howls
{
    public record class GetDuplicateElements : RevitHowl
    {
        public GetDuplicateElements(Document doc) => SetRevitDocument(doc);
        public override bool Execute()
        {
             using FilteredElementCollector collector = new(GetRevitDocument());
            ICollection<Element> elements = collector
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .Cast<Element>()
                .ToList();

            Dictionary<string, List<Element>> elementGroups = [];

            foreach (Element element in elements)
            {
                try
                {
                    using Location location = element.Location;
                    switch (location)
                    {
                        case LocationPoint locationPoint:
                            {
                                string key = $"{element.GetType().Name}-{double.Round(locationPoint.Point.X)},{double.Round(locationPoint.Point.Y)},{double.Round(locationPoint.Point.Z)}";

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
                                string key = $"{element.GetType().Name}-{locationCurve.Curve.GetEndPoint(0)}-{locationCurve.Curve.GetEndPoint(1)}";

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
                    continue;
                }
            }

            var d = new Dictionary<string, object>()
            {
                ["duplicateElements"] = elementGroups
            };
            SendCatchToCallback(new Prey(d));
            return true;


        }
    }
}
