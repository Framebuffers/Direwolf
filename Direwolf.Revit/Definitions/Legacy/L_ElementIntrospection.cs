using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions.Legacy
{
    public readonly record struct L_ElementIntrospection
    {
        public double? Id { get; init; }
        public string? UniqueElementId { get; init; }
        public string? FamilyName { get; init; }
        public int? BuiltInCategory { get; init; }
        public string? Name { get; init; } 
        public L_ElementIntrospection() { }
        public static string ToSql()
        {
            return """
                INSERT INTO "ElementInformation"(
                "uniqueElementId",
                "wolfpackId"
                "idValue",
                "elementVersionId",
                "familyName",
                "builtInCategory",
                "name",
                "parameters"
                ) VALUES (
                @uniqueElementId,
                @wolfpackId,
                @idValue,
                @elementVersionId,
                @familyName,
                @builtInCategory,
                @name,
                @parameters);
                """;
        }
    }

}





