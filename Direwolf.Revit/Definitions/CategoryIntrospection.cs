using Autodesk.Revit.DB;

namespace Direwolf.Revit.Definitions
{
    public readonly record struct CategoryIntrospection(Category c)
    {
        public double? id { get; init; }
        public string? name { get; init; }
        public string? builtInCategory { get; init; }
        public string? categoryType { get; init; }
        public bool? allowsBoundParameters { get; init; }
        public bool? canAddSubcategory { get; init; }
        public bool? hasMaterialQuantities { get; init; }
        public bool? isCuttable { get; init; }
        public bool? isTagCategory { get; init; }
        public bool? isVisibleInUi { get; init; }
        public string? color { get; init; }
        public string? material { get; init; }
        public double? parent { get; init; }
        public IEnumerable<double>? subcategoriesId { get; init; }
    }
}
