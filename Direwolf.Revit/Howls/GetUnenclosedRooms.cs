using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Direwolf.Definitions;

namespace Direwolf.Revit.Howls;

public record GetUnenclosedRooms : RevitHowl
{
    public GetUnenclosedRooms(Document doc)
    {
        SetRevitDocument(doc);
    }

    public override bool Execute()
    {
        using var roomCollector = new FilteredElementCollector(GetRevitDocument())
            .OfCategory(BuiltInCategory.OST_Rooms)
            .WhereElementIsNotElementType();

        List<string> unenclosedRooms = [];

        foreach (var roomElement in roomCollector)
            if (roomElement is Room room)
            {
                IList<IList<BoundarySegment>> boundarySegments =
                    room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                if (boundarySegments == null || boundarySegments.Count == 0)
                    unenclosedRooms.Add($"Room Name: {room.Name}, Room ID: {room.Id}");
            }

        var d = new Dictionary<string, object>
        {
            ["unenclosedRooms"] = unenclosedRooms
        };
        SendCatchToCallback(new Prey(d));
        return true;
    }
}