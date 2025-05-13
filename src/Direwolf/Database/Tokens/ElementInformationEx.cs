using Autodesk.Revit.DB;

namespace Direwolf.Database.Tokens;

//TODO: Fix code to more reliably get information. It's not capturing all properties of an Element.
public readonly record struct ElementInformationEx(
    double Id,
    string UniqueId,
    double ElementTypeId,
    string? Category,
    string? BuiltInCategory,
    double? AssemblyInstanceId,
    double? CreatedPhaseId,
    double? DemolishedPhaseId,
    double? DesignOptionId,
    double? GroupId,
    double? LevelId,
    string? Location,
    string? Name,
    double? OwnerViewId,
    bool? IsPinned,
    bool? IsViewSpecific,
    double? WorksetId)
{
    public static ElementInformationEx Create(Document doc, ElementId e)
    {
        // Just to be safe.
        var element = doc.GetElement(e);
        if (element is null) return new ElementInformationEx();
        
        // Early bailouts:
        //      These conditions trigger an internal Exception in Revit:
        //          - Null ElementType
        //          - Null Category
        //      Not handling these will make the whole main loop fail.
        //      Therefore, we check for null Categories and Types early on. If any one of them is null,
        //      return a blank record.
        //      
        //      If we don't use vars to check for these conditions, Revit will throw an Exception.
        //      So, just to be safe, we check them this way instead of directly inside the if statement.
        var categoryRef = element.Category;
        var elementTypeId = element.GetTypeId();
        
        if (elementTypeId is null) return new ElementInformationEx();
        if (categoryRef is null) return new ElementInformationEx();
        var category = HandleCategory(categoryRef);
        
        return new ElementInformationEx(
            e.Value,
            element.UniqueId,
            elementTypeId.Value, 
            category.CategoryName,
            category.BuiltInCategory,
            SafeGet(() => element.AssemblyInstanceId.Value, -1),
            SafeGet(() => element.CreatedPhaseId.Value, -1),
            SafeGet(() => element.DemolishedPhaseId.Value, -1),
            SafeGet(() => element.DesignOption.Id.Value, -1),
            SafeGet(() => element.GroupId?.Value != -1 ? element.GroupId!.Value : -1, -1),
            SafeGet(() => element.LevelId.Value, -1),
            SafeGet(() => element.Location?.ToString(), string.Empty)!,
            SafeGet(() => element.Name, string.Empty),
            SafeGet(() => element.OwnerViewId.Value, -1),
            SafeGet(() => element.Pinned, false),
            SafeGet(() => element.ViewSpecific, false),
            SafeGet(() => element.WorksetId.IntegerValue, -1));
    }
    
    // Each property is obtained by try/catching each one. Revit tends to throw an Exception that will paralyze
    // the whole process if even a single object throws a NullReferenceException or an InvalidOperationException.
    // It *may* be slower, but it makes sure that the result is valid and no element is left behind.
    private static T SafeGet<T>(Func<T> func, T defaultValue = default!)
    {
        try { return func(); }
        catch { return defaultValue!; }
    }
   
    // Categories need some special handling. If a Category is null, and it's 
    private static (string? CategoryName, string? BuiltInCategory) HandleCategory(Category c)
    {
        return (
            c.Name,
            Enum.Parse(
                    typeof(BuiltInCategory), 
                    c.BuiltInCategory.ToString())
                .ToString());
    }
}